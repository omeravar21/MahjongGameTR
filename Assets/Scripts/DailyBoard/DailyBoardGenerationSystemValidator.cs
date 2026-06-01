using System;
using System.Text;
using MahjongGame.BoardGeneration;
using MahjongGame.Core.Save;
using UnityEngine;

namespace MahjongGame.DailyBoard
{
    public static class DailyBoardGenerationSystemValidator
    {
        private static readonly DateTime SampleUtcDate = new DateTime(2026, 5, 31, 12, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime NextUtcDate = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateDeterministicRecipeGeneration(reportBuilder);
            passed &= ValidateDeterministicBoardGeneration(reportBuilder);
            passed &= ValidateDifferentDayRecipes(reportBuilder);
            passed &= ValidateBoardQuality(reportBuilder);
            passed &= ValidateProgressionIsolationOnCompletion(reportBuilder);
            passed &= ValidateCompletionMarksLastCompletedDayId(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Daily board generation validation completed successfully."
                : "[FAIL] Daily board generation validation found issues.");

            return passed;
        }

        private static bool ValidateDeterministicRecipeGeneration(StringBuilder reportBuilder)
        {
            int dayId = DailyBoardDefinition.GetUtcDayId(SampleUtcDate);
            int dailySeed = DailyBoardDefinition.ComputeSeed(dayId);

            LevelRecipe first = DailyBoardRecipeDefinition.GenerateRecipe(dayId, dailySeed);
            LevelRecipe second = DailyBoardRecipeDefinition.GenerateRecipe(dayId, dailySeed);

            if (!LevelRecipeEqual(first, second))
            {
                AppendLine(reportBuilder, "[FAIL] Same day id did not produce identical daily recipes.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Same day id produces identical daily recipes.");
            return true;
        }

        private static bool ValidateDeterministicBoardGeneration(StringBuilder reportBuilder)
        {
            int dayId = DailyBoardDefinition.GetUtcDayId(SampleUtcDate);
            int dailySeed = DailyBoardDefinition.ComputeSeed(dayId);
            LevelRecipe recipe = DailyBoardRecipeDefinition.GenerateRecipe(dayId, dailySeed);

            BoardData first = BoardGenerationPipeline.GenerateBoardData(recipe);
            BoardData second = BoardGenerationPipeline.GenerateBoardData(recipe);

            if (first == null
                || second == null
                || first.Seed != second.Seed
                || first.TileCount != second.TileCount
                || !first.IsValidated
                || !second.IsValidated)
            {
                AppendLine(reportBuilder, "[FAIL] Same day id did not produce identical validated board data.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Same day id produces identical validated board seed and tile count.");
            return true;
        }

        private static bool ValidateDifferentDayRecipes(StringBuilder reportBuilder)
        {
            int firstDayId = DailyBoardDefinition.GetUtcDayId(SampleUtcDate);
            int secondDayId = DailyBoardDefinition.GetUtcDayId(NextUtcDate);

            LevelRecipe first = DailyBoardRecipeDefinition.GenerateRecipe(
                firstDayId,
                DailyBoardDefinition.ComputeSeed(firstDayId));
            LevelRecipe second = DailyBoardRecipeDefinition.GenerateRecipe(
                secondDayId,
                DailyBoardDefinition.ComputeSeed(secondDayId));

            if (LevelRecipeEqual(first, second))
            {
                AppendLine(reportBuilder, "[FAIL] Different day ids produced identical daily recipes.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Different day ids produce different daily recipes.");
            return true;
        }

        private static bool ValidateBoardQuality(StringBuilder reportBuilder)
        {
            int dayId = DailyBoardDefinition.GetUtcDayId(SampleUtcDate);
            LevelRecipe recipe = DailyBoardRecipeDefinition.GenerateRecipe(
                dayId,
                DailyBoardDefinition.ComputeSeed(dayId));
            BoardData boardData = BoardGenerationPipeline.GenerateBoardData(recipe);

            if (boardData == null || !boardData.IsValidated)
            {
                AppendLine(reportBuilder, "[FAIL] Daily board generation did not produce validated board data.");
                return false;
            }

            BoardQualityCheckResult qualityResult = BoardQualityChecker.Check(boardData);
            if (!qualityResult.IsValid)
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] Generated daily board failed BoardQualityChecker: "
                    + qualityResult.FailureReason
                    + ".");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Generated daily board passes BoardQualityChecker.");
            return true;
        }

        private static bool ValidateProgressionIsolationOnCompletion(StringBuilder reportBuilder)
        {
            PlayerSaveData saveData = PlayerSaveData.CreateDefault();
            saveData.EnsureDefaults();
            saveData.currentLevel = 42;

            int dayId = DailyBoardDefinition.GetUtcDayId(SampleUtcDate);
            DailyBoardData dailyBoardData = DailyBoardData.CreateDefault();
            dailyBoardData.SetLastCompletedDayId(dayId);
            dailyBoardData.WriteToSave(saveData);

            if (saveData.currentLevel != 42)
            {
                AppendLine(reportBuilder, "[FAIL] Daily board completion modified PlayerSaveData.currentLevel.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Daily board completion does not modify PlayerSaveData.currentLevel.");
            return true;
        }

        private static bool ValidateCompletionMarksLastCompletedDayId(StringBuilder reportBuilder)
        {
            GameObject validationObject = new GameObject("DailyBoardGenerationValidator_Completion");
            DailyBoardDirector director = validationObject.AddComponent<DailyBoardDirector>();

            bool passed = true;

            try
            {
                int dayId = DailyBoardDefinition.GetUtcDayId(SampleUtcDate);
                director.SetStateForValidation(0, SampleUtcDate);

                if (!director.TryMarkCompletedToday())
                {
                    AppendLine(reportBuilder, "[FAIL] Daily board completion did not mark lastCompletedDayId.");
                    passed = false;
                }
                else if (director.LastCompletedDayId != dayId)
                {
                    AppendLine(reportBuilder, "[FAIL] Daily board completion did not persist lastCompletedDayId.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] Daily board completion marks lastCompletedDayId.");
                }
            }
            finally
            {
                director.ResetUtcNowProviderForValidation();
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(validationObject);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(validationObject);
                }
            }

            return passed;
        }

        private static bool LevelRecipeEqual(LevelRecipe left, LevelRecipe right)
        {
            if (left == null || right == null)
            {
                return false;
            }

            return left.LevelNumber == right.LevelNumber
                && left.Seed == right.Seed
                && left.TileCount == right.TileCount
                && left.LayerDepth == right.LayerDepth
                && left.ArchetypeId == right.ArchetypeId
                && left.VariationIndex == right.VariationIndex
                && left.HolePatternId == right.HolePatternId
                && left.ClosedTileCount == right.ClosedTileCount
                && left.ClosedTilePatternId == right.ClosedTilePatternId
                && left.JokerCount == right.JokerCount
                && left.RewardJokerPatternId == right.RewardJokerPatternId
                && Mathf.Approximately(left.RecommendedTimerSeconds, right.RecommendedTimerSeconds)
                && Mathf.Approximately(left.DifficultyRating, right.DifficultyRating)
                && left.MaxRegenerationAttempts == right.MaxRegenerationAttempts;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
