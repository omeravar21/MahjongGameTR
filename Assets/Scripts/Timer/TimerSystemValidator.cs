using System.Reflection;
using System.Text;
using MahjongGame.Session;
using UnityEngine;

namespace MahjongGame.Timer
{
    public static class TimerSystemValidator
    {
        private const float ValidationDurationSeconds = 0.05f;

        public static bool Validate(Transform gameplayRoot, StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            if (gameplayRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] GameplayRoot is missing for timer system validation.");
                return false;
            }

            Transform timerRoot = gameplayRoot.Find("TimerRoot");
            TimerController timerController = timerRoot != null
                ? timerRoot.GetComponent<TimerController>()
                : null;

            passed &= ValidateComponents(timerRoot, timerController, reportBuilder);
            passed &= ValidateTypesAndEvents(reportBuilder);
            passed &= ValidatePublicApi(timerController, reportBuilder);

            if (Application.isPlaying && timerController != null && timerController.enabled)
            {
                passed &= ValidatePlayModeBehavior(timerController, reportBuilder);
            }
            else if (!Application.isPlaying)
            {
                AppendLine(reportBuilder, "[SKIP] Timer play-mode checks require Play Mode on GameScene.");
            }
            else
            {
                AppendLine(reportBuilder, "[SKIP] Timer play-mode checks require an enabled TimerController in Play Mode.");
            }

            AppendLine(reportBuilder, passed
                ? "[PASS] Timer system validation completed successfully."
                : "[FAIL] Timer system validation found issues.");

            return passed;
        }

        private static bool ValidateComponents(
            Transform timerRoot,
            TimerController timerController,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            if (timerRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] TimerRoot is missing under GameplayRoot.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] TimerRoot is present under GameplayRoot.");

            if (timerController == null)
            {
                AppendLine(reportBuilder, "[FAIL] TimerController is missing on TimerRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TimerController is present on TimerRoot.");
            }

