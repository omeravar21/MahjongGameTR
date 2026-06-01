using System.Reflection;
using System.Text;
using MahjongGame.Progression;
using MahjongGame.Session;
using UnityEngine;

namespace MahjongGame.UI
{
    public static class PerformanceScreenSystemValidator
    {
        public static bool Validate(Transform gameplayRoot, StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            if (gameplayRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] GameplayRoot is missing for performance screen validation.");
                return false;
            }

            LevelResultController levelResultController = gameplayRoot.GetComponent<LevelResultController>();
            LevelCompletionController levelCompletionController = gameplayRoot.GetComponent<LevelCompletionController>();
            PerformanceScreenController performanceScreenController = ResolvePerformanceScreenController(gameplayRoot);

            passed &= ValidateComponents(
                levelResultController,
                levelCompletionController,
                performanceScreenController,
                reportBuilder);
            passed &= ValidateTypesAndEvents(reportBuilder);
            passed &= ValidateLayout(reportBuilder);

            if (Application.isPlaying
                && levelResultController != null
                && performanceScreenController != null
                && levelResultController.enabled
                && performanceScreenController.enabled)
            {
                passed &= ValidatePlayModeBehavior(
                    levelResultController,
                    performanceScreenController,
                    reportBuilder);
            }
            else if (!Application.isPlaying)
            {
                AppendLine(reportBuilder, "[SKIP] Performance screen play-mode checks require Play Mode on GameScene.");
            }
            else
            {
                AppendLine(reportBuilder, "[SKIP] Performance screen play-mode checks require enabled controllers in Play Mode.");
            }

            AppendLine(reportBuilder, passed
                ? "[PASS] Performance screen validation completed successfully."
                : "[FAIL] Performance screen validation found issues.");

            return passed;
        }

        private static PerformanceScreenController ResolvePerformanceScreenController(Transform gameplayRoot)
        {
            Transform uiRoot = gameplayRoot.Find("UIRoot");
            return uiRoot != null ? uiRoot.GetComponent<PerformanceScreenController>() : null;
        }

        private static bool ValidateComponents(
            LevelResultController levelResultController,
            LevelCompletionController levelCompletionController,
            PerformanceScreenController performanceScreenController,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            if (levelResultController == null)
            {
                AppendLine(reportBuilder, "[FAIL] LevelResultController is missing on GameplayRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] LevelResultController is present on GameplayRoot.");
            }

            if (levelCompletionController == null)
            {
                AppendLine(reportBuilder, "[FAIL] LevelCompletionController is missing on GameplayRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] LevelCompletionController is present on GameplayRoot.");
            }

            if (performanceScreenController == null)
            {
                AppendLine(reportBuilder, "[FAIL] PerformanceScreenController is missing on UIRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] PerformanceScreenController is present on UIRoot.");
            }

            return passed;
        }

        private static bool ValidateTypesAndEvents(StringBuilder reportBuilder)
        {
            bool passed = true;

            AppendLine(reportBuilder, typeof(LevelResultSummary) != null
                ? "[PASS] LevelResultSummary type is present."
                : "[FAIL] LevelResultSummary type is missing.");

            if (typeof(LevelResultSummary).GetProperty(nameof(LevelResultSummary.EarlyJokerMatchCount)) == null)
            {
                AppendLine(reportBuilder, "[FAIL] LevelResultSummary.EarlyJokerMatchCount property is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] LevelResultSummary.EarlyJokerMatchCount property is present.");
            }

            if (typeof(LevelResultSummary).GetProperty(nameof(LevelResultSummary.JokerBonusTotal)) == null)
            {
                AppendLine(reportBuilder, "[FAIL] LevelResultSummary.JokerBonusTotal property is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] LevelResultSummary.JokerBonusTotal property is present.");
            }

            passed &= ValidateEventExists(typeof(LevelResultEvents), nameof(LevelResultEvents.LevelResultReady), reportBuilder);
            passed &= ValidateEventExists(typeof(ProgressionEvents), nameof(ProgressionEvents.LevelCompleted), reportBuilder);
            passed &= ValidateEventExists(typeof(ProgressionEvents), nameof(ProgressionEvents.LevelAdvanced), reportBuilder);

            return passed;
        }

