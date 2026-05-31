namespace MahjongGame.BoardGeneration
{
    public sealed class HolePatternLayout
    {
        public HolePatternId HolePatternId { get; }

        public BoardArchetypeId ArchetypeId { get; }

        public int VariationIndex { get; }

        public int LevelNumber { get; }

        public int Seed { get; }

        public int ActiveCellCount { get; }

        public GridMask Mask { get; }

        public HolePatternLayout(
            HolePatternId holePatternId,
            BoardArchetypeId archetypeId,
            int variationIndex,
            int levelNumber,
            int seed,
            GridMask mask)
        {
            HolePatternId = holePatternId;
            ArchetypeId = archetypeId;
            VariationIndex = variationIndex;
            LevelNumber = levelNumber;
            Seed = seed;
            Mask = mask;
            ActiveCellCount = mask != null ? mask.ActiveCellCount : 0;
        }
    }
}
