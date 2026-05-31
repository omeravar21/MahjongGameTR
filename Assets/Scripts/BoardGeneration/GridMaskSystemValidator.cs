using System.Text;
using MahjongGame.Board;
using MahjongGame.Progression;

namespace MahjongGame.BoardGeneration
{
    public static class GridMaskSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateBaseGridDimensions(reportBuilder);
            passed &= ValidateFullBaseGridMask(reportBuilder);
            passed &= ValidateDeterministicMask(reportBuilder);
            passed &= ValidateRecipeIntegration(reportBuilder);
            passed &= ValidateOccupancyDefinition(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Grid mask system validation completed successfully."
                : "[FAIL] Grid mask system validation found issues.");

            return passed;
        }

        private static bool ValidateBaseGridDimensions(StringBuilder reportBuilder)
        {
            if (BoardGridDefinition.ColumnCount != 6 || BoardGridDefinition.RowCount != 7)
            {
                AppendLine(reportBuilder, "[FAIL] Base grid dimensions are not 6x7.");
                return false;
            }

            if (BoardGridDefinition.TotalCellCount != 42)
            {
                AppendLine(reportBuilder, "[FAIL] Base grid cell count is not 42.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Base grid dimensions are 6x7 with 42 cells.");
            return true;
        }

        private static bool ValidateFullBaseGridMask(StringBuilder reportBuilder)
        {
            GridMask gridMask = GridMaskDefinition.CreateFullBaseGridMask(LevelProgressData.MinLevel, 0);

            if (gridMask.ActiveCellCount != BoardGridDefinition.TotalCellCount)
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] Full base grid mask active cell count is "
                    + gridMask.ActiveCellCount
                    + ", expected "
                    + BoardGridDefinition.TotalCellCount
                    + ".");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Full base grid mask activates all 42 cells.");
            return true;
        }

        private static bool ValidateDeterministicMask(StringBuilder reportBuilder)
        {
            GridMask first = GridMaskDefinition.GenerateFromLevel(17);
            GridMask second = GridMaskDefinition.GenerateFromLevel(17);

            if (first.ActiveCellCount != second.ActiveCellCount
                || first.Seed != second.Seed
                || first.LevelNumber != second.LevelNumber)
            {
                AppendLine(reportBuilder, "[FAIL] Level 17 grid mask is not deterministic.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Grid masks are deterministic per level recipe.");
            return true;
        }

        private static bool ValidateRecipeIntegration(StringBuilder reportBuilder)
        {
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(25);
            GridMask gridMask = GridMaskDefinition.GenerateFromRecipe(recipe);

            if (gridMask.LevelNumber != recipe.LevelNumber || gridMask.Seed != recipe.Seed)
            {
                AppendLine(reportBuilder, "[FAIL] Grid mask does not match level recipe identity.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Grid mask integrates with level recipe inputs.");
            return true;
        }

        private static bool ValidateOccupancyDefinition(StringBuilder reportBuilder)
        {
            GridMask gridMask = GridMaskDefinition.GenerateFromLevel(1);

            if (gridMask.IsCellEligible(-1, 0) || gridMask.IsCellEligible(0, -1))
            {
                AppendLine(reportBuilder, "[FAIL] Out-of-bounds coordinates were marked eligible.");
                return false;
            }

            if (gridMask.IsCellEligible(BoardGridDefinition.ColumnCount, 0)
                || gridMask.IsCellEligible(0, BoardGridDefinition.RowCount))
            {
                AppendLine(reportBuilder, "[FAIL] Out-of-bounds coordinates were marked eligible.");
                return false;
            }

            if (!gridMask.IsCellEligible(0, 0) || !gridMask.IsCellEligible(5, 6))
            {
                AppendLine(reportBuilder, "[FAIL] Valid corner cells are not eligible in base mask.");
                return false;
            }

            if (gridMask.GetCellOccupancy(0, 0) != GridCellOccupancy.Eligible)
            {
                AppendLine(reportBuilder, "[FAIL] Occupancy definition does not mark valid cells as eligible.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Occupancy definition handles valid and invalid coordinates correctly.");
            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
