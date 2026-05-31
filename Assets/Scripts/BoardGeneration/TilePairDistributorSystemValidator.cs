using System.Collections.Generic;
using System.Text;
using MahjongGame.Tiles;

namespace MahjongGame.BoardGeneration
{
    public static class TilePairDistributorSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateLaunchArchetypes(reportBuilder);
            passed &= ValidateDeterministicDistribution(reportBuilder);
            passed &= ValidatePairIntegrity(reportBuilder);
            passed &= ValidateBounds(reportBuilder);
            passed &= ValidateFairness(reportBuilder);
            passed &= ValidateMetadataPreservation(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] TilePairDistributor system validation completed successfully."
                : "[FAIL] TilePairDistributor system validation found issues.");

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
                LayeredBoardLayout layeredBoardLayout = LayerBuilder.Build(holePatternLayout, 24, 2);
                DistributedBoardLayout distributedLayout = TilePairDistributor.Distribute(
                    layeredBoardLayout,
                    LevelRecipeDefinition.GenerateRecipe(1));

                if (distributedLayout.ArchetypeId != archetypeId
                    || distributedLayout.EffectiveTileCount <= 0
                    || distributedLayout.EffectiveTileCount % 2 != 0
                    || distributedLayout.Assignments == null
                    || distributedLayout.Assignments.Count != distributedLayout.EffectiveTileCount)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] TilePairDistributor failed for archetype "
                        + archetypeId
                        + ".");
                    passed = false;
                    continue;
                }

                AppendLine(
                    reportBuilder,
                    "[PASS] TilePairDistributor produced a valid layout for archetype "
                    + archetypeId
                    + ".");
            }

            return passed;
        }

        private static bool ValidateDeterministicDistribution(StringBuilder reportBuilder)
        {
            DistributedBoardLayout first = TilePairDistributor.DistributeFromLevel(25);
            DistributedBoardLayout second = TilePairDistributor.DistributeFromLevel(25);

            if (!LayoutsEqual(first, second))
            {
                AppendLine(reportBuilder, "[FAIL] Level 25 tile pair distribution is not deterministic.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Tile pair distributions are deterministic per level recipe.");
            return true;
        }

        private static bool ValidatePairIntegrity(StringBuilder reportBuilder)
        {
            DistributedBoardLayout layout = TilePairDistributor.DistributeFromLevel(12);
            Dictionary<int, int> symbolCounts = new Dictionary<int, int>();

            for (int index = 0; index < layout.Assignments.Count; index++)
            {
                int symbolId = layout.Assignments[index].SymbolId;
                if (!symbolCounts.ContainsKey(symbolId))
                {
                    symbolCounts[symbolId] = 0;
                }

                symbolCounts[symbolId]++;
            }

            foreach (KeyValuePair<int, int> entry in symbolCounts)
            {
                if (entry.Value != 2)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] Symbol "
                        + entry.Key
                        + " appeared "
                        + entry.Value
                        + " times instead of exactly 2.");
                    return false;
                }
            }

            if (layout.EffectiveTileCount % 2 != 0)
            {
                AppendLine(reportBuilder, "[FAIL] Effective tile count is not even.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] All distributed symbols appear in valid pairs.");
            return true;
        }

        private static bool ValidateBounds(StringBuilder reportBuilder)
        {
            LayeredBoardLayout layeredBoardLayout = LayerBuilder.BuildFromLevel(18);
            DistributedBoardLayout distributedLayout = TilePairDistributor.Distribute(
                layeredBoardLayout,
                LevelRecipeDefinition.GenerateRecipe(18));

            int expectedEffectiveCount = layeredBoardLayout.AssignedTileCount;
            if (expectedEffectiveCount % 2 != 0)
            {
                expectedEffectiveCount--;
            }

            if (distributedLayout.EffectiveTileCount != expectedEffectiveCount)
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] Distributed tile count does not match the layered board effective count.");
                return false;
            }

            for (int index = 0; index < distributedLayout.Assignments.Count; index++)
            {
                TileSymbolAssignment assignment = distributedLayout.Assignments[index];

                if (assignment.TileId != index)
                {
                    AppendLine(reportBuilder, "[FAIL] TilePairDistributor produced non-sequential tile ids.");
                    return false;
                }

                if (!assignment.Position.Equals(layeredBoardLayout.Positions[index]))
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] TilePairDistributor changed a tile board position.");
                    return false;
                }

                if (assignment.SymbolId < 0
                    || assignment.SymbolId >= TilePairDistributionDefinition.LaunchSymbolCount)
                {
                    AppendLine(reportBuilder, "[FAIL] TilePairDistributor assigned an out-of-range symbol id.");
                    return false;
                }
            }

            AppendLine(reportBuilder, "[PASS] TilePairDistributor preserved layered positions and symbol bounds.");
            return true;
        }

        private static bool ValidateFairness(StringBuilder reportBuilder)
        {
            DistributedBoardLayout layout = TilePairDistributor.DistributeFromLevel(30);

            if (layout.PairCount >= 4 && layout.DistinctSymbolCount < layout.PairCount)
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] TilePairDistributor reused symbols when enough launch symbols were available.");
                return false;
            }

            Dictionary<int, int> symbolCounts = new Dictionary<int, int>();
            for (int index = 0; index < layout.Assignments.Count; index++)
            {
                int symbolId = layout.Assignments[index].SymbolId;
                if (!symbolCounts.ContainsKey(symbolId))
                {
                    symbolCounts[symbolId] = 0;
                }

                symbolCounts[symbolId]++;
            }

            foreach (KeyValuePair<int, int> entry in symbolCounts)
            {
                if (entry.Value > 2)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] Symbol "
                        + entry.Key
                        + " appeared more than twice.");
                    return false;
                }
            }

            AppendLine(reportBuilder, "[PASS] TilePairDistributor distributed pairs fairly.");
            return true;
        }

        private static bool ValidateMetadataPreservation(StringBuilder reportBuilder)
        {
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(20);
            LayeredBoardLayout layeredBoardLayout = LayerBuilder.BuildFromLevel(recipe.LevelNumber);
            DistributedBoardLayout distributedLayout = TilePairDistributor.Distribute(layeredBoardLayout, recipe);

            if (distributedLayout.ArchetypeId != recipe.ArchetypeId
                || distributedLayout.VariationIndex != recipe.VariationIndex
                || distributedLayout.HolePatternId != recipe.HolePatternId
                || distributedLayout.LayerDepth != layeredBoardLayout.LayerDepth
                || distributedLayout.RequestedTileCount != layeredBoardLayout.RequestedTileCount
                || distributedLayout.Seed != recipe.Seed)
            {
                AppendLine(reportBuilder, "[FAIL] TilePairDistributor dropped upstream pipeline metadata.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] TilePairDistributor preserved upstream pipeline metadata.");
            return true;
        }

        private static bool LayoutsEqual(DistributedBoardLayout left, DistributedBoardLayout right)
        {
            if (left == null || right == null || left.Assignments == null || right.Assignments == null)
            {
                return false;
            }

            if (left.EffectiveTileCount != right.EffectiveTileCount
                || left.PairCount != right.PairCount
                || left.DistinctSymbolCount != right.DistinctSymbolCount
                || left.ArchetypeId != right.ArchetypeId
                || left.VariationIndex != right.VariationIndex
                || left.HolePatternId != right.HolePatternId
                || left.Seed != right.Seed)
            {
                return false;
            }

            if (left.Assignments.Count != right.Assignments.Count)
            {
                return false;
            }

            for (int index = 0; index < left.Assignments.Count; index++)
            {
                TileSymbolAssignment leftAssignment = left.Assignments[index];
                TileSymbolAssignment rightAssignment = right.Assignments[index];

                if (leftAssignment.TileId != rightAssignment.TileId
                    || leftAssignment.SymbolId != rightAssignment.SymbolId
                    || !leftAssignment.Position.Equals(rightAssignment.Position))
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
