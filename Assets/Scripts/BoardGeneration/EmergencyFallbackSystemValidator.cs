using System.Text;

namespace MahjongGame.BoardGeneration
{
    public static class EmergencyFallbackSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateFallbackRecipeShape(reportBuilder);
            passed &= ValidateFallbackBoardGeneration(reportBuilder);
            passed &= ValidateLaunchLevelsProduceValidatedBoards(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Emergency fallback system validation completed successfully."
                : "[FAIL] Emergency fallback system validation found issues.");

            return passed;
        }

        private static bool ValidateFallbackRecipeShape(StringBuilder reportBuilder)
        {
            LevelRecipe baseRecipe = LevelRecipeDefinition.GenerateRecipe(12);
            LevelRecipe fallbackRecipe = EmergencyFallbackRecipeDefinition.CreateFallbackRecipe(baseRecipe);

            if (fallbackRecipe.ArchetypeId != BoardArchetypeId.Diamond
                || fallbackRecipe.VariationIndex != 0
                || fallbackRecipe.HolePatternId != HolePatternId.SingleCenter
                || fallbackRecipe.LayerDepth != EmergencyFallbackRecipeDefinition.FallbackLayerDepth
                || fallbackRecipe.ClosedTileCount != 0
                || fallbackRecipe.JokerCount != 0
                || fallbackRecipe.TileCount < BoardQualityChecker.MinimumTileCount
                || fallbackRecipe.TileCount % 2 != 0)
            {
                AppendLine(reportBuilder, "[FAIL] Emergency fallback recipe shape is invalid.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Emergency fallback recipe shape is valid.");
            return true;
        }

        private static bool ValidateFallbackBoardGeneration(StringBuilder reportBuilder)
        {
            LevelRecipe baseRecipe = LevelRecipeDefinition.GenerateRecipe(8);
            BoardData fallbackBoard = EmergencyFallbackRecipeDefinition.GenerateFallbackBoardData(baseRecipe);

            if (fallbackBoard == null
                || !fallbackBoard.IsValidated
                || fallbackBoard.TileCount <= 0)
            {
                AppendLine(reportBuilder, "[FAIL] Emergency fallback board generation failed.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Emergency fallback board generation succeeded.");
            return true;
        }

        private static bool ValidateLaunchLevelsProduceValidatedBoards(StringBuilder reportBuilder)
        {
            bool passed = true;

            for (int levelNumber = 1; levelNumber <= 10; levelNumber++)
            {
                BoardData boardData = BoardGenerationPipeline.GenerateBoardData(levelNumber);
                if (!boardData.IsValidated || boardData.TileCount <= 0)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] Level "
                        + levelNumber
                        + " did not produce a validated board after regeneration/fallback.");
                    passed = false;
                    continue;
                }

                AppendLine(
                    reportBuilder,
                    "[PASS] Level "
                    + levelNumber
                    + " produced a validated board.");
            }

            return passed;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
