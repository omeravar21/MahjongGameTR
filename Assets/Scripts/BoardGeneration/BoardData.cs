using System.Collections.Generic;

namespace MahjongGame.BoardGeneration
{
    public sealed class BoardData
    {
        public int LevelNumber { get; }

        public int Seed { get; }

        public BoardArchetypeId ArchetypeId { get; }

        public int VariationIndex { get; }

        public HolePatternId HolePatternId { get; }

        public int LayerDepth { get; }

        public int TileCount { get; }

        public int ClosedTileCount { get; }

        public int JokerCount { get; }

        public bool IsValidated { get; }

        public IReadOnlyList<TileSymbolAssignment> TileAssignments { get; }

        public BoardData(
            int levelNumber,
            int seed,
            BoardArchetypeId archetypeId,
            int variationIndex,
            HolePatternId holePatternId,
            int layerDepth,
            int tileCount,
            int closedTileCount,
            int jokerCount,
            bool isValidated,
            IReadOnlyList<TileSymbolAssignment> tileAssignments)
        {
            LevelNumber = levelNumber;
            Seed = seed;
            ArchetypeId = archetypeId;
            VariationIndex = variationIndex;
            HolePatternId = holePatternId;
            LayerDepth = layerDepth;
            TileCount = tileCount;
            ClosedTileCount = closedTileCount;
            JokerCount = jokerCount;
            IsValidated = isValidated;
            TileAssignments = tileAssignments ?? new TileSymbolAssignment[0];
        }
    }
}
