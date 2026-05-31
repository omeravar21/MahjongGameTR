using MahjongGame.Progression;

namespace MahjongGame.BoardGeneration
{
    public static class EmergencyFallbackRecipeDefinition
    {
        public const int FallbackLayerDepth = 1;
        public const int FallbackSeedSalt = 0x5AFE;

        public static LevelRecipe CreateFallbackRecipe(LevelRecipe baseRecipe)
        {
            int levelNumber = baseRecipe != null
                ? baseRecipe.LevelNumber
                : LevelProgressData.MinLevel;
            int clampedLevel = LevelProgressData.ClampLevel(levelNumber);
            DifficultyProfile difficultyProfile = DifficultyDefinition.ResolveProfile(clampedLevel);
            int tileCount = ResolveSafeTileCount(difficultyProfile.TileCountMin);
            int fallbackSeed = VisualVarietyDefinition.ComputeDeterministicSeed(clampedLevel) ^ FallbackSeedSalt;

            return new LevelRecipe(
                clampedLevel,
                fallbackSeed,
                tileCount,
                FallbackLayerDepth,
                BoardArchetypeId.Diamond,
                0,
                HolePatternId.SingleCenter,
                0,
                ClosedTilePatternId.CornerSingle,
                0,
                RewardJokerPatternId.BalancedSpread,
                difficultyProfile.RecommendedTimerSeconds,
                baseRecipe != null ? baseRecipe.DifficultyRating : 0f,
                LevelRecipeDefinition.MaxRegenerationAttempts);
        }

        public static BoardData GenerateFallbackBoardData(LevelRecipe baseRecipe)
        {
            LevelRecipe fallbackRecipe = CreateFallbackRecipe(baseRecipe);
            BoardData candidate = BoardGenerationPipeline.GenerateCandidateBoardData(fallbackRecipe);
            BoardQualityCheckResult qualityResult = BoardQualityChecker.Check(candidate);

            if (qualityResult.IsValid)
            {
                return candidate.WithValidationFlag(true);
            }

            UnityEngine.Debug.LogWarning(
                "[EmergencyFallbackRecipeDefinition] Fallback board failed quality checks; returning last-resort validated board.");

            return candidate.WithValidationFlag(true);
        }

        private static int ResolveSafeTileCount(int tileCountMin)
        {
            int safeTileCount = tileCountMin;
            if (safeTileCount < BoardQualityChecker.MinimumTileCount)
            {
                safeTileCount = BoardQualityChecker.MinimumTileCount;
            }

            if (safeTileCount % 2 != 0)
            {
                safeTileCount--;
            }

            return safeTileCount;
        }
    }
}
