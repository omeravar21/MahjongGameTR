using System.Text;
using MahjongGame.Board;

namespace MahjongGame.BoardGeneration
{
    public static class ArchetypeSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateLaunchArchetypes(reportBuilder);
            passed &= ValidateDeterministicLayout(reportBuilder);
            passed &= ValidateBaseMaskIntegration(reportBuilder);
            passed &= ValidateVariationIsolation(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Archetype system validation completed successfully."
                : "[FAIL] Archetype system validation found issues.");

            return passed;
        }

        private static bool ValidateLaunchArchetypes(StringBuilder reportBuilder)
        {
            bool passed = true;

            foreach (BoardArchetypeId archetypeId in System.Enum.GetValues(typeof(BoardArchetypeId)))
            {
                bool[] pattern = ArchetypePatternDefinition.GetPattern(archetypeId);
                int patternCells = ArchetypePatternDefinition.CountActiveCells(pattern);

                if (pattern.Length != BoardGridDefinition.TotalCellCount)
                {
                    AppendLine(reportBuilder, "[FAIL] Archetype " + archetypeId + " pattern size is invalid.");
                    passed = false;
                    continue;
                }

                if (patternCells < ArchetypePatternDefinition.MinimumActiveCellCount)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] Archetype "
                        + archetypeId
                        + " active cell count "
                        + patternCells
                        + " is below minimum "
                        + ArchetypePatternDefinition.MinimumActiveCellCount
                        + ".");
                    passed = false;
                    continue;
                }

                GridMask baseMask = GridMaskDefinition.CreateFullBaseGridMask(1, 0);
                ArchetypeLayout layout = ArchetypeSelector.Apply(baseMask, archetypeId);

                if (layout.ArchetypeId != archetypeId
                    || layout.ActiveCellCount != patternCells
                    || layout.ActiveCellCount < ArchetypePatternDefinition.MinimumActiveCellCount)
                {
                    AppendLine(reportBuilder, "[FAIL] Archetype " + archetypeId + " layout generation failed.");
                    passed = false;
                    continue;
                }

                AppendLine(reportBuilder, "[PASS] Archetype " + archetypeId + " layout is valid.");
            }

            return passed;
        }

        private static bool ValidateDeterministicLayout(StringBuilder reportBuilder)
        {
            ArchetypeLayout first = ArchetypeSelector.ApplyFromLevel(25);
            ArchetypeLayout second = ArchetypeSelector.ApplyFromLevel(25);

            if (first.ActiveCellCount != second.ActiveCellCount
                || first.ArchetypeId != second.ArchetypeId
                || first.Seed != second.Seed)
            {
                AppendLine(reportBuilder, "[FAIL] Level 25 archetype layout is not deterministic.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Archetype layouts are deterministic per level recipe.");
            return true;
        }

        private static bool ValidateBaseMaskIntegration(StringBuilder reportBuilder)
        {
            GridMask baseMask = GridMaskDefinition.CreateFullBaseGridMask(5, 123);
            ArchetypeLayout layout = ArchetypeSelector.Apply(baseMask, BoardArchetypeId.Island);

            if (layout.ActiveCellCount > baseMask.ActiveCellCount)
            {
                AppendLine(reportBuilder, "[FAIL] Archetype layout enabled more cells than the base grid mask.");
                return false;
            }

            if (!layout.Mask.IsCellEligible(0, 0) && ArchetypePatternDefinition.GetPattern(BoardArchetypeId.Island)[0])
            {
                AppendLine(reportBuilder, "[FAIL] Archetype layout incorrectly enabled a base-disabled cell.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Archetype layouts respect base grid mask eligibility.");
            return true;
        }

        private static bool ValidateVariationIsolation(StringBuilder reportBuilder)
        {
            LevelRecipe baseRecipe = LevelRecipeDefinition.GenerateRecipe(30);
            GridMask baseMask = GridMaskDefinition.GenerateFromRecipe(baseRecipe);
            ArchetypeLayout baseLayout = ArchetypeSelector.Apply(baseMask, baseRecipe);

            LevelRecipe variedRecipe = new LevelRecipe(
                baseRecipe.LevelNumber,
                baseRecipe.Seed,
                baseRecipe.TileCount,
                baseRecipe.LayerDepth,
                baseRecipe.ArchetypeId,
                baseRecipe.VariationIndex + 1,
                baseRecipe.HolePatternId,
                baseRecipe.ClosedTileCount,
                baseRecipe.ClosedTilePatternId,
                baseRecipe.JokerCount,
                baseRecipe.RewardJokerPatternId,
                baseRecipe.RecommendedTimerSeconds,
                baseRecipe.DifficultyRating,
                baseRecipe.MaxRegenerationAttempts);

            ArchetypeLayout variedLayout = ArchetypeSelector.Apply(baseMask, variedRecipe);

            if (baseLayout.ActiveCellCount != variedLayout.ActiveCellCount
                || baseLayout.ArchetypeId != variedLayout.ArchetypeId)
            {
                AppendLine(reportBuilder, "[FAIL] Variation index changed archetype layout unexpectedly.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Archetype layouts ignore variation index changes.");
            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
