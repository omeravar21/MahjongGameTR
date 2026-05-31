using MahjongGame.Board;
using MahjongGame.Progression;

namespace MahjongGame.BoardGeneration
{
    public static class ArchetypeSelector
    {
        public static ArchetypeLayout Apply(GridMask baseMask, LevelRecipe recipe)
        {
            if (baseMask == null)
            {
                baseMask = GridMaskDefinition.CreateFullBaseGridMask(
                    recipe != null ? recipe.LevelNumber : LevelProgressData.MinLevel,
                    recipe != null ? recipe.Seed : 0);
            }

            BoardArchetypeId archetypeId = recipe != null
                ? recipe.ArchetypeId
                : BoardArchetypeId.Diamond;

            return Apply(baseMask, archetypeId);
        }

        public static ArchetypeLayout Apply(GridMask baseMask, BoardArchetypeId archetypeId)
        {
            bool[] pattern = ArchetypePatternDefinition.GetPattern(archetypeId);
            GridCellOccupancy[] occupancy = CreateOccupancy(baseMask, pattern);
            GridMask filteredMask = new GridMask(baseMask.LevelNumber, baseMask.Seed, occupancy);
            return new ArchetypeLayout(archetypeId, baseMask.LevelNumber, baseMask.Seed, filteredMask);
        }

        public static ArchetypeLayout ApplyFromLevel(int levelNumber)
        {
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(levelNumber);
            GridMask baseMask = GridMaskDefinition.GenerateFromRecipe(recipe);
            return Apply(baseMask, recipe);
        }

        private static GridCellOccupancy[] CreateOccupancy(GridMask baseMask, bool[] pattern)
        {
            GridCellOccupancy[] occupancy = new GridCellOccupancy[BoardGridDefinition.TotalCellCount];

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    int index = (row * BoardGridDefinition.ColumnCount) + column;
                    bool isEligible = baseMask.IsCellEligible(column, row) && pattern[index];
                    occupancy[index] = isEligible
                        ? GridCellOccupancy.Eligible
                        : GridCellOccupancy.Disabled;
                }
            }

            return occupancy;
        }
    }
}
