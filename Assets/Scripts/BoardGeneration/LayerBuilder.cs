using System.Collections.Generic;
using MahjongGame.Board;
using MahjongGame.Progression;
using MahjongGame.Tiles;

namespace MahjongGame.BoardGeneration
{
    public static class LayerBuilder
    {
        public static LayeredBoardLayout Build(HolePatternLayout holePatternLayout, LevelRecipe recipe)
        {
            int requestedTileCount = recipe != null ? recipe.TileCount : 0;
            int layerDepth = recipe != null ? recipe.LayerDepth : 1;
            return Build(holePatternLayout, requestedTileCount, layerDepth);
        }

        public static LayeredBoardLayout Build(
            HolePatternLayout holePatternLayout,
            int tileCount,
            int layerDepth)
        {
            if (holePatternLayout == null || holePatternLayout.Mask == null)
            {
                return CreateEmptyLayout(tileCount, layerDepth);
            }

            int clampedLayerDepth = LayerBuildDefinition.ClampLayerDepth(layerDepth);
            int availableSlotCount = LayerBuildDefinition.CountAvailableSlots(
                holePatternLayout.Mask,
                clampedLayerDepth);

            IReadOnlyList<TileBoardPosition> positions = LayerBuildDefinition.BuildPositions(
                holePatternLayout.Mask,
                tileCount,
                clampedLayerDepth,
                holePatternLayout.Seed);

            return new LayeredBoardLayout(
                holePatternLayout.HolePatternId,
                holePatternLayout.ArchetypeId,
                holePatternLayout.VariationIndex,
                holePatternLayout.LevelNumber,
                holePatternLayout.Seed,
                clampedLayerDepth,
                tileCount,
                positions.Count,
                availableSlotCount,
                positions);
        }

        public static LayeredBoardLayout BuildFromLevel(int levelNumber)
        {
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(levelNumber);
            GridMask baseMask = GridMaskDefinition.GenerateFromRecipe(recipe);
            ArchetypeLayout archetypeLayout = ArchetypeSelector.Apply(baseMask, recipe);
            VariationLayout variationLayout = VariationSelector.Apply(archetypeLayout, recipe);
            HolePatternLayout holePatternLayout = HolePatternSelector.Apply(variationLayout, recipe);
            return Build(holePatternLayout, recipe);
        }

        private static LayeredBoardLayout CreateEmptyLayout(int tileCount, int layerDepth)
        {
            return new LayeredBoardLayout(
                HolePatternId.SingleCenter,
                BoardArchetypeId.Diamond,
                0,
                LevelProgressData.MinLevel,
                0,
                LayerBuildDefinition.ClampLayerDepth(layerDepth),
                tileCount,
                0,
                0,
                new TileBoardPosition[0]);
        }
    }
}
