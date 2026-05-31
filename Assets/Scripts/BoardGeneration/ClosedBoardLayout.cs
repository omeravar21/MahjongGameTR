using System.Collections.Generic;

namespace MahjongGame.BoardGeneration
{
    public sealed class ClosedBoardLayout
    {
        public HolePatternId HolePatternId { get; }

        public BoardArchetypeId ArchetypeId { get; }

        public int VariationIndex { get; }

        public int LevelNumber { get; }

        public int Seed { get; }

        public int LayerDepth { get; }

        public int RequestedTileCount { get; }

        public int EffectiveTileCount { get; }

        public int PairCount { get; }

        public int DistinctSymbolCount { get; }

        public ClosedTilePatternId ClosedTilePatternId { get; }

        public int AppliedClosedTileCount { get; }

        public IReadOnlyList<TileSymbolAssignment> Assignments { get; }

        public ClosedBoardLayout(
            HolePatternId holePatternId,
            BoardArchetypeId archetypeId,
            int variationIndex,
            int levelNumber,
            int seed,
            int layerDepth,
            int requestedTileCount,
            int effectiveTileCount,
            int pairCount,
            int distinctSymbolCount,
            ClosedTilePatternId closedTilePatternId,
            int appliedClosedTileCount,
            IReadOnlyList<TileSymbolAssignment> assignments)
        {
            HolePatternId = holePatternId;
            ArchetypeId = archetypeId;
            VariationIndex = variationIndex;
            LevelNumber = levelNumber;
            Seed = seed;
            LayerDepth = layerDepth;
            RequestedTileCount = requestedTileCount;
            EffectiveTileCount = effectiveTileCount;
            PairCount = pairCount;
            DistinctSymbolCount = distinctSymbolCount;
            ClosedTilePatternId = closedTilePatternId;
            AppliedClosedTileCount = appliedClosedTileCount;
            Assignments = assignments ?? new TileSymbolAssignment[0];
        }

        public static ClosedBoardLayout FromDistributedLayout(
            DistributedBoardLayout layout,
            ClosedTilePatternId closedTilePatternId,
            int appliedClosedTileCount,
            IReadOnlyList<TileSymbolAssignment> assignments)
        {
            if (layout == null)
            {
                return new ClosedBoardLayout(
                    HolePatternId.SingleCenter,
                    BoardArchetypeId.Diamond,
                    0,
                    0,
                    0,
                    1,
                    0,
                    0,
                    0,
                    0,
                    closedTilePatternId,
                    appliedClosedTileCount,
                    assignments);
            }

            return new ClosedBoardLayout(
                layout.HolePatternId,
                layout.ArchetypeId,
                layout.VariationIndex,
                layout.LevelNumber,
                layout.Seed,
                layout.LayerDepth,
                layout.RequestedTileCount,
                layout.EffectiveTileCount,
                layout.PairCount,
                layout.DistinctSymbolCount,
                closedTilePatternId,
                appliedClosedTileCount,
                assignments);
        }
    }
}
