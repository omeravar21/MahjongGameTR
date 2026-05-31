using MahjongGame.Progression;

namespace MahjongGame.BoardGeneration
{
    public static class BoardGenerationPipeline
    {
        public static BoardData GenerateBoardData(int levelNumber)
        {
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(levelNumber);
            GridMask baseMask = GridMaskDefinition.GenerateFromRecipe(recipe);
            ArchetypeLayout archetypeLayout = ArchetypeSelector.Apply(baseMask, recipe);
            VariationLayout variationLayout = VariationSelector.Apply(archetypeLayout, recipe);
            HolePatternLayout holePatternLayout = HolePatternSelector.Apply(variationLayout, recipe);
            LayeredBoardLayout layeredBoardLayout = LayerBuilder.Build(holePatternLayout, recipe);
            DistributedBoardLayout distributedBoardLayout = TilePairDistributor.Distribute(
                layeredBoardLayout,
                recipe);

            return CreateBoardData(recipe, distributedBoardLayout);
        }

        private static BoardData CreateBoardData(LevelRecipe recipe, DistributedBoardLayout distributedBoardLayout)
        {
            if (recipe == null || distributedBoardLayout == null)
            {
                return new BoardData(
                    LevelProgressData.MinLevel,
                    0,
                    BoardArchetypeId.Diamond,
                    0,
                    HolePatternId.SingleCenter,
                    1,
                    0,
                    0,
                    0,
                    false,
                    new TileSymbolAssignment[0]);
            }

            return new BoardData(
                recipe.LevelNumber,
                recipe.Seed,
                distributedBoardLayout.ArchetypeId,
                distributedBoardLayout.VariationIndex,
                distributedBoardLayout.HolePatternId,
                distributedBoardLayout.LayerDepth,
                distributedBoardLayout.EffectiveTileCount,
                recipe.ClosedTileCount,
                recipe.JokerCount,
                false,
                distributedBoardLayout.Assignments);
        }
    }
}