        private static bool ValidateEventExists(System.Type eventType, string eventName, StringBuilder reportBuilder)
        {
            EventInfo eventInfo = eventType.GetEvent(eventName, BindingFlags.Static | BindingFlags.Public);
            if (eventInfo == null)
            {
                AppendLine(reportBuilder, "[FAIL] " + eventType.Name + "." + eventName + " event is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] " + eventType.Name + "." + eventName + " event is present.");
            return true;
        }

        private static bool ValidateLayout(StringBuilder reportBuilder)
        {
            if (!Application.isPlaying)
            {
                AppendLine(reportBuilder, "[SKIP] Performance screen layout is built at runtime during Awake.");
                return true;
            }

            if (!PerformanceScreenController.HasRequiredLayout())
            {
                AppendLine(reportBuilder, "[FAIL] Performance screen UI layout is incomplete.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Performance screen UI layout is present.");
            return true;
        }

        private static bool ValidatePlayModeBehavior(
            LevelResultController levelResultController,
            PerformanceScreenController performanceScreenController,
            StringBuilder reportBuilder)
        {
            if (!EnsureActiveSessionForValidation(reportBuilder))
            {
                return false;
            }

            if (performanceScreenController.IsVisible)
            {
                AppendLine(reportBuilder, "[FAIL] Performance screen is visible before win validation.");
                return false;
            }

            int levelBefore = ResolveCurrentLevel();
            int sessionIdBefore = SessionDirector.Instance.CurrentSession.SessionId;
            bool levelCompletedRaised = false;
            bool levelAdvancedRaised = false;

            void HandleLevelCompleted(LevelCompletedContext context)
            {
                if (context != null && context.CompletedLevel == levelBefore)
                {
                    levelCompletedRaised = true;
                }
            }

            void HandleLevelAdvanced(LevelAdvancedContext context)
            {
                if (context != null
                    && context.PreviousLevel == levelBefore
                    && context.NewLevel == levelBefore + 1)
                {
                    levelAdvancedRaised = true;
                }
            }

            ProgressionEvents.LevelCompleted += HandleLevelCompleted;
            ProgressionEvents.LevelAdvanced += HandleLevelAdvanced;

            try
            {
                if (!SessionDirector.Instance.TryEndSession(SessionEndReason.Win))
                {
                    AppendLine(reportBuilder, "[FAIL] TryEndSession(Win) failed during performance screen validation.");
                    return false;
                }

                if (!levelCompletedRaised)
                {
                    AppendLine(reportBuilder, "[FAIL] LevelCompleted event was not raised after win.");
                    return false;
                }

                if (ResolveCurrentLevel() != levelBefore)
                {
                    AppendLine(reportBuilder, "[FAIL] Current level changed before Next Level selection.");
                    return false;
                }

                AppendLine(reportBuilder, "[PASS] Win marks the current level complete without advancing progression.");

                if (!performanceScreenController.IsVisible)
                {
                    AppendLine(reportBuilder, "[FAIL] Performance screen is not visible after win.");
                    return false;
                }

                LevelResultSummary summary = levelResultController.BuildSummaryForValidation();
                if (summary == null)
                {
                    AppendLine(reportBuilder, "[FAIL] Level result summary is null after win.");
                    return false;
                }

                if (summary.Score < 0 || summary.CompletionTimeSeconds < 0f || summary.TotalComboCount < 0)
                {
                    AppendLine(reportBuilder, "[FAIL] Level result summary contains invalid values.");
                    return false;
                }

                AppendLine(reportBuilder, "[PASS] Performance screen displays after win with valid summary data.");

                performanceScreenController.InvokeNextLevelForValidation();

                if (!performanceScreenController.IsVisible)
                {
                    AppendLine(reportBuilder, "[PASS] Performance screen hides after Next Level is selected.");
                }
                else
                {
                    AppendLine(reportBuilder, "[FAIL] Performance screen remains visible after Next Level is selected.");
                    return false;
                }

                if (!levelAdvancedRaised)
                {
                    AppendLine(reportBuilder, "[FAIL] LevelAdvanced event was not raised after Next Level selection.");
                    return false;
                }

                if (!SessionDirector.Instance.IsSessionActive)
                {
                    AppendLine(reportBuilder, "[FAIL] Session is not active after Next Level selection.");
                    return false;
                }

                if (SessionDirector.Instance.CurrentSession.SessionId <= sessionIdBefore)
                {
                    AppendLine(reportBuilder, "[FAIL] Session id did not increase after Next Level selection.");
                    return false;
                }

                if (ResolveCurrentLevel() != levelBefore + 1)
                {
                    AppendLine(reportBuilder, "[FAIL] Player level did not advance after Next Level selection.");
                    return false;
                }

                AppendLine(reportBuilder, "[PASS] Next Level advances progression and starts a new session.");
                return true;
            }
            finally
            {
                ProgressionEvents.LevelCompleted -= HandleLevelCompleted;
                ProgressionEvents.LevelAdvanced -= HandleLevelAdvanced;
            }
        }

        private static bool EnsureActiveSessionForValidation(StringBuilder reportBuilder)
        {
            if (!SessionDirector.HasInstance)
            {
                AppendLine(reportBuilder, "[FAIL] SessionDirector is not available for performance screen validation.");
                return false;
            }

            if (!SessionDirector.Instance.IsSessionActive)
            {
                if (!SessionDirector.Instance.TryStartSession(out _))
                {
                    AppendLine(reportBuilder, "[FAIL] Could not start a session for performance screen validation.");
                    return false;
                }
            }

            return true;
        }

        private static int ResolveCurrentLevel()
        {
            if (PlayerProgressionDirector.HasInstance)
            {
                return PlayerProgressionDirector.Instance.CurrentLevel;
            }

            return LevelProgressData.MinLevel;
        }

        private static void AppendLine(StringBuilder reportBuilder, string line)
        {
            reportBuilder.AppendLine(line);
        }
    }
}
