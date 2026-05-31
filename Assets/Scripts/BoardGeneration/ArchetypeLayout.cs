namespace MahjongGame.BoardGeneration
{
    public sealed class ArchetypeLayout
    {
        public BoardArchetypeId ArchetypeId { get; }

        public int LevelNumber { get; }

        public int Seed { get; }

        public int ActiveCellCount { get; }

        public GridMask Mask { get; }

        public ArchetypeLayout(
            BoardArchetypeId archetypeId,
            int levelNumber,
            int seed,
            GridMask mask)
        {
            ArchetypeId = archetypeId;
            LevelNumber = levelNumber;
            Seed = seed;
            Mask = mask;
            ActiveCellCount = mask != null ? mask.ActiveCellCount : 0;
        }
    }
}
