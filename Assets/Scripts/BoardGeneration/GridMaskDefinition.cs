using MahjongGame.Board;
using MahjongGame.Progression;

namespace MahjongGame.BoardGeneration
{
    public static class GridMaskDefinition
    {
        public static GridMask GenerateFromRecipe(LevelRecipe recipe)
        {
            if (recipe == null)
            {
                return CreateFullBaseGridMask(LevelProgressData.MinLevel, 0);
            }

            return CreateFullBaseGridMask(recipe.LevelNumber, recipe.Seed);
        }

        public static GridMask GenerateFromLevel(int levelNumber)
        {
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(levelNumber);
            return GenerateFromRecipe(recipe);
        }

        public static GridMask CreateFullBaseGridMask(int levelNumber, int seed)
        {
            int clampedLevel = LevelProgressData.ClampLevel(levelNumber);
            GridCellOccupancy[] occupancy = CreateFullEligibleOccupancy();
            return new GridMask(clampedLevel, seed, occupancy);
        }

        public static GridCellOccupancy[] CreateFullEligibleOccupancy()
        {
            GridCellOccupancy[] occupancy = new GridCellOccupancy[BoardGridDefinition.TotalCellCount];

            for (int index = 0; index < occupancy.Length; index++)
            {
                occupancy[index] = GridCellOccupancy.Eligible;
            }

            return occupancy;
        }

        public static bool IsValidOccupancyMap(GridCellOccupancy[] occupancy)
        {
            if (occupancy == null || occupancy.Length != BoardGridDefinition.TotalCellCount)
            {
                return false;
            }

            for (int index = 0; index < occupancy.Length; index++)
            {
                GridCellOccupancy state = occupancy[index];
                if (state != GridCellOccupancy.Disabled && state != GridCellOccupancy.Eligible)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
