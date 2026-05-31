namespace MahjongGame.BoardGeneration
{
    public sealed class DifficultyProfile
    {
        public int LevelNumber { get; }

        public int TileCountMin { get; }

        public int TileCountMax { get; }

        public int TileCount { get; }

        public int LayerDepth { get; }

        public int ClosedTileMin { get; }

        public int ClosedTileMax { get; }

        public int ClosedTileCount { get; }

        public int JokerMin { get; }

        public int JokerMax { get; }

        public int JokerCount { get; }

        public float RecommendedTimerSeconds { get; }

        public ComplexityTier ComplexityTier { get; }

        public DifficultyProfile(
            int levelNumber,
            int tileCountMin,
            int tileCountMax,
            int tileCount,
            int layerDepth,
            int closedTileMin,
            int closedTileMax,
            int closedTileCount,
            int jokerMin,
            int jokerMax,
            int jokerCount,
            float recommendedTimerSeconds,
            ComplexityTier complexityTier)
        {
            LevelNumber = levelNumber;
            TileCountMin = tileCountMin;
            TileCountMax = tileCountMax;
            TileCount = tileCount;
            LayerDepth = layerDepth;
            ClosedTileMin = closedTileMin;
            ClosedTileMax = closedTileMax;
            ClosedTileCount = closedTileCount;
            JokerMin = jokerMin;
            JokerMax = jokerMax;
            JokerCount = jokerCount;
            RecommendedTimerSeconds = recommendedTimerSeconds;
            ComplexityTier = complexityTier;
        }
    }
}
