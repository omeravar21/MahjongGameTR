using System.Reflection;
using System.Text;
using MahjongGame.Score;
using MahjongGame.Session;
using UnityEngine;

namespace MahjongGame.Combo
{
    public static class ComboSystemValidator
    {
        private const float ComboWindowSeconds = ComboDefinition.ComboWindowSeconds;

        public static bool Validate(Transform gameplayRoot, StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            if (gameplayRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] GameplayRoot is missing for combo system validation.");
                return false;
            }

            ComboController comboController = gameplayRoot.GetComponent<ComboController>();

            passed &= ValidateComponents(comboController, reportBuilder);
            passed &= ValidateTypesAndEvents(reportBuilder);
            passed &= ValidatePublicApi(comboController, reportBuilder);
            passed &= ValidateDefinition(reportBuilder);

            if (Application.isPlaying && comboController != null && comboController.enabled)
            {
                passed &= ValidatePlayModeBehavior(comboController, reportBuilder);
            }
            else if (!Application.isPlaying)
            {
                AppendLine(reportBuilder, "[SKIP] Combo play-mode checks require Play Mode on GameScene.");
            }
            else
            {
                AppendLine(reportBuilder, "[SKIP] Combo play-mode checks require an enabled ComboController in Play Mode.");
            }

            AppendLine(reportBuilder, passed
                ? "[PASS] Combo system validation completed successfully."
                : "[FAIL] Combo system validation found issues.");

            return passed;
        }