            return passed;
        }

        private static bool ValidateTypesAndEvents(StringBuilder reportBuilder)
        {
            bool passed = true;

            if (typeof(TimerDefinition) == null)
            {
                AppendLine(reportBuilder, "[FAIL] TimerDefinition type is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TimerDefinition type is present.");
            }

            if (typeof(TimerEvents) == null)
            {
                AppendLine(reportBuilder, "[FAIL] TimerEvents type is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TimerEvents type is present.");
            }

            passed &= ValidateEventExists(typeof(TimerEvents), nameof(TimerEvents.TimerStarted), reportBuilder);
            passed &= ValidateEventExists(typeof(TimerEvents), nameof(TimerEvents.TimerRemainingTimeChanged), reportBuilder);
            passed &= ValidateEventExists(typeof(TimerEvents), nameof(TimerEvents.TimerPaused), reportBuilder);
            passed &= ValidateEventExists(typeof(TimerEvents), nameof(TimerEvents.TimerResumed), reportBuilder);
            passed &= ValidateEventExists(typeof(TimerEvents), nameof(TimerEvents.TimerExpired), reportBuilder);

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

        private static bool ValidatePublicApi(TimerController timerController, StringBuilder reportBuilder)
        {
            if (timerController == null)
            {
                AppendLine(reportBuilder, "[FAIL] Cannot validate TimerController API because component is missing.");
                return false;
            }

            bool passed = true;
            passed &= ValidatePublicMethod(timerController, nameof(TimerController.TryStartTimer), reportBuilder);
            passed &= ValidatePublicMethod(timerController, nameof(TimerController.TryPauseTimer), reportBuilder);
            passed &= ValidatePublicMethod(timerController, nameof(TimerController.TryResumeTimer), reportBuilder);
            passed &= ValidatePublicMethod(timerController, nameof(TimerController.StopTimer), reportBuilder);
            return passed;
        }

        private static bool ValidatePublicMethod(
            TimerController timerController,
            string methodName,
            StringBuilder reportBuilder)
        {
            MethodInfo method = timerController.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);

            if (method == null)
            {
                AppendLine(reportBuilder, "[FAIL] TimerController." + methodName + " is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] TimerController." + methodName + " is present.");
            return true;
        }

        private static bool ValidatePlayModeBehavior(TimerController timerController, StringBuilder reportBuilder)
        {
            bool passed = true;

            if (!EnsureActiveSessionForValidation(reportBuilder))
            {
                return false;
            }

            if (!timerController.IsRunning)
            {
                AppendLine(reportBuilder, "[FAIL] Timer is not running during an active session.");
                passed = false;
            }
            else if (timerController.RemainingTimeSeconds <= 0f)
            {
                AppendLine(reportBuilder, "[FAIL] Timer remaining time is not positive after session start.");
                passed = false;
            }
            else if (timerController.AllocatedTimeSeconds <= 0f)
            {
                AppendLine(reportBuilder, "[FAIL] Timer allocated time is not positive after session start.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Timer is running with positive remaining time after session start.");
            }

            float remainingBeforeTick = timerController.RemainingTimeSeconds;
            timerController.AdvanceTimerForValidation(0.1f);
            if (timerController.CurrentState != TimerState.Running)
            {
                AppendLine(reportBuilder, "[FAIL] Timer stopped unexpectedly during countdown validation tick.");
                passed = false;
            }
            else if (timerController.RemainingTimeSeconds >= remainingBeforeTick)
            {
                AppendLine(reportBuilder, "[FAIL] Timer remaining time did not decrease after validation tick.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Timer remaining time decreases while running.");
            }

            passed &= ValidateExpirationEvent(timerController, reportBuilder);

            return passed;
        }

        private static bool EnsureActiveSessionForValidation(StringBuilder reportBuilder)
        {
            if (!SessionDirector.HasInstance)
            {
                AppendLine(reportBuilder, "[FAIL] SessionDirector is not available for timer play-mode validation.");
                return false;
            }

            if (!SessionDirector.Instance.IsSessionActive)
            {
                if (!SessionDirector.Instance.TryStartSession(out _))
                {
                    AppendLine(reportBuilder, "[FAIL] Could not start a session for timer play-mode validation.");
                    return false;
                }
            }

            return true;
        }

        private static bool ValidateExpirationEvent(TimerController timerController, StringBuilder reportBuilder)
        {
            bool expiredRaised = false;

            void HandleTimerExpired(TimerExpiredContext context)
            {
                if (context != null)
                {
                    expiredRaised = true;
                }
            }

            TimerEvents.TimerExpired += HandleTimerExpired;
            try
            {
                timerController.StopTimer();
                if (!timerController.TryStartTimer(ValidationDurationSeconds))
                {
                    AppendLine(reportBuilder, "[FAIL] TryStartTimer failed for timer expiration validation.");
                    return false;
                }

                timerController.AdvanceTimerForValidation(ValidationDurationSeconds + 0.01f);

                if (!expiredRaised)
                {
                    AppendLine(reportBuilder, "[FAIL] TimerExpired event was not raised when remaining time reached zero.");
                    return false;
                }

                if (timerController.CurrentState != TimerState.Expired)
                {
                    AppendLine(reportBuilder, "[FAIL] Timer state is not Expired after countdown reached zero.");
                    return false;
                }

                AppendLine(reportBuilder, "[PASS] TimerExpired event fires when remaining time reaches zero.");
                return true;
            }
            finally
            {
                TimerEvents.TimerExpired -= HandleTimerExpired;
                timerController.StopTimer();

                if (SessionDirector.HasInstance && !SessionDirector.Instance.IsSessionActive)
                {
                    SessionDirector.Instance.TryStartSession(out _);
                }
            }
        }

        private static void AppendLine(StringBuilder reportBuilder, string line)
        {
            reportBuilder.AppendLine(line);
        }
    }
}
