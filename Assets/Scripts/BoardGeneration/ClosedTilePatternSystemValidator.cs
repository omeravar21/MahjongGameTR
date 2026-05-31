using System.Text;

namespace MahjongGame.BoardGeneration
{
    public static class ClosedTilePatternSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateLaunchPatterns(reportBuilder);
            passed &= ValidateDeterministicLayout(reportBuilder);
            passed &= ValidateInactiveLevels(reportBuilder);
            passed &= ValidatePipelineIntegration(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Closed tile pattern system validation completed successfully."
                : "[FAIL] Closed tile pattern system validation found issues.");

            return passed;
        }

        private static bool ValidateLaunchPatterns(StringBuilder reportBuilder)
        {
            bool passed = true;
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(18);

            for (int patternIndex = 0;
                patternIndex < VisualVarietyDefinition.LaunchClosedTilePatternCount;
                patternIndex++)
            {
                ClosedTilePatternId patternId = (ClosedTilePatternId)patternIndex;
                DistributedBoardLayout distributedLayout = TilePairDistributor.DistributeFromLevel(18);
                LevelRecipe patternRecipe = new LevelRecipe(
                    recipe.LevelNumber,
                    recipe.Seed,
                    recipe.TileCount,
                    recipe.LayerDepth,
                    recipe.ArchetypeId,
                    recipe.VariationIndex,
                    recipe.HolePatternId,
                    recipe.ClosedTileCount,
                    patternId,
                    recipe.JokerCount,
                    recipe.RewardJokerPatternId,
                    recipe.RecommendedTimerSeconds,
                    recipe.DifficultyRating,
                    recipe.MaxRegenerationAttempts);

                ClosedBoardLayout layout = ClosedTilePatternSelector.Apply(distributedLayout, patternRecipe);
                int closedAssignmentCount = CountClosedAssignments(layout.Assignments);

                if (layout.ClosedTilePatternId != patternId
                    || layout.AppliedClosedTileCount != closedAssignmentCount
                    || closedAssignmentCount != recipe.ClosedTileCount
                    || closedAssignmentCount <= 0)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] Closed tile pattern "
                        + patternId
                        + " did not apply the expected closed tile count.");
                    passed = false;
                    continue;
                }

                AppendLine(
                    reportBuilder,
                    "[PASS] Closed tile pattern "
                    + patternId
                    + " applied "
                    + closedAssignmentCount
                    + " closed tiles.");
            }

            return passed;
        }

        private static bool ValidateDeterministicLayout(StringBuilder reportBuilder)
        {
            ClosedBoardLayout first = ClosedTilePatternSelector.ApplyFromLevel(25);
            ClosedBoardLayout second = ClosedTilePatternSelector.ApplyFromLevel(25);

            if (first.AppliedClosedTileCount != second.AppliedClosedTileCount
                || first.ClosedTilePatternId != second.ClosedTilePatternId
                || first.Assignments.Count != second.Assignments.Count)
            {
                AppendLine(reportBuilder, "[FAIL] Closed tile pattern selection is not deterministic.");
                return false;
            }

            for (int index = 0; index < first.Assignments.Count; index++)
            {
                if (first.Assignments[index].IsClosed != second.Assignments[index].IsClosed)
                {
                    AppendLine(reportBuilder, "[FAIL] Closed tile pattern assignment flags are not deterministic.");
                    return false;
                }
            }

            AppendLine(reportBuilder, "[PASS] Closed tile pattern selection is deterministic.");
            return true;
        }

        private static bool ValidateInactiveLevels(StringBuilder reportBuilder)
        {
            ClosedBoardLayout layout = ClosedTilePatternSelector.ApplyFromLevel(5);
            if (CountClosedAssignments(layout.Assignments) != 0 || layout.AppliedClosedTileCount != 0)
            {
                AppendLine(reportBuilder, "[FAIL] Closed tile patterns were applied before activation level.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Closed tile patterns remain inactive before level 10.");
            return true;
        }

        private static bool ValidatePipelineIntegration(StringBuilder reportBuilder)
        {
            BoardData boardData = BoardGenerationPipeline.GenerateCandidateBoardData(
                LevelRecipeDefinition.GenerateRecipe(18));

            if (boardData.ClosedTileCount <= 0)
            {
                AppendLine(reportBuilder, "[FAIL] Pipeline candidate board for level 18 has no closed tile count.");
                return false;
            }

            int closedAssignmentCount = CountClosedAssignments(boardData.TileAssignments);
            if (closedAssignmentCount != boardData.ClosedTileCount)
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] Pipeline candidate board closed assignments do not match recipe count.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Pipeline candidate board includes closed tile assignments.");
            return true;
        }

        private static int CountClosedAssignments(System.Collections.Generic.IReadOnlyList<TileSymbolAssignment> assignments)
        {
            if (assignments == null)
            {
                return 0;
            }

            int count = 0;
            for (int index = 0; index < assignments.Count; index++)
            {
                if (assignments[index].IsClosed)
                {
                    count++;
                }
            }

            return count;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
