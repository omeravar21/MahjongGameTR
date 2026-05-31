using System.Collections.Generic;
using System.Text;
using MahjongGame.Board;
using MahjongGame.Tiles;

namespace MahjongGame.BoardGeneration
{
    public static class LayerBuilderSystemValidator
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
            passed &= ValidateBounds(reportBuilder);
            passed &= ValidateUniqueness(reportBuilder);
            passed &= ValidateLayerDepth(reportBuilder);
            passed &= ValidateReadability(reportBuilder);
            passed &= ValidateMetadataPreservation(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] LayerBuilder system validation completed successfully."
                : "[FAIL] LayerBuilder system validation found issues.");

            return passed;
        }

        private static bool ValidateLaunchArchetypes(StringBuilder reportBuilder)
        {
            bool passed = true;
            GridMask baseMask = GridMaskDefinition.CreateFullBaseGridMask(1, 0);

            foreach (BoardArchetypeId archetypeId in System.Enum.GetValues(typeof(BoardArchetypeId)))
            {
                ArchetypeLayout archetypeLayout = ArchetypeSelector.Apply(baseMask, archetypeId);
                VariationLayout variationLayout = VariationSelector.Apply(archetypeLayout, 0);
                HolePatternLayout holePatternLayout = HolePatternSelector.Apply(
                    variationLayout,
                    HolePatternId.SingleCenter);

                LayeredBoardLayout layout = LayerBuilder.Build(holePatternLayout, 24, 2);

                if (layout.ArchetypeId != archetypeId
                    || layout.AssignedTileCount <= 0
                    || layout.Positions == null
                    || layout.Positions.Count != layout.AssignedTileCount)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] LayerBuilder failed for archetype "
                        + archetypeId
                        + ".");
                    passed = false;
                    continue;
                }

                AppendLine(
                    reportBuilder,
                    "[PASS] LayerBuilder produced a valid layout for archetype "
                    + archetypeId
                    + ".");
            }

            return passed;
        }

        private static bool ValidateDeterministicLayout(StringBuilder reportBuilder)
        {
            LayeredBoardLayout first = LayerBuilder.BuildFromLevel(25);
            LayeredBoardLayout second = LayerBuilder.BuildFromLevel(25);

            if (!LayoutsEqual(first, second))
            {
                AppendLine(reportBuilder, "[FAIL] Level 25 layered board layout is not deterministic.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Layered board layouts are deterministic per level recipe.");
            return true;
        }

        private static bool ValidateBounds(StringBuilder reportBuilder)
        {
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(12);
            GridMask baseMask = GridMaskDefinition.GenerateFromRecipe(recipe);
            ArchetypeLayout archetypeLayout = ArchetypeSelector.Apply(baseMask, recipe);
            VariationLayout variationLayout = VariationSelector.Apply(archetypeLayout, recipe);
            HolePatternLayout holePatternLayout = HolePatternSelector.Apply(variationLayout, recipe);
            LayeredBoardLayout layout = LayerBuilder.Build(holePatternLayout, recipe);

            for (int index = 0; index < layout.Positions.Count; index++)
            {
                TileBoardPosition position = layout.Positions[index];

                if (!position.IsValid)
                {
                    AppendLine(reportBuilder, "[FAIL] LayerBuilder produced an invalid tile board position.");
                    return false;
                }

                if (!holePatternLayout.Mask.IsCellEligible(
                        position.GridCoordinate.Column,
                        position.GridCoordinate.Row))
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] LayerBuilder assigned a position outside the hole-pattern mask.");
                    return false;
                }

                if (!BoardLayerDefinition.IsValidLayerIndex(position.LayerIndex))
                {
                    AppendLine(reportBuilder, "[FAIL] LayerBuilder assigned an out-of-range layer index.");
                    return false;
                }
            }

            AppendLine(reportBuilder, "[PASS] LayerBuilder positions stay within hole-pattern and layer bounds.");
            return true;
        }

        private static bool ValidateUniqueness(StringBuilder reportBuilder)
        {
            LayeredBoardLayout layout = LayerBuilder.BuildFromLevel(18);
            HashSet<TileBoardPosition> uniquePositions = new HashSet<TileBoardPosition>();

            for (int index = 0; index < layout.Positions.Count; index++)
            {
                if (!uniquePositions.Add(layout.Positions[index]))
                {
                    AppendLine(reportBuilder, "[FAIL] LayerBuilder produced duplicate tile board positions.");
                    return false;
                }
            }

            AppendLine(reportBuilder, "[PASS] LayerBuilder positions are unique per coordinate and layer.");
            return true;
        }

        private static bool ValidateLayerDepth(StringBuilder reportBuilder)
        {
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(30);
            LayeredBoardLayout layout = LayerBuilder.BuildFromLevel(recipe.LevelNumber);
            int expectedLayerDepth = LayerBuildDefinition.ClampLayerDepth(recipe.LayerDepth);

            if (layout.LayerDepth != expectedLayerDepth)
            {
                AppendLine(reportBuilder, "[FAIL] LayerBuilder layer depth does not match the level recipe.");
                return false;
            }

            for (int index = 0; index < layout.Positions.Count; index++)
            {
                if (layout.Positions[index].LayerIndex >= expectedLayerDepth)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] LayerBuilder assigned a position above the recipe layer depth.");
                    return false;
                }
            }

            AppendLine(reportBuilder, "[PASS] LayerBuilder respects recipe layer depth.");
            return true;
        }

        private static bool ValidateReadability(StringBuilder reportBuilder)
        {
            LayeredBoardLayout layout = LayerBuilder.BuildFromLevel(5);

            if (layout.AssignedTileCount <= 0)
            {
                AppendLine(reportBuilder, "[FAIL] LayerBuilder assigned no tile positions.");
                return false;
            }

            bool usesMultipleLayers = false;
            int firstLayerIndex = layout.Positions[0].LayerIndex;

            for (int index = 1; index < layout.Positions.Count; index++)
            {
                if (layout.Positions[index].LayerIndex != firstLayerIndex)
                {
                    usesMultipleLayers = true;
                    break;
                }
            }

            if (layout.LayerDepth > 1 && layout.AssignedTileCount > layout.AvailableSlotCount / layout.LayerDepth
                && !usesMultipleLayers)
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] LayerBuilder did not spread positions across readable layer depth.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] LayerBuilder produced a readable layered layout.");
            return true;
        }

        private static bool ValidateMetadataPreservation(StringBuilder reportBuilder)
        {
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(20);
            LayeredBoardLayout layout = LayerBuilder.BuildFromLevel(recipe.LevelNumber);

            if (layout.ArchetypeId != recipe.ArchetypeId
                || layout.VariationIndex != recipe.VariationIndex
                || layout.HolePatternId != recipe.HolePatternId
                || layout.RequestedTileCount != recipe.TileCount
                || layout.Seed != recipe.Seed)
            {
                AppendLine(reportBuilder, "[FAIL] LayerBuilder dropped upstream pipeline metadata.");
                return false;
            }

            if (layout.AssignedTileCount > layout.RequestedTileCount)
            {
                AppendLine(reportBuilder, "[FAIL] LayerBuilder assigned more tiles than requested.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] LayerBuilder preserved upstream pipeline metadata.");
            return true;
        }

        private static bool LayoutsEqual(LayeredBoardLayout left, LayeredBoardLayout right)
        {
            if (left == null || right == null || left.Positions == null || right.Positions == null)
            {
                return false;
            }

            if (left.AssignedTileCount != right.AssignedTileCount
                || left.LayerDepth != right.LayerDepth
                || left.ArchetypeId != right.ArchetypeId
                || left.VariationIndex != right.VariationIndex
                || left.HolePatternId != right.HolePatternId
                || left.Seed != right.Seed)
            {
                return false;
            }

            if (left.Positions.Count != right.Positions.Count)
            {
                return false;
            }

            for (int index = 0; index < left.Positions.Count; index++)
            {
                if (!left.Positions[index].Equals(right.Positions[index]))
                {
                    return false;
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
