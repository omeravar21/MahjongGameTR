namespace MahjongGame.BoardGeneration
{
    public sealed class LevelRecipe
    {
        public int LevelNumber { get; }

        public int Seed { get; }

        public int TileCount { get; }

        public int LayerDepth { get; }

        public BoardArchetypeId ArchetypeId { get; }

        public int VariationIndex { get; }

        public HolePatternId HolePatternId { get; }

        public int ClosedTileCount { get; }

        public ClosedTilePatternId ClosedTilePatternId { get; }

        public int JokerCount { get; }

        public RewardJokerPatternId RewardJokerPatternId { get; }

        public float RecommendedTimerSeconds { get; }

        public float DifficultyRating { get; }

        public int MaxRegenerationAttempts { get; }

        public LevelRecipe(
            int levelNumber,
            int seed,
            int tileCount,
            int layerDepth,
            BoardArchetypeId archetypeId,
            int variationIndex,
            HolePatternId holePatternId,
            int closedTileCount,
            ClosedTilePatternId closedTilePatternId,
            int jokerCount,
            RewardJokerPatternId rewardJokerPatternId,
            float recommendedTimerSeconds,
            float difficultyRating,
            int maxRegenerationAttempts)
        {
            LevelNumber = levelNumber;
            Seed = seed;
            TileCount = tileCount;
            LayerDepth = layerDepth;
            ArchetypeId = archetypeId;
            VariationIndex = variationIndex;
            HolePatternId = holePatternId;
            ClosedTileCount = closedTileCount;
            ClosedTilePatternId = closedTilePatternId;
            JokerCount = jokerCount;
            RewardJokerPatternId = rewardJokerPatternId;
            RecommendedTimerSeconds = recommendedTimerSeconds;
            DifficultyRating = difficultyRating;
            MaxRegenerationAttempts = maxRegenerationAttempts;
        }
    }
}
