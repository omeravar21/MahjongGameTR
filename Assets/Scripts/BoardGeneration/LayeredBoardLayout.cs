using System.Collections.Generic;
using MahjongGame.Tiles;

namespace MahjongGame.BoardGeneration
{
    public sealed class LayeredBoardLayout
    {
        public HolePatternId HolePatternId { get; }

        public BoardArchetypeId ArchetypeId { get; }

        public int VariationIndex { get; }

        public int LevelNumber { get; }

        public int Seed { get; }

        public int LayerDepth { get; }

        public int RequestedTileCount { get; }

        public int AssignedTileCount { get; }

        public int AvailableSlotCount { get; }

        public IReadOnlyList<TileBoardPosition> Positions { get; }

        public LayeredBoardLayout(
            HolePatternId holePatternId,
            BoardArchetypeId archetypeId,
            int variationIndex,
            int levelNumber,
            int seed,
            int layerDepth,
            int requestedTileCount,
            int assignedTileCount,
            int availableSlotCount,
            IReadOnlyList<TileBoardPosition> positions)
        {
            HolePatternId = holePatternId;
            ArchetypeId = archetypeId;
            VariationIndex = variationIndex;
            LevelNumber = levelNumber;
            Seed = seed;
            LayerDepth = layerDepth;
            RequestedTileCount = requestedTileCount;
            AssignedTileCount = assignedTileCount;
            AvailableSlotCount = availableSlotCount;
            Positions = positions ?? new TileBoardPosition[0];
        }
    }
}
