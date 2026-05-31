using MahjongGame.Board;
using MahjongGame.Progression;

namespace MahjongGame.BoardGeneration
{
    public static class VariationSelector
    {
        public static VariationLayout Apply(ArchetypeLayout archetypeLayout, LevelRecipe recipe)
        {
            int variationIndex = recipe != null ? recipe.VariationIndex : 0;
            return Apply(archetypeLayout, variationIndex);
        }

        public static VariationLayout Apply(ArchetypeLayout archetypeLayout, int variationIndex)
        {
            if (archetypeLayout == null || archetypeLayout.Mask == null)
            {
                GridMask emptyMask = new GridMask(
                    LevelProgressData.MinLevel,
                    0,
                    new GridCellOccupancy[BoardGridDefinition.TotalCellCount]);

                return new VariationLayout(
                    BoardArchetypeId.Diamond,
                    VariationPatternDefinition.ClampVariationIndex(variationIndex),
                    LevelProgressData.MinLevel,
                    0,
                    emptyMask);
            }

            bool[] archetypePattern = ExtractPattern(archetypeLayout.Mask);
            int clampedVariationIndex = VariationPatternDefinition.ClampVariationIndex(variationIndex);
            bool[] variationPattern = VariationPatternDefinition.ApplyVariation(
                archetypePattern,
                clampedVariationIndex,
                archetypeLayout.Seed);

            GridMask variationMask = CreateVariationMask(archetypeLayout, variationPattern);
            return new VariationLayout(
                archetypeLayout.ArchetypeId,
                clampedVariationIndex,
                archetypeLayout.LevelNumber,
                archetypeLayout.Seed,
                variationMask);
        }

        public static VariationLayout ApplyFromLevel(int levelNumber)
        {
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(levelNumber);
            GridMask baseMask = GridMaskDefinition.GenerateFromRecipe(recipe);
            ArchetypeLayout archetypeLayout = ArchetypeSelector.Apply(baseMask, recipe);
            return Apply(archetypeLayout, recipe);
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

        private static GridMask CreateVariationMask(ArchetypeLayout archetypeLayout, bool[] variationPattern)
        {
            GridCellOccupancy[] occupancy = new GridCellOccupancy[BoardGridDefinition.TotalCellCount];

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    int index = (row * BoardGridDefinition.ColumnCount) + column;
                    bool isEligible = archetypeLayout.Mask.IsCellEligible(column, row) && variationPattern[index];
                    occupancy[index] = isEligible
                        ? GridCellOccupancy.Eligible
                        : GridCellOccupancy.Disabled;
                }
            }

            return new GridMask(archetypeLayout.LevelNumber, archetypeLayout.Seed, occupancy);
        }
    }
}
