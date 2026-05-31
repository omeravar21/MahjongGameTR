using System.Collections.Generic;

namespace MahjongGame.BoardGeneration
{
    public static class RewardJokerPatternSelector
    {
        public static JokerBoardLayout Apply(ClosedBoardLayout layout, LevelRecipe recipe)
        {
            if (layout == null)
            {
                return JokerBoardLayout.FromClosedBoardLayout(
                    null,
                    RewardJokerPatternId.BalancedSpread,
                    0,
                    new TileSymbolAssignment[0]);
            }

            if (recipe == null || recipe.JokerCount <= 0)
            {
                return JokerBoardLayout.FromClosedBoardLayout(
                    layout,
                    recipe != null ? recipe.RewardJokerPatternId : RewardJokerPatternId.BalancedSpread,
                    0,
                    layout.Assignments);
            }

            RewardJokerPatternId patternId = RewardJokerPatternDefinition.ClampRewardJokerPatternId(
                recipe.RewardJokerPatternId);
            HashSet<int> jokerIndices = RewardJokerPatternDefinition.SelectJokerTileIndices(
                layout.Assignments,
                patternId,
                recipe.JokerCount,
                layout.Seed);

            TileSymbolAssignment[] updatedAssignments = new TileSymbolAssignment[layout.Assignments.Count];
            for (int index = 0; index < layout.Assignments.Count; index++)
            {
                TileSymbolAssignment assignment = layout.Assignments[index];
                updatedAssignments[index] = assignment.WithJoker(jokerIndices.Contains(index));
            }

            return JokerBoardLayout.FromClosedBoardLayout(
                layout,
                patternId,
                jokerIndices.Count,
                updatedAssignments);
        }

        public static JokerBoardLayout ApplyFromLevel(int levelNumber)
        {
            ClosedBoardLayout closedBoardLayout = ClosedTilePatternSelector.ApplyFromLevel(levelNumber);
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(levelNumber);
            return Apply(closedBoardLayout, recipe);
        }
    }
}
