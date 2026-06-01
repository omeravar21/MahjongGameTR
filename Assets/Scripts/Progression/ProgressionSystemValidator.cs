using System.Reflection;
using System.Text;
using MahjongGame.BoardGeneration;
using MahjongGame.Core;
using MahjongGame.Core.Save;
using UnityEngine;

namespace MahjongGame.Progression
{
    public static class ProgressionSystemValidator
    {
        private static readonly int[] LongTermLevelCheckpoints = { 1, 5, 10, 25 };
        private static readonly int[] DifficultySampleLevels = { 1, 5, 10, 25, 100, 1000, LevelProgressData.MaxLevel };
        private static readonly (int Previous, int Next)[] DifficultyAdjacentPairs =
        {
            (1, 2),
            (9, 10),
            (24, 25),
            (49, 50),
        };

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
            passed &= ValidateLongTermLevelFlow(reportBuilder);
            passed &= ValidateLongTermDifficultyFlow(reportBuilder);
            passed &= ValidateLongTermSaveFlow(reportBuilder);

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
                DestroyValidationObject(validationObject);
            }

            return passed;
        }

        private static bool ValidateLongTermLevelFlow(StringBuilder reportBuilder)
        {
            GameObject validationObject = new GameObject("ProgressionSystemValidator_LevelFlow");
            PlayerProgressionDirector director = validationObject.AddComponent<PlayerProgressionDirector>();

            bool passed = true;

            try
            {
                director.SetCurrentLevelForValidation(1);
                passed &= ValidateLevelFlowCheckpoint(director, 1, reportBuilder);

                const int targetLevel = 25;
                for (int currentLevel = 1; currentLevel < targetLevel; currentLevel++)
                {
                    if (!director.TryCompleteCurrentLevel(out LevelProgressionResult completionResult)
                        || !completionResult.Success)
                    {
                        AppendLine(
                            reportBuilder,
                            "[FAIL] Long-term level flow failed to complete level " + currentLevel + ".");
                        return false;
                    }

                    if (!director.TryAdvanceToNextLevel(out LevelProgressionResult advanceResult)
                        || !advanceResult.Success)
                    {
                        AppendLine(
                            reportBuilder,
                            "[FAIL] Long-term level flow failed to advance from level " + currentLevel + ".");
                        return false;
                    }

                    int newLevel = director.CurrentLevel;
                    if (IsLongTermLevelCheckpoint(newLevel))
                    {
                        passed &= ValidateLevelFlowCheckpoint(director, newLevel, reportBuilder);
                    }
                }

                director.SetCurrentLevelForValidation(LevelProgressData.MaxLevel);
                if (director.TryAdvanceToNextLevel(out LevelProgressionResult maxResult)
                    || !maxResult.IsMaxLevelReached)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] Long-term level flow allowed advancement beyond max level.");
                    passed = false;
                }
                else
                {
                    AppendLine(
                        reportBuilder,
                        "[PASS] Long-term level flow blocks advancement at max level.");
                }
            }
            finally
            {
                DestroyValidationObject(validationObject);
            }

            if (passed)
            {
                AppendLine(reportBuilder, "[PASS] Long-term level flow remains stable through sample checkpoints.");
            }

            return passed;
        }

        private static bool ValidateLongTermDifficultyFlow(StringBuilder reportBuilder)
        {
            bool passed = true;

            DifficultyProfile levelNineProfile = DifficultyDefinition.ResolveProfile(9);
            DifficultyProfile levelTenProfile = DifficultyDefinition.ResolveProfile(10);

            if (levelNineProfile.ClosedTileCount != 0)
            {
                AppendLine(reportBuilder, "[FAIL] Difficulty flow keeps closed tiles active before level 10.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Difficulty flow keeps closed tiles inactive before level 10.");
            }

            if (levelTenProfile.ClosedTileCount <= 0)
            {
                AppendLine(reportBuilder, "[FAIL] Difficulty flow does not activate closed tiles at level 10.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Difficulty flow activates closed tiles at level 10.");
            }

            for (int index = 0; index < DifficultySampleLevels.Length - 1; index++)
            {
                int previousLevel = DifficultySampleLevels[index];
                int nextLevel = DifficultySampleLevels[index + 1];
                DifficultyProfile previousProfile = DifficultyDefinition.ResolveProfile(previousLevel);
                DifficultyProfile nextProfile = DifficultyDefinition.ResolveProfile(nextLevel);

                if (!DifficultyDirector.HasDifficultyScaled(previousProfile, nextProfile))
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] Difficulty flow regressed between levels "
                        + previousLevel
                        + " and "
                        + nextLevel
                        + ".");
                    passed = false;
                }
            }

            if (passed)
            {
                AppendLine(
                    reportBuilder,
                    "[PASS] Difficulty flow scales monotonically across long-term sample levels.");
            }

            foreach ((int previousLevel, int nextLevel) in DifficultyAdjacentPairs)
            {
                DifficultyProfile previousProfile = DifficultyDefinition.ResolveProfile(previousLevel);
                DifficultyProfile nextProfile = DifficultyDefinition.ResolveProfile(nextLevel);

                if (!DifficultyDirector.HasDifficultyScaled(previousProfile, nextProfile))
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] Difficulty flow regressed between adjacent levels "
                        + previousLevel
                        + " and "
                        + nextLevel
                        + ".");
                    passed = false;
                }
            }

            if (passed)
            {
                AppendLine(
                    reportBuilder,
                    "[PASS] Difficulty flow scales monotonically across adjacent progression transitions.");
            }

            return passed;
        }

        private static bool ValidateLongTermSaveFlow(StringBuilder reportBuilder)
        {
            bool passed = true;

            PlayerProgressData progressData = new PlayerProgressData();
            progressData.SetCurrentLevel(25);
            progressData.SetGlobalPerformanceScore(12_345);

            PlayerSaveData saveData = PlayerSaveData.CreateDefault();
            progressData.WriteToSave(saveData);
            PlayerProgressData restoredProgressData = PlayerProgressData.FromSave(saveData);

            if (restoredProgressData.CurrentLevel != 25
                || restoredProgressData.HighestLevel != 25
                || restoredProgressData.GlobalPerformanceScore != 12_345)
            {
                AppendLine(reportBuilder, "[FAIL] Progression save data round-trip lost level or score values.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Progression save data round-trip preserves level and score.");
            }

            PlayerProgressData advancedProgressData = new PlayerProgressData();
            advancedProgressData.SetCurrentLevel(10);
            advancedProgressData.MarkCurrentLevelCompleted();
            advancedProgressData.SetCurrentLevel(11);

            PlayerSaveData advancedSaveData = PlayerSaveData.CreateDefault();
            advancedProgressData.WriteToSave(advancedSaveData);
            PlayerProgressData reloadedAdvancedProgress = PlayerProgressData.FromSave(advancedSaveData);

            if (reloadedAdvancedProgress.CurrentLevel != 11
                || reloadedAdvancedProgress.HighestLevel != 11)
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] Progression save flow did not persist highest level after simulated advance.");
                passed = false;
            }
            else
            {
                AppendLine(
                    reportBuilder,
                    "[PASS] Progression save flow persists highest level after simulated advance.");
            }

            if (SaveSystem.HasInstance || PlayerProgressionDirector.HasInstance)
            {
                AppendLine(
                    reportBuilder,
                    "[SKIP] SaveSystem director persistence requires an isolated editor session.");
            }
            else
            {
                passed &= ValidateSaveSystemDirectorPersistence(reportBuilder);
            }

            if (passed)
            {
                AppendLine(reportBuilder, "[PASS] Long-term progression save flow remains stable.");
            }

            return passed;
        }

        private static bool ValidateSaveSystemDirectorPersistence(StringBuilder reportBuilder)
        {
            GameObject validationObject = new GameObject("ProgressionSystemValidator_SaveFlow");
            SaveSystem saveSystem = validationObject.AddComponent<SaveSystem>();
            PlayerProgressionDirector director = validationObject.AddComponent<PlayerProgressionDirector>();

            bool passed = true;

            try
            {
                saveSystem.ResetToDefaults();
                director.SetCurrentLevel(25);

                if (director.CurrentLevel != 25 || director.HighestLevel != 25)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] SaveSystem director persistence did not apply the requested level.");
                    return false;
                }

                director.LoadFromSave();

                if (director.CurrentLevel != 25 || director.HighestLevel != 25)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] SaveSystem director persistence did not reload persisted level data.");
                    passed = false;
                }
                else
                {
                    AppendLine(
                        reportBuilder,
                        "[PASS] SaveSystem director persistence reloads persisted progression data.");
                }
            }
            finally
            {
                DestroyValidationObject(validationObject);
            }

            return passed;
        }

        private static bool ValidateLevelFlowCheckpoint(
            PlayerProgressionDirector director,
            int expectedLevel,
            StringBuilder reportBuilder)
        {
            if (director.CurrentLevel != expectedLevel
                || director.HighestLevel != expectedLevel
                || director.GetCurrentLevelData().levelNumber != expectedLevel)
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] Long-term level flow checkpoint "
                    + expectedLevel
                    + " has inconsistent progression state.");
                return false;
            }

            AppendLine(
                reportBuilder,
                "[PASS] Long-term level flow checkpoint " + expectedLevel + " is consistent.");
            return true;
        }

        private static bool IsLongTermLevelCheckpoint(int levelNumber)
        {
            for (int index = 0; index < LongTermLevelCheckpoints.Length; index++)
            {
                if (LongTermLevelCheckpoints[index] == levelNumber)
                {
                    return true;
                }
            }

            return false;
        }

        private static void DestroyValidationObject(GameObject validationObject)
        {
            if (validationObject == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(validationObject);
            }
            else
            {
                Object.DestroyImmediate(validationObject);
            }
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
