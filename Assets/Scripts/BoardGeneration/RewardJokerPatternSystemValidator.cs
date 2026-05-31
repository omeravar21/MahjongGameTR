using System.Text;

namespace MahjongGame.BoardGeneration
{
    public static class RewardJokerPatternSystemValidator
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
            passed &= ValidateMutualExclusion(reportBuilder);
            passed &= ValidatePipelineIntegration(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Reward joker pattern system validation completed successfully."
                : "[FAIL] Reward joker pattern system validation found issues.");

            return passed;
        }

        private static bool ValidateLaunchPatterns(StringBuilder reportBuilder)
        {
            bool passed = true;
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(1);

            for (int patternIndex = 0;
                patternIndex < LevelRecipeDefinition.LaunchRewardJokerPatternCount;
                patternIndex++)
            {
                RewardJokerPatternId patternId = (RewardJokerPatternId)patternIndex;
                DistributedBoardLayout distributedLayout = TilePairDistributor.DistributeFromLevel(1);
                ClosedBoardLayout closedLayout = ClosedTilePatternSelector.Apply(distributedLayout, recipe);
                LevelRecipe patternRecipe = new LevelRecipe(
                    recipe.LevelNumber,
                    recipe.Seed,
                    recipe.TileCount,
                    recipe.LayerDepth,
                    recipe.ArchetypeId,
                    recipe.VariationIndex,
                    recipe.HolePatternId,
                    recipe.ClosedTileCount,
                    recipe.ClosedTilePatternId,
                    recipe.JokerCount,
                    patternId,
                    recipe.RecommendedTimerSeconds,
                    recipe.DifficultyRating,
                    recipe.MaxRegenerationAttempts);

                JokerBoardLayout layout = RewardJokerPatternSelector.Apply(closedLayout, patternRecipe);
                int jokerAssignmentCount = CountJokerAssignments(layout.Assignments);

                if (layout.RewardJokerPatternId != patternId
                    || layout.AppliedJokerCount != jokerAssignmentCount
                    || jokerAssignmentCount != recipe.JokerCount
                    || jokerAssignmentCount <= 0)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] Reward joker pattern "
                        + patternId
                        + " did not apply the expected joker count.");
                    passed = false;
                    continue;
                }

                AppendLine(
                    reportBuilder,
                    "[PASS] Reward joker pattern "
                    + patternId
                    + " applied "
                    + jokerAssignmentCount
                    + " joker tiles.");
            }

            return passed;
        }

        private static bool ValidateDeterministicLayout(StringBuilder reportBuilder)
        {
            JokerBoardLayout first = RewardJokerPatternSelector.ApplyFromLevel(25);
            JokerBoardLayout second = RewardJokerPatternSelector.ApplyFromLevel(25);

            if (first.AppliedJokerCount != second.AppliedJokerCount
                || first.RewardJokerPatternId != second.RewardJokerPatternId
                || first.Assignments.Count != second.Assignments.Count)
            {
                AppendLine(reportBuilder, "[FAIL] Reward joker pattern selection is not deterministic.");
                return false;
            }

            for (int index = 0; index < first.Assignments.Count; index++)
            {
                if (first.Assignments[index].IsJoker != second.Assignments[index].IsJoker)
                {
                    AppendLine(reportBuilder, "[FAIL] Reward joker assignment flags are not deterministic.");
                    return false;
                }
            }

            AppendLine(reportBuilder, "[PASS] Reward joker pattern selection is deterministic.");
            return true;
        }

        private static bool ValidateMutualExclusion(StringBuilder reportBuilder)
        {
            JokerBoardLayout layout = RewardJokerPatternSelector.ApplyFromLevel(18);
            for (int index = 0; index < layout.Assignments.Count; index++)
            {
                TileSymbolAssignment assignment = layout.Assignments[index];
                if (assignment.IsClosed && assignment.IsJoker)
                {
                    AppendLine(reportBuilder, "[FAIL] Closed and joker flags overlap on the same assignment.");
                    return false;
                }
            }

            AppendLine(reportBuilder, "[PASS] Closed and joker flags remain mutually exclusive.");
            return true;
        }

        private static bool ValidatePipelineIntegration(StringBuilder reportBuilder)
        {
            BoardData boardData = BoardGenerationPipeline.GenerateCandidateBoardData(
                LevelRecipeDefinition.GenerateRecipe(1));

            if (boardData.JokerCount <= 0)
            {
                AppendLine(reportBuilder, "[FAIL] Pipeline candidate board for level 1 has no joker count.");
                return false;
            }

            int jokerAssignmentCount = CountJokerAssignments(boardData.TileAssignments);
            if (jokerAssignmentCount != boardData.JokerCount)
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] Pipeline candidate board joker assignments do not match recipe count.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Pipeline candidate board includes joker assignments.");
            return true;
        }

        private static int CountJokerAssignments(System.Collections.Generic.IReadOnlyList<TileSymbolAssignment> assignments)
        {
            if (assignments == null)
            {
                return 0;
            }

            int count = 0;
            for (int index = 0; index < assignments.Count; index++)
            {
                if (assignments[index].IsJoker)
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
