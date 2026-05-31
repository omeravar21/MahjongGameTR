using System.Collections.Generic;
using MahjongGame.Progression;

namespace MahjongGame.BoardGeneration
{
    public static class TilePairDistributor
    {
        public static DistributedBoardLayout Distribute(LayeredBoardLayout layeredBoardLayout, LevelRecipe recipe)
        {
            if (layeredBoardLayout == null)
            {
                return CreateEmptyLayout(recipe);
            }

            int levelNumber = recipe != null ? recipe.LevelNumber : layeredBoardLayout.LevelNumber;
            int seed = recipe != null ? recipe.Seed : layeredBoardLayout.Seed;

            IReadOnlyList<TileSymbolAssignment> assignments = TilePairDistributionDefinition.BuildAssignments(
                layeredBoardLayout.Positions,
                levelNumber,
                seed);

            int effectiveTileCount = assignments.Count;
            int pairCount = effectiveTileCount / 2;
            int distinctSymbolCount = TilePairDistributionDefinition.CountDistinctSymbols(assignments);

            return new DistributedBoardLayout(
                layeredBoardLayout.HolePatternId,
                layeredBoardLayout.ArchetypeId,
                layeredBoardLayout.VariationIndex,
                layeredBoardLayout.LevelNumber,
                layeredBoardLayout.Seed,
                layeredBoardLayout.LayerDepth,
                layeredBoardLayout.RequestedTileCount,
                effectiveTileCount,
                pairCount,
                distinctSymbolCount,
                assignments);
        }

        public static DistributedBoardLayout DistributeFromLevel(int levelNumber)
        {
            LayeredBoardLayout layeredBoardLayout = LayerBuilder.BuildFromLevel(levelNumber);
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(levelNumber);
            return Distribute(layeredBoardLayout, recipe);
        }

        private static DistributedBoardLayout CreateEmptyLayout(LevelRecipe recipe)
        {
            int levelNumber = recipe != null ? recipe.LevelNumber : LevelProgressData.MinLevel;
            int seed = recipe != null ? recipe.Seed : 0;

            return new DistributedBoardLayout(
                HolePatternId.SingleCenter,
                BoardArchetypeId.Diamond,
                0,
                levelNumber,
                seed,
                1,
                recipe != null ? recipe.TileCount : 0,
                0,
                0,
                0,
                new TileSymbolAssignment[0]);
        }
    }
}
