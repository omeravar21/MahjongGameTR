using System.Reflection;
using System.Text;
using UnityEngine;

namespace MahjongGame.Progression
{
    public static class ProgressionSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateTypes(reportBuilder);
            passed &= ValidateDefinitionRules(reportBuilder);
            passed &= ValidateDirectorBehavior(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Progression system validation completed successfully."
                : "[FAIL] Progression system validation found issues.");

            return passed;
        }

        private static bool ValidateTypes(StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= ValidateTypeExists(typeof(LevelProgressionDefinition), reportBuilder);
            passed &= ValidateTypeExists(typeof(LevelProgressionResult), reportBuilder);
            passed &= ValidateTypeExists(typeof(ProgressionEvents), reportBuilder);
            passed &= ValidateTypeExists(typeof(PlayerProgressionDirector), reportBuilder);

            passed &= ValidateEventExists(
                typeof(ProgressionEvents),
                nameof(ProgressionEvents.LevelCompleted),
                reportBuilder);
            passed &= ValidateEventExists(
                typeof(ProgressionEvents),
                nameof(ProgressionEvents.LevelAdvanced),
                reportBuilder);

            return passed;
        }

        private static bool ValidateDefinitionRules(StringBuilder reportBuilder)
        {
            bool passed = true;

            if (LevelProgressionDefinition.GetNextLevelNumber(1) != 2)
            {
                AppendLine(reportBuilder, "[FAIL] Next level after level 1 is not level 2.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Linear next-level rule resolves level 1 to level 2.");
            }

            if (LevelProgressionDefinition.GetNextLevelNumber(LevelProgressData.MaxLevel) != LevelProgressData.MaxLevel)
            {
                AppendLine(reportBuilder, "[FAIL] Next level after max level is not clamped to max level.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Max level progression is clamped correctly.");
            }

            if (!LevelProgressionDefinition.CanAdvanceFrom(1)
                || LevelProgressionDefinition.CanAdvanceFrom(LevelProgressData.MaxLevel))
            {
                AppendLine(reportBuilder, "[FAIL] CanAdvanceFrom boundaries are incorrect.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] CanAdvanceFrom boundaries are correct.");
            }

            return passed;
        }

        private static bool ValidateDirectorBehavior(StringBuilder reportBuilder)
        {
            GameObject validationObject = new GameObject("ProgressionSystemValidator_Temp");
            PlayerProgressionDirector director = validationObject.AddComponent<PlayerProgressionDirector>();

            bool passed = true;

            try
            {
                director.SetCurrentLevelForValidation(3);

                if (!director.TryCompleteCurrentLevel(out LevelProgressionResult completionResult)
                    || !completionResult.Success
                    || completionResult.PreviousLevel != 3)
                {
                    AppendLine(reportBuilder, "[FAIL] TryCompleteCurrentLevel did not complete level 3.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] TryCompleteCurrentLevel marks the current level complete.");
                }

                if (!director.TryAdvanceToNextLevel(out LevelProgressionResult advanceResult)
                    || !advanceResult.Success
                    || advanceResult.PreviousLevel != 3
                    || advanceResult.NewLevel != 4
                    || director.CurrentLevel != 4)
                {
                    AppendLine(reportBuilder, "[FAIL] TryAdvanceToNextLevel did not advance from level 3 to 4.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] TryAdvanceToNextLevel advances linearly and persists current level.");
                }

                director.SetCurrentLevelForValidation(LevelProgressData.MaxLevel);
                if (director.TryAdvanceToNextLevel(out LevelProgressionResult maxResult)
                    || !maxResult.IsMaxLevelReached)
                {
                    AppendLine(reportBuilder, "[FAIL] TryAdvanceToNextLevel allowed advancement beyond max level.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] TryAdvanceToNextLevel blocks advancement at max level.");
                }
            }
            finally
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(validationObject);
                }
                else
                {
                    Object.DestroyImmediate(validationObject);
                }
            }

            return passed;
        }

        private static bool ValidateTypeExists(System.Type type, StringBuilder reportBuilder)
        {
            if (type == null)
            {
                AppendLine(reportBuilder, "[FAIL] Required progression type is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] " + type.Name + " type is present.");
            return true;
        }

        private static bool ValidateEventExists(
            System.Type eventOwner,
            string eventName,
            StringBuilder reportBuilder)
        {
            FieldInfo eventBackingField = eventOwner.GetField(
                eventName,
                BindingFlags.Public | BindingFlags.Static);

            if (eventBackingField == null)
            {
                AppendLine(reportBuilder, "[FAIL] Event " + eventName + " is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Event " + eventName + " is present.");
            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
