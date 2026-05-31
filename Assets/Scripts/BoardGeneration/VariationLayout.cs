namespace MahjongGame.BoardGeneration
{
    public sealed class VariationLayout
    {
        public BoardArchetypeId ArchetypeId { get; }

        public int VariationIndex { get; }

        public int LevelNumber { get; }

        public int Seed { get; }

        public int ActiveCellCount { get; }

        public GridMask Mask { get; }

        public VariationLayout(
            BoardArchetypeId archetypeId,
            int variationIndex,
            int levelNumber,
            int seed,
            GridMask mask)
        {
            ArchetypeId = archetypeId;
            VariationIndex = variationIndex;
            LevelNumber = levelNumber;
            Seed = seed;
            Mask = mask;
            ActiveCellCount = mask != null ? mask.ActiveCellCount : 0;
        }
    }
}
