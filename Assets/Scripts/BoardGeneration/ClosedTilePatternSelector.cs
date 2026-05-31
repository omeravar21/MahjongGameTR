using System.Collections.Generic;

namespace MahjongGame.BoardGeneration
{
    public static class ClosedTilePatternSelector
    {
        public static ClosedBoardLayout Apply(DistributedBoardLayout layout, LevelRecipe recipe)
        {
            if (layout == null)
            {
                return ClosedBoardLayout.FromDistributedLayout(
                    null,
                    ClosedTilePatternId.CornerSingle,
                    0,
                    new TileSymbolAssignment[0]);
            }

            if (recipe == null
                || recipe.ClosedTileCount <= 0
                || !MahjongGame.ClosedTiles.ClosedTileDefinition.IsClosedTileMechanicActive(recipe.LevelNumber))
            {
                return ClosedBoardLayout.FromDistributedLayout(
                    layout,
                    recipe != null ? recipe.ClosedTilePatternId : ClosedTilePatternId.CornerSingle,
                    0,
                    layout.Assignments);
            }

            ClosedTilePatternId patternId = ClosedTilePatternDefinition.ClampClosedTilePatternId(recipe.ClosedTilePatternId);
            HashSet<int> closedIndices = ClosedTilePatternDefinition.SelectClosedTileIndices(
                layout.Assignments,
                patternId,
                recipe.ClosedTileCount,
                layout.Seed,
                recipe.LevelNumber);

            TileSymbolAssignment[] updatedAssignments = new TileSymbolAssignment[layout.Assignments.Count];
            for (int index = 0; index < layout.Assignments.Count; index++)
            {
                TileSymbolAssignment assignment = layout.Assignments[index];
                updatedAssignments[index] = assignment.WithClosed(closedIndices.Contains(index));
            }

            return ClosedBoardLayout.FromDistributedLayout(
                layout,
                patternId,
                closedIndices.Count,
                updatedAssignments);
        }

        public static ClosedBoardLayout ApplyFromLevel(int levelNumber)
        {
            DistributedBoardLayout distributedLayout = TilePairDistributor.DistributeFromLevel(levelNumber);
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(levelNumber);
            return Apply(distributedLayout, recipe);
        }
    }
}
