using System.Collections.Generic;

namespace MahjongGame.BoardGeneration
{
    public sealed class DistributedBoardLayout
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

        public IReadOnlyList<TileSymbolAssignment> Assignments { get; }

        public DistributedBoardLayout(
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
            Assignments = assignments ?? new TileSymbolAssignment[0];
        }
    }
}