        private static bool ValidateComponents(ComboController comboController, StringBuilder reportBuilder)
        {
            if (comboController == null)
            {
                AppendLine(reportBuilder, "[FAIL] ComboController is missing on GameplayRoot.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] ComboController is present on GameplayRoot.");
            return true;
        }

        private static bool ValidateTypesAndEvents(StringBuilder reportBuilder)
        {
            bool passed = true;

            AppendLine(reportBuilder, typeof(ComboDefinition) != null
                ? "[PASS] ComboDefinition type is present."
                : "[FAIL] ComboDefinition type is missing.");

            AppendLine(reportBuilder, typeof(ComboEvents) != null
                ? "[PASS] ComboEvents type is present."
                : "[FAIL] ComboEvents type is missing.");

            passed &= ValidateEventExists(typeof(ComboEvents), nameof(ComboEvents.ComboChanged), reportBuilder);
            passed &= ValidateEventExists(typeof(ComboEvents), nameof(ComboEvents.ComboIncreased), reportBuilder);
            passed &= ValidateEventExists(typeof(ComboEvents), nameof(ComboEvents.ComboExpired), reportBuilder);

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

        private static bool ValidatePublicApi(ComboController comboController, StringBuilder reportBuilder)
        {
            if (comboController == null)
            {
                AppendLine(reportBuilder, "[FAIL] Cannot validate ComboController API because component is missing.");
                return false;
            }

            bool passed = true;
            passed &= ValidatePublicProperty(comboController, nameof(ComboController.CurrentCombo), reportBuilder);
            passed &= ValidatePublicProperty(comboController, nameof(ComboController.HighestCombo), reportBuilder);
            passed &= ValidatePublicProperty(comboController, nameof(ComboController.TotalComboCount), reportBuilder);
            passed &= ValidatePublicProperty(comboController, nameof(ComboController.ComboWindowRemainingSeconds), reportBuilder);
            passed &= ValidatePublicProperty(comboController, nameof(ComboController.IsComboWindowActive), reportBuilder);
            return passed;
        }

        private static bool ValidatePublicProperty(
            ComboController comboController,
            string propertyName,
            StringBuilder reportBuilder)
        {
            PropertyInfo property = comboController.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            if (property == null)
            {
                AppendLine(reportBuilder, "[FAIL] ComboController." + propertyName + " property is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] ComboController." + propertyName + " property is present.");
            return true;
        }

        private static bool ValidateDefinition(StringBuilder reportBuilder)
        {
            bool passed = true;

            if (!Mathf.Approximately(ComboDefinition.ComboWindowSeconds, ComboWindowSeconds))
            {
                AppendLine(reportBuilder, "[FAIL] ComboDefinition.ComboWindowSeconds is not 3.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] ComboDefinition.ComboWindowSeconds is 3.");
            }

            passed &= ValidateComboBonus(2, 200, reportBuilder);
            passed &= ValidateComboBonus(3, 400, reportBuilder);
            passed &= ValidateComboBonus(4, 600, reportBuilder);
            passed &= ValidateComboBonus(5, 800, reportBuilder);
            passed &= ValidateComboBonus(6, 1200, reportBuilder);
            passed &= ValidateComboBonus(10, 1200, reportBuilder);

            return passed;
        }

        private static bool ValidateComboBonus(int comboLevel, int expectedBonus, StringBuilder reportBuilder)
        {
            if (ScoreDefinition.ResolveComboBonus(comboLevel) != expectedBonus)
            {
                AppendLine(reportBuilder, "[FAIL] Combo bonus for x" + comboLevel + " is incorrect.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Combo x" + comboLevel + " bonus is +" + expectedBonus + ".");
            return true;
        }

        private static bool ValidatePlayModeBehavior(ComboController comboController, StringBuilder reportBuilder)
        {
            if (!EnsureActiveSessionForValidation(reportBuilder))
            {
                return false;
            }

            bool passed = true;

            if (comboController.CurrentCombo != 0)
            {
                AppendLine(reportBuilder, "[FAIL] Combo is not zero after session start.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Combo resets to zero after session start.");
            }

            comboController.RegisterMatchForComboValidation();
            if (comboController.CurrentCombo != 1)
            {
                AppendLine(reportBuilder, "[FAIL] First match did not set combo to 1.");
                passed = false;
            }
            else if (!comboController.IsComboWindowActive)
            {
                AppendLine(reportBuilder, "[FAIL] Combo window did not start after first match.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] First match starts combo chain at x1 with active window.");
            }

            passed &= ValidateComboIncrease(comboController, reportBuilder, expectedCombo: 2, expectedBonus: 200);
            passed &= ValidateComboIncrease(comboController, reportBuilder, expectedCombo: 3, expectedBonus: 400);
            passed &= ValidateComboExpiration(comboController, reportBuilder);

            return passed;
        }

        private static bool ValidateComboIncrease(
            ComboController comboController,
            StringBuilder reportBuilder,
            int expectedCombo,
            int expectedBonus)
        {
            bool comboIncreasedRaised = false;

            void HandleComboIncreased(ComboIncreasedContext context)
            {
                if (context != null && context.ComboLevel == expectedCombo)
                {
                    comboIncreasedRaised = true;
                }
            }

            ComboEvents.ComboIncreased += HandleComboIncreased;
            try
            {
                comboController.RegisterMatchForComboValidation();

                if (comboController.CurrentCombo != expectedCombo)
                {
                    AppendLine(reportBuilder, "[FAIL] Combo x" + expectedCombo + " was not reached.");
                    return false;
                }

                if (!comboIncreasedRaised)
                {
                    AppendLine(reportBuilder, "[FAIL] ComboIncreased event was not raised for x" + expectedCombo + ".");
                    return false;
                }

                AppendLine(reportBuilder, "[PASS] Combo x" + expectedCombo + " awards +" + expectedBonus + " bonus event.");
                return true;
            }
            finally
            {
                ComboEvents.ComboIncreased -= HandleComboIncreased;
            }
        }

        private static bool ValidateComboExpiration(ComboController comboController, StringBuilder reportBuilder)
        {
            bool comboExpiredRaised = false;

            void HandleComboExpired()
            {
                comboExpiredRaised = true;
            }

            ComboEvents.ComboExpired += HandleComboExpired;
            try
            {
                comboController.AdvanceComboWindowForValidation(ComboWindowSeconds + 0.01f);

                if (comboController.CurrentCombo != 0)
                {
                    AppendLine(reportBuilder, "[FAIL] Combo did not reset after window expiration.");
                    return false;
                }

                if (!comboExpiredRaised)
                {
                    AppendLine(reportBuilder, "[FAIL] ComboExpired event was not raised after window expiration.");
                    return false;
                }

                AppendLine(reportBuilder, "[PASS] Combo resets when the 3-second window expires.");
                return true;
            }
            finally
            {
                ComboEvents.ComboExpired -= HandleComboExpired;
            }
        }

        private static bool EnsureActiveSessionForValidation(StringBuilder reportBuilder)
        {
            if (!SessionDirector.HasInstance)
            {
                AppendLine(reportBuilder, "[FAIL] SessionDirector is not available for combo play-mode validation.");
                return false;
            }

            if (!SessionDirector.Instance.IsSessionActive)
            {
                if (!SessionDirector.Instance.TryStartSession(out _))
                {
                    AppendLine(reportBuilder, "[FAIL] Could not start a session for combo play-mode validation.");
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
