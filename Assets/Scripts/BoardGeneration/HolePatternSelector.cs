using MahjongGame.Board;
using MahjongGame.Progression;

namespace MahjongGame.BoardGeneration
{
    public static class HolePatternSelector
    {
        public static HolePatternLayout Apply(VariationLayout variationLayout, LevelRecipe recipe)
        {
            HolePatternId holePatternId = recipe != null
                ? recipe.HolePatternId
                : HolePatternId.SingleCenter;

            return Apply(variationLayout, holePatternId);
        }

        public static HolePatternLayout Apply(VariationLayout variationLayout, HolePatternId holePatternId)
        {
            if (variationLayout == null || variationLayout.Mask == null)
            {
                GridMask emptyMask = new GridMask(
                    LevelProgressData.MinLevel,
                    0,
                    new GridCellOccupancy[BoardGridDefinition.TotalCellCount]);

                return new HolePatternLayout(
                    HolePatternDefinition.ClampHolePatternId(holePatternId),
                    BoardArchetypeId.Diamond,
                    0,
                    LevelProgressData.MinLevel,
                    0,
                    emptyMask);
            }

            HolePatternId clampedHolePatternId = HolePatternDefinition.ClampHolePatternId(holePatternId);
            bool[] variationPattern = ExtractPattern(variationLayout.Mask);
            bool[] holedPattern = HolePatternDefinition.ApplyHolePattern(
                variationPattern,
                clampedHolePatternId,
                variationLayout.Seed);

            GridMask holePatternMask = CreateHolePatternMask(variationLayout, holedPattern);
            return new HolePatternLayout(
                clampedHolePatternId,
                variationLayout.ArchetypeId,
                variationLayout.VariationIndex,
                variationLayout.LevelNumber,
                variationLayout.Seed,
                holePatternMask);
        }

        public static HolePatternLayout ApplyFromLevel(int levelNumber)
        {
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(levelNumber);
            GridMask baseMask = GridMaskDefinition.GenerateFromRecipe(recipe);
            ArchetypeLayout archetypeLayout = ArchetypeSelector.Apply(baseMask, recipe);
            VariationLayout variationLayout = VariationSelector.Apply(archetypeLayout, recipe);
            return Apply(variationLayout, recipe);
        }

        private static bool[] ExtractPattern(GridMask mask)
        {
            bool[] pattern = new bool[BoardGridDefinition.TotalCellCount];

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    int index = (row * BoardGridDefinition.ColumnCount) + column;
                    pattern[index] = mask.IsCellEligible(column, row);
                }
            }

            return pattern;
        }

        private static GridMask CreateHolePatternMask(VariationLayout variationLayout, bool[] holedPattern)
        {
            GridCellOccupancy[] occupancy = new GridCellOccupancy[BoardGridDefinition.TotalCellCount];

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    int index = (row * BoardGridDefinition.ColumnCount) + column;
                    bool isEligible = variationLayout.Mask.IsCellEligible(column, row) && holedPattern[index];
                    occupancy[index] = isEligible
                        ? GridCellOccupancy.Eligible
                        : GridCellOccupancy.Disabled;
                }
            }

            return new GridMask(variationLayout.LevelNumber, variationLayout.Seed, occupancy);
        }
    }
}
