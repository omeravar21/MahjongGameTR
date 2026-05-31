using System.Text;
using MahjongGame.Board;

namespace MahjongGame.BoardGeneration
{
    public static class VariationSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateLaunchVariations(reportBuilder);
            passed &= ValidateDeterministicLayout(reportBuilder);
            passed &= ValidateVariationIndependence(reportBuilder);
            passed &= ValidateArchetypeIndependence(reportBuilder);
            passed &= ValidateBounds(reportBuilder);
            passed &= ValidateMinimumActiveCellCount(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Variation system validation completed successfully."
                : "[FAIL] Variation system validation found issues.");

            return passed;
        }

        private static bool ValidateLaunchVariations(StringBuilder reportBuilder)
        {
            bool passed = true;
            GridMask baseMask = GridMaskDefinition.CreateFullBaseGridMask(1, 0);

            foreach (BoardArchetypeId archetypeId in System.Enum.GetValues(typeof(BoardArchetypeId)))
            {
                ArchetypeLayout archetypeLayout = ArchetypeSelector.Apply(baseMask, archetypeId);

                for (int variationIndex = 0; variationIndex < VisualVarietyDefinition.VariationsPerArchetype; variationIndex++)
                {
                    VariationLayout layout = VariationSelector.Apply(archetypeLayout, variationIndex);

                    if (layout.ArchetypeId != archetypeId
                        || layout.VariationIndex != variationIndex
                        || layout.ActiveCellCount <= 0
                        || layout.Mask == null)
                    {
                        AppendLine(
                            reportBuilder,
                            "[FAIL] Variation "
                            + variationIndex
                            + " failed for archetype "
                            + archetypeId
                            + ".");
                        passed = false;
                        continue;
                    }

                    AppendLine(
                        reportBuilder,
                        "[PASS] Variation "
                        + variationIndex
                        + " layout is valid for archetype "
                        + archetypeId
                        + ".");
                }
            }

            return passed;
        }

        private static bool ValidateDeterministicLayout(StringBuilder reportBuilder)
        {
            VariationLayout first = VariationSelector.ApplyFromLevel(25);
            VariationLayout second = VariationSelector.ApplyFromLevel(25);

            if (first.ActiveCellCount != second.ActiveCellCount
                || first.ArchetypeId != second.ArchetypeId
                || first.VariationIndex != second.VariationIndex
                || first.Seed != second.Seed)
            {
                AppendLine(reportBuilder, "[FAIL] Level 25 variation layout is not deterministic.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Variation layouts are deterministic per level recipe.");
            return true;
        }

        private static bool ValidateVariationIndependence(StringBuilder reportBuilder)
        {
            bool passed = true;
            bool anyDifferenceFound = false;
            GridMask baseMask = GridMaskDefinition.CreateFullBaseGridMask(1, 0);

            foreach (BoardArchetypeId archetypeId in System.Enum.GetValues(typeof(BoardArchetypeId)))
            {
                ArchetypeLayout archetypeLayout = ArchetypeSelector.Apply(baseMask, archetypeId);
                VariationLayout standardLayout = VariationSelector.Apply(archetypeLayout, 0);
                VariationLayout trimmedLayout = VariationSelector.Apply(archetypeLayout, 1);

                if (LayoutsEqual(standardLayout, trimmedLayout))
                {
                    continue;
                }

                anyDifferenceFound = true;

                if (trimmedLayout.ActiveCellCount >= standardLayout.ActiveCellCount)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] Variation 1 did not reduce layout density for archetype "
                        + archetypeId
                        + ".");
                    passed = false;
                }
            }

            if (!anyDifferenceFound)
            {
                AppendLine(reportBuilder, "[INFO] No archetype produced a distinct variation 1 layout at minimum cell floor.");
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Variation index changes layout independently from archetype identity.");
            }

            return passed;
        }

        private static bool ValidateArchetypeIndependence(StringBuilder reportBuilder)
        {
            GridMask baseMask = GridMaskDefinition.CreateFullBaseGridMask(1, 0);
            ArchetypeLayout diamondLayout = ArchetypeSelector.Apply(baseMask, BoardArchetypeId.Diamond);
            ArchetypeLayout ovalLayout = ArchetypeSelector.Apply(baseMask, BoardArchetypeId.Oval);

            VariationLayout diamondVariation = VariationSelector.Apply(diamondLayout, 1);
            VariationLayout ovalVariation = VariationSelector.Apply(ovalLayout, 1);

            if (LayoutsEqual(diamondVariation, ovalVariation))
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] Different archetypes with the same variation index produced identical layouts.");
                return false;
            }

            AppendLine(
                reportBuilder,
                "[PASS] Archetype identity remains independent under the same variation index.");
            return true;
        }

        private static bool ValidateBounds(StringBuilder reportBuilder)
        {
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(12);
            GridMask baseMask = GridMaskDefinition.GenerateFromRecipe(recipe);
            ArchetypeLayout archetypeLayout = ArchetypeSelector.Apply(baseMask, recipe);
            VariationLayout variationLayout = VariationSelector.Apply(archetypeLayout, recipe);

            if (variationLayout.ActiveCellCount > archetypeLayout.ActiveCellCount)
            {
                AppendLine(reportBuilder, "[FAIL] Variation layout enabled more cells than the archetype layout.");
                return false;
            }

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    if (variationLayout.Mask.IsCellEligible(column, row)
                        && !archetypeLayout.Mask.IsCellEligible(column, row))
                    {
                        AppendLine(
                            reportBuilder,
                            "[FAIL] Variation layout enabled a cell outside the archetype silhouette.");
                        return false;
                    }

                    if (variationLayout.Mask.IsCellEligible(column, row)
                        && !baseMask.IsCellEligible(column, row))
                    {
                        AppendLine(
                            reportBuilder,
                            "[FAIL] Variation layout enabled a base-disabled cell.");
                        return false;
                    }
                }
            }

            AppendLine(reportBuilder, "[PASS] Variation layouts stay within archetype and base mask bounds.");
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
                    VariationLayout layout = VariationSelector.Apply(archetypeLayout, variationIndex);
                    if (layout.ActiveCellCount < ArchetypePatternDefinition.MinimumActiveCellCount)
                    {
                        AppendLine(
                            reportBuilder,
                            "[FAIL] Variation "
                            + variationIndex
                            + " for archetype "
                            + archetypeId
                            + " dropped below minimum active cell count.");
                        passed = false;
                    }
                }
            }

            if (passed)
            {
                AppendLine(reportBuilder, "[PASS] All variation layouts respect minimum active cell count.");
            }

            return passed;
        }

        private static bool LayoutsEqual(VariationLayout left, VariationLayout right)
        {
            if (left == null || right == null || left.Mask == null || right.Mask == null)
            {
                return false;
            }

            if (left.ActiveCellCount != right.ActiveCellCount
                || left.ArchetypeId != right.ArchetypeId
                || left.VariationIndex != right.VariationIndex)
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
