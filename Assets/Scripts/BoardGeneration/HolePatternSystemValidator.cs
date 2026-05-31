using System.Text;
using MahjongGame.Board;

namespace MahjongGame.BoardGeneration
{
    public static class HolePatternSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateLaunchHolePatterns(reportBuilder);
            passed &= ValidateDeterministicLayout(reportBuilder);
            passed &= ValidatePatternIndependence(reportBuilder);
            passed &= ValidateBounds(reportBuilder);
            passed &= ValidateMinimumActiveCellCount(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Hole pattern system validation completed successfully."
                : "[FAIL] Hole pattern system validation found issues.");

            return passed;
        }

        private static bool ValidateLaunchHolePatterns(StringBuilder reportBuilder)
        {
            bool passed = true;
            GridMask baseMask = GridMaskDefinition.CreateFullBaseGridMask(1, 0);

            foreach (BoardArchetypeId archetypeId in System.Enum.GetValues(typeof(BoardArchetypeId)))
            {
                ArchetypeLayout archetypeLayout = ArchetypeSelector.Apply(baseMask, archetypeId);
                VariationLayout variationLayout = VariationSelector.Apply(archetypeLayout, 0);

                for (int holePatternIndex = 0;
                    holePatternIndex < VisualVarietyDefinition.LaunchHolePatternCount;
                    holePatternIndex++)
                {
                    HolePatternId holePatternId = (HolePatternId)holePatternIndex;
                    HolePatternLayout layout = HolePatternSelector.Apply(variationLayout, holePatternId);

                    if (layout.HolePatternId != holePatternId
                        || layout.ArchetypeId != archetypeId
                        || layout.VariationIndex != 0
                        || layout.ActiveCellCount <= 0
                        || layout.Mask == null)
                    {
                        AppendLine(
                            reportBuilder,
                            "[FAIL] Hole pattern "
                            + holePatternId
                            + " failed for archetype "
                            + archetypeId
                            + ".");
                        passed = false;
                        continue;
                    }

                    AppendLine(
                        reportBuilder,
                        "[PASS] Hole pattern "
                        + holePatternId
                        + " layout is valid for archetype "
                        + archetypeId
                        + ".");
                }
            }

            return passed;
        }

        private static bool ValidateDeterministicLayout(StringBuilder reportBuilder)
        {
            HolePatternLayout first = HolePatternSelector.ApplyFromLevel(25);
            HolePatternLayout second = HolePatternSelector.ApplyFromLevel(25);

            if (first.ActiveCellCount != second.ActiveCellCount
                || first.ArchetypeId != second.ArchetypeId
                || first.VariationIndex != second.VariationIndex
                || first.HolePatternId != second.HolePatternId
                || first.Seed != second.Seed)
            {
                AppendLine(reportBuilder, "[FAIL] Level 25 hole pattern layout is not deterministic.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Hole pattern layouts are deterministic per level recipe.");
            return true;
        }

        private static bool ValidatePatternIndependence(StringBuilder reportBuilder)
        {
            bool passed = true;
            bool anyDifferenceFound = false;
            GridMask baseMask = GridMaskDefinition.CreateFullBaseGridMask(1, 0);
            ArchetypeLayout archetypeLayout = ArchetypeSelector.Apply(baseMask, BoardArchetypeId.Diamond);
            VariationLayout variationLayout = VariationSelector.Apply(archetypeLayout, 0);

            HolePatternLayout singleCenterLayout = HolePatternSelector.Apply(
                variationLayout,
                HolePatternId.SingleCenter);
            HolePatternLayout dualCornerLayout = HolePatternSelector.Apply(
                variationLayout,
                HolePatternId.DualCorner);

            if (!LayoutsEqual(singleCenterLayout, dualCornerLayout))
            {
                anyDifferenceFound = true;
            }

            if (!anyDifferenceFound)
            {
                AppendLine(
                    reportBuilder,
                    "[INFO] SingleCenter and DualCorner produced identical layouts at minimum cell floor.");
            }
            else if (dualCornerLayout.ActiveCellCount > singleCenterLayout.ActiveCellCount)
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] DualCorner did not reduce layout density relative to SingleCenter.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Hole pattern id changes layout independently.");
            }

            return passed;
        }

        private static bool ValidateBounds(StringBuilder reportBuilder)
        {
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(12);
            GridMask baseMask = GridMaskDefinition.GenerateFromRecipe(recipe);
            ArchetypeLayout archetypeLayout = ArchetypeSelector.Apply(baseMask, recipe);
            VariationLayout variationLayout = VariationSelector.Apply(archetypeLayout, recipe);
            HolePatternLayout holePatternLayout = HolePatternSelector.Apply(variationLayout, recipe);

            if (holePatternLayout.ActiveCellCount > variationLayout.ActiveCellCount)
            {
                AppendLine(reportBuilder, "[FAIL] Hole pattern layout enabled more cells than the variation layout.");
                return false;
            }

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    if (holePatternLayout.Mask.IsCellEligible(column, row)
                        && !variationLayout.Mask.IsCellEligible(column, row))
                    {
                        AppendLine(
                            reportBuilder,
                            "[FAIL] Hole pattern layout enabled a cell outside the variation silhouette.");
                        return false;
                    }

                    if (holePatternLayout.Mask.IsCellEligible(column, row)
                        && !archetypeLayout.Mask.IsCellEligible(column, row))
                    {
                        AppendLine(
                            reportBuilder,
                            "[FAIL] Hole pattern layout enabled a cell outside the archetype silhouette.");
                        return false;
                    }

                    if (holePatternLayout.Mask.IsCellEligible(column, row)
                        && !baseMask.IsCellEligible(column, row))
                    {
                        AppendLine(
                            reportBuilder,
                            "[FAIL] Hole pattern layout enabled a base-disabled cell.");
                        return false;
                    }
                }
            }

            AppendLine(reportBuilder, "[PASS] Hole pattern layouts stay within variation, archetype, and base mask bounds.");
            return true;
        }

        private static bool ValidateMinimumActiveCellCount(StringBuilder reportBuilder)
        {
            bool passed = true;
            GridMask baseMask = GridMaskDefinition.CreateFullBaseGridMask(1, 0);

            foreach (BoardArchetypeId archetypeId in System.Enum.GetValues(typeof(BoardArchetypeId)))
            {
                ArchetypeLayout archetypeLayout = ArchetypeSelector.Apply(baseMask, archetypeId);

                for (int variationIndex = 0; variationIndex < VisualVarietyDefinition.VariationsPerArchetype; variationIndex++)
                {
                    VariationLayout variationLayout = VariationSelector.Apply(archetypeLayout, variationIndex);

                    for (int holePatternIndex = 0;
                        holePatternIndex < VisualVarietyDefinition.LaunchHolePatternCount;
                        holePatternIndex++)
                    {
                        HolePatternId holePatternId = (HolePatternId)holePatternIndex;
                        HolePatternLayout layout = HolePatternSelector.Apply(variationLayout, holePatternId);

                        if (layout.ActiveCellCount < ArchetypePatternDefinition.MinimumActiveCellCount)
                        {
                            AppendLine(
                                reportBuilder,
                                "[FAIL] Hole pattern "
                                + holePatternId
                                + " for archetype "
                                + archetypeId
                                + " variation "
                                + variationIndex
                                + " dropped below minimum active cell count.");
                            passed = false;
                        }
                    }
                }
            }

            if (passed)
            {
                AppendLine(reportBuilder, "[PASS] All hole pattern layouts respect minimum active cell count.");
            }

            return passed;
        }

        private static bool LayoutsEqual(HolePatternLayout left, HolePatternLayout right)
        {
            if (left == null || right == null || left.Mask == null || right.Mask == null)
            {
                return false;
            }

            if (left.ActiveCellCount != right.ActiveCellCount
                || left.ArchetypeId != right.ArchetypeId
                || left.VariationIndex != right.VariationIndex
                || left.HolePatternId != right.HolePatternId)
            {
                return false;
            }

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    if (left.Mask.IsCellEligible(column, row) != right.Mask.IsCellEligible(column, row))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
