using System.Collections.Generic;
using System.Text;

namespace MahjongGame.BoardGeneration
{
    public static class BoardGenerationPipelineSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateLaunchGeneration(reportBuilder);
            passed &= ValidateDeterministicGeneration(reportBuilder);
            passed &= ValidateMetadataPreservation(reportBuilder);
            passed &= ValidatePairIntegrity(reportBuilder);
            passed &= ValidateValidatedOutput(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] BoardGenerationPipeline system validation completed successfully."
                : "[FAIL] BoardGenerationPipeline system validation found issues.");

            return passed;
        }

        private static bool ValidateLaunchGeneration(StringBuilder reportBuilder)
        {
            BoardData boardData = BoardGenerationPipeline.GenerateBoardData(1);

            if (boardData == null
                || boardData.TileCount <= 0
                || boardData.TileAssignments == null
                || boardData.TileAssignments.Count != boardData.TileCount)
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] BoardGenerationPipeline failed to produce board data for launch validation.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] BoardGenerationPipeline produces non-empty board data.");
            return true;
        }

        private static bool ValidateDeterministicGeneration(StringBuilder reportBuilder)
        {
            BoardData first = BoardGenerationPipeline.GenerateBoardData(25);
            BoardData second = BoardGenerationPipeline.GenerateBoardData(25);

            if (!BoardDataEqual(first, second))
            {
                AppendLine(reportBuilder, "[FAIL] Level 25 board data is not deterministic.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] BoardGenerationPipeline output is deterministic per level.");
            return true;
        }

        private static bool ValidateMetadataPreservation(StringBuilder reportBuilder)
        {
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(20);
            BoardData boardData = BoardGenerationPipeline.GenerateBoardData(recipe.LevelNumber);

            if (boardData.LevelNumber != recipe.LevelNumber
                || boardData.ArchetypeId != recipe.ArchetypeId
                || boardData.VariationIndex != recipe.VariationIndex
                || boardData.HolePatternId != recipe.HolePatternId
                || boardData.ClosedTileCount != recipe.ClosedTileCount
                || boardData.JokerCount != recipe.JokerCount)
            {
                AppendLine(reportBuilder, "[FAIL] BoardGenerationPipeline dropped recipe metadata.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] BoardGenerationPipeline preserved recipe metadata.");
            return true;
        }

        private static bool ValidatePairIntegrity(StringBuilder reportBuilder)
        {
            BoardData boardData = BoardGenerationPipeline.GenerateBoardData(12);

            if (boardData.TileCount % 2 != 0)
            {
                AppendLine(reportBuilder, "[FAIL] BoardGenerationPipeline produced an odd tile count.");
                return false;
            }

            Dictionary<int, int> symbolCounts = new Dictionary<int, int>();
            for (int index = 0; index < boardData.TileAssignments.Count; index++)
            {
                int symbolId = boardData.TileAssignments[index].SymbolId;
                if (!symbolCounts.ContainsKey(symbolId))
                {
                    symbolCounts[symbolId] = 0;
                }

                symbolCounts[symbolId]++;
            }

            foreach (KeyValuePair<int, int> entry in symbolCounts)
            {
                if (entry.Value != 2)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] BoardGenerationPipeline produced invalid symbol pair counts.");
                    return false;
                }
            }

            AppendLine(reportBuilder, "[PASS] BoardGenerationPipeline preserved valid symbol pairs.");
            return true;
        }

        private static bool ValidateValidatedOutput(StringBuilder reportBuilder)
        {
            bool passed = true;

            for (int levelNumber = 1; levelNumber <= 10; levelNumber++)
            {
                BoardData boardData = BoardGenerationPipeline.GenerateBoardData(levelNumber);
                if (!boardData.IsValidated)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] Level "
                        + levelNumber
                        + " board was not validated by BoardGenerationPipeline.");
                    passed = false;
                    continue;
                }

                AppendLine(
                    reportBuilder,
                    "[PASS] Level "
                    + levelNumber
                    + " board is validated by BoardGenerationPipeline.");
            }

            return passed;
        }

        private static bool BoardDataEqual(BoardData left, BoardData right)
        {
            if (left == null || right == null || left.TileAssignments == null || right.TileAssignments == null)
            {
                return false;
            }

            if (left.LevelNumber != right.LevelNumber
                || left.Seed != right.Seed
                || left.ArchetypeId != right.ArchetypeId
                || left.VariationIndex != right.VariationIndex
                || left.HolePatternId != right.HolePatternId
                || left.LayerDepth != right.LayerDepth
                || left.TileCount != right.TileCount
                || left.ClosedTileCount != right.ClosedTileCount
                || left.JokerCount != right.JokerCount
                || left.IsValidated != right.IsValidated
                || left.TileAssignments.Count != right.TileAssignments.Count)
            {
                return false;
            }

            for (int index = 0; index < left.TileAssignments.Count; index++)
            {
                TileSymbolAssignment leftAssignment = left.TileAssignments[index];
                TileSymbolAssignment rightAssignment = right.TileAssignments[index];

                if (leftAssignment.TileId != rightAssignment.TileId
                    || leftAssignment.SymbolId != rightAssignment.SymbolId
                    || !leftAssignment.Position.Equals(rightAssignment.Position))
                {
                    return false;
                }
            }

            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
