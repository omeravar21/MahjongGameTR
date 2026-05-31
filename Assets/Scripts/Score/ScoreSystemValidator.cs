using System.Reflection;
using System.Text;
using MahjongGame.Matching;
using MahjongGame.Session;
using UnityEngine;

namespace MahjongGame.Score
{
    public static class ScoreSystemValidator
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
                AppendLine(reportBuilder, "[FAIL] GameplayRoot is missing for score system validation.");
                return false;
            }

            ScoreController scoreController = gameplayRoot.GetComponent<ScoreController>();

            passed &= ValidateComponents(scoreController, reportBuilder);
            passed &= ValidateTypesAndEvents(reportBuilder);
            passed &= ValidatePublicApi(scoreController, reportBuilder);
            passed &= ValidateDefinition(reportBuilder);
            passed &= ValidateJokerBonusAward(scoreController, reportBuilder);

            if (Application.isPlaying && scoreController != null && scoreController.enabled)
            {
                passed &= ValidatePlayModeBehavior(scoreController, reportBuilder);
            }
            else if (!Application.isPlaying)
            {
                AppendLine(reportBuilder, "[SKIP] Score play-mode checks require Play Mode on GameScene.");
            }
            else
            {
                AppendLine(reportBuilder, "[SKIP] Score play-mode checks require an enabled ScoreController in Play Mode.");
            }

            AppendLine(reportBuilder, passed
                ? "[PASS] Score system validation completed successfully."
                : "[FAIL] Score system validation found issues.");

            return passed;
        }

        private static bool ValidateComponents(ScoreController scoreController, StringBuilder reportBuilder)
        {
            if (scoreController == null)
            {
                AppendLine(reportBuilder, "[FAIL] ScoreController is missing on GameplayRoot.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] ScoreController is present on GameplayRoot.");
            return true;
        }

        private static bool ValidateTypesAndEvents(StringBuilder reportBuilder)
        {
            bool passed = true;

            AppendLine(reportBuilder, typeof(ScoreDefinition) != null
                ? "[PASS] ScoreDefinition type is present."
                : "[FAIL] ScoreDefinition type is missing.");

            AppendLine(reportBuilder, typeof(ScoreEvents) != null
                ? "[PASS] ScoreEvents type is present."
                : "[FAIL] ScoreEvents type is missing.");

            passed &= ValidateEventExists(typeof(ScoreEvents), nameof(ScoreEvents.ScoreChanged), reportBuilder);
            passed &= ValidateEventExists(typeof(ScoreEvents), nameof(ScoreEvents.MatchScoreAwarded), reportBuilder);
            passed &= ValidateEventExists(typeof(ScoreEvents), nameof(ScoreEvents.JokerBonusAwarded), reportBuilder);

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

        private static bool ValidatePublicApi(ScoreController scoreController, StringBuilder reportBuilder)
        {
            if (scoreController == null)
            {
                AppendLine(reportBuilder, "[FAIL] Cannot validate ScoreController API because component is missing.");
                return false;
            }

            bool passed = true;

            if (scoreController.GetType().GetProperty(nameof(ScoreController.CurrentScore), BindingFlags.Instance | BindingFlags.Public) == null)
            {
                AppendLine(reportBuilder, "[FAIL] ScoreController.CurrentScore property is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] ScoreController.CurrentScore property is present.");
            }

            if (scoreController.GetType().GetProperty(nameof(ScoreController.MatchScoreTotal), BindingFlags.Instance | BindingFlags.Public) == null)
            {
                AppendLine(reportBuilder, "[FAIL] ScoreController.MatchScoreTotal property is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] ScoreController.MatchScoreTotal property is present.");
            }

            if (scoreController.GetType().GetProperty(nameof(ScoreController.MatchCount), BindingFlags.Instance | BindingFlags.Public) == null)
            {
                AppendLine(reportBuilder, "[FAIL] ScoreController.MatchCount property is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] ScoreController.MatchCount property is present.");
            }

            return passed;
        }

        private static bool ValidateDefinition(StringBuilder reportBuilder)
        {
            if (ScoreDefinition.BaseMatchScore != 1000)
            {
                AppendLine(reportBuilder, "[FAIL] ScoreDefinition.BaseMatchScore is not 1000.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] ScoreDefinition.BaseMatchScore is 1000.");

            if (ScoreDefinition.JokerEarlyMatchBonus != 2500)
            {
                AppendLine(reportBuilder, "[FAIL] ScoreDefinition.JokerEarlyMatchBonus is not 2500.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] ScoreDefinition.JokerEarlyMatchBonus is 2500.");
            return true;
        }

        private static bool ValidateJokerBonusAward(ScoreController scoreController, StringBuilder reportBuilder)
        {
            if (scoreController == null)
            {
                AppendLine(reportBuilder, "[FAIL] ScoreController is unavailable for joker bonus validation.");
                return false;
            }

            bool jokerBonusAwardedRaised = false;
            int scoreBefore = scoreController.CurrentScore;

            void HandleJokerBonusAwarded(JokerBonusAwardedContext context)
            {
                if (context != null && context.BonusPoints == ScoreDefinition.JokerEarlyMatchBonus)
                {
                    jokerBonusAwardedRaised = true;
                }
            }

            ScoreEvents.JokerBonusAwarded += HandleJokerBonusAwarded;
            try
            {
                scoreController.AwardJokerBonusForValidation(900);
            }
            finally
            {
                ScoreEvents.JokerBonusAwarded -= HandleJokerBonusAwarded;
            }

            if (!jokerBonusAwardedRaised)
            {
                AppendLine(reportBuilder, "[FAIL] JokerBonusAwarded event was not raised.");
                return false;
            }

            if (scoreController.CurrentScore != scoreBefore + ScoreDefinition.JokerEarlyMatchBonus)
            {
                AppendLine(reportBuilder, "[FAIL] Joker bonus did not update current score.");
                return false;
            }

            if (scoreController.EarlyJokerMatchCount <= 0 || scoreController.JokerBonusTotal <= 0)
            {
                AppendLine(reportBuilder, "[FAIL] Joker bonus totals were not tracked.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Joker bonus awards +2500 and tracks totals.");
            return true;
        }

        private static bool ValidatePlayModeBehavior(ScoreController scoreController, StringBuilder reportBuilder)
        {
            bool passed = true;

            if (!EnsureActiveSessionForValidation(reportBuilder))
            {
                return false;
            }

            if (scoreController.CurrentScore != 0)
            {
                AppendLine(reportBuilder, "[FAIL] Score is not zero after session start.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Score resets to zero after session start.");
            }

            passed &= ValidateMatchScoreAward(scoreController, reportBuilder, 1);
            passed &= ValidateMatchScoreAward(scoreController, reportBuilder, 2);

            return passed;
        }

        private static bool ValidateMatchScoreAward(
            ScoreController scoreController,
            StringBuilder reportBuilder,
            int expectedMatchCount)
        {
            bool scoreChangedRaised = false;
            bool matchScoreAwardedRaised = false;
            int observedRunningTotal = -1;

            void HandleScoreChanged(ScoreChangedContext context)
            {
                if (context != null && context.Delta == ScoreDefinition.BaseMatchScore)
                {
                    scoreChangedRaised = true;
                }
            }

            void HandleMatchScoreAwarded(MatchScoreAwardedContext context)
            {
                if (context != null && context.PointsAwarded == ScoreDefinition.BaseMatchScore)
                {
                    matchScoreAwardedRaised = true;
                    observedRunningTotal = context.RunningTotal;
                }
            }

            ScoreEvents.ScoreChanged += HandleScoreChanged;
            ScoreEvents.MatchScoreAwarded += HandleMatchScoreAwarded;
            try
            {
                scoreController.AwardBaseMatchScoreForValidation();

                int expectedScore = ScoreDefinition.BaseMatchScore * expectedMatchCount;
                if (scoreController.CurrentScore != expectedScore)
                {
                    AppendLine(reportBuilder, "[FAIL] CurrentScore is " + scoreController.CurrentScore + " after match " + expectedMatchCount + ". Expected " + expectedScore + ".");
                    return false;
                }

                if (scoreController.MatchScoreTotal != expectedScore)
                {
                    AppendLine(reportBuilder, "[FAIL] MatchScoreTotal is " + scoreController.MatchScoreTotal + " after match " + expectedMatchCount + ". Expected " + expectedScore + ".");
                    return false;
                }

                if (scoreController.MatchCount != expectedMatchCount)
                {
                    AppendLine(reportBuilder, "[FAIL] MatchCount is " + scoreController.MatchCount + " after match " + expectedMatchCount + ".");
                    return false;
                }

                if (!scoreChangedRaised)
                {
                    AppendLine(reportBuilder, "[FAIL] ScoreChanged event was not raised for match " + expectedMatchCount + ".");
                    return false;
                }

                if (!matchScoreAwardedRaised)
                {
                    AppendLine(reportBuilder, "[FAIL] MatchScoreAwarded event was not raised for match " + expectedMatchCount + ".");
                    return false;
                }

                if (observedRunningTotal != expectedScore)
                {
                    AppendLine(reportBuilder, "[FAIL] MatchScoreAwarded running total is incorrect for match " + expectedMatchCount + ".");
                    return false;
                }

                AppendLine(reportBuilder, "[PASS] Match " + expectedMatchCount + " awards +" + ScoreDefinition.BaseMatchScore + " score.");
                return true;
            }
            finally
            {
                ScoreEvents.ScoreChanged -= HandleScoreChanged;
                ScoreEvents.MatchScoreAwarded -= HandleMatchScoreAwarded;
            }
        }

        private static bool EnsureActiveSessionForValidation(StringBuilder reportBuilder)
        {
            if (!SessionDirector.HasInstance)
            {
                AppendLine(reportBuilder, "[FAIL] SessionDirector is not available for score play-mode validation.");
                return false;
            }

            if (!SessionDirector.Instance.IsSessionActive)
            {
                if (!SessionDirector.Instance.TryStartSession(out _))
                {
                    AppendLine(reportBuilder, "[FAIL] Could not start a session for score play-mode validation.");
                    return false;
                }
            }

            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string line)
        {
            reportBuilder.AppendLine(line);
        }
    }
}
