namespace MahjongGame.BoardGeneration
{
    public sealed class VisualVarietyProfile
    {
        public int LevelNumber { get; }

        public int DeterministicSeed { get; }

        public BoardArchetypeId ArchetypeId { get; }

        public int VariationIndex { get; }

        public HolePatternId HolePatternId { get; }

        public ClosedTilePatternId ClosedTilePatternId { get; }

        public VisualVarietyProfile(
            int levelNumber,
            int deterministicSeed,
            BoardArchetypeId archetypeId,
            int variationIndex,
            HolePatternId holePatternId,
            ClosedTilePatternId closedTilePatternId)
        {
            LevelNumber = levelNumber;
            DeterministicSeed = deterministicSeed;
            ArchetypeId = archetypeId;
            VariationIndex = variationIndex;
            HolePatternId = holePatternId;
            ClosedTilePatternId = closedTilePatternId;
        }
    }
}
