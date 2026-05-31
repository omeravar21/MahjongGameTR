using MahjongGame.Progression;

namespace MahjongGame.BoardGeneration
{
    public static class LevelRecipeDefinition
    {
        public const int MaxRegenerationAttempts = 50;
        public const int LaunchRewardJokerPatternCount = 4;

        public static LevelRecipe GenerateRecipe(
            int levelNumber,
            DifficultyProfile difficultyProfile,
            VisualVarietyProfile visualVarietyProfile)
        {
            int clampedLevel = LevelProgressData.ClampLevel(levelNumber);
            DifficultyProfile difficulty = difficultyProfile ?? DifficultyDefinition.ResolveProfile(clampedLevel);
            VisualVarietyProfile variety = visualVarietyProfile ?? VisualVarietyDefinition.ResolveProfile(clampedLevel);

            RewardJokerPatternId rewardJokerPatternId = ResolveRewardJokerPatternId(
                variety.DeterministicSeed,
                difficulty.JokerCount);

            float difficultyRating = ResolveDifficultyRating(difficulty, variety);

            return new LevelRecipe(
                clampedLevel,
                variety.DeterministicSeed,
                difficulty.TileCount,
                difficulty.LayerDepth,
                variety.ArchetypeId,
                variety.VariationIndex,
                variety.HolePatternId,
                difficulty.ClosedTileCount,
                variety.ClosedTilePatternId,
                difficulty.JokerCount,
                rewardJokerPatternId,
                difficulty.RecommendedTimerSeconds,
                difficultyRating,
                MaxRegenerationAttempts);
        }

        public static LevelRecipe GenerateRecipe(int levelNumber)
        {
            int clampedLevel = LevelProgressData.ClampLevel(levelNumber);
            DifficultyProfile difficultyProfile = DifficultyDefinition.ResolveProfile(clampedLevel);
            VisualVarietyProfile visualVarietyProfile = VisualVarietyDefinition.ResolveProfile(clampedLevel);
            return GenerateRecipe(clampedLevel, difficultyProfile, visualVarietyProfile);
        }

        private static RewardJokerPatternId ResolveRewardJokerPatternId(int seed, int jokerCount)
        {
            if (jokerCount <= 0)
            {
                return RewardJokerPatternId.BalancedSpread;
            }

            int patternIndex = PositiveMod((seed * 13) + jokerCount, LaunchRewardJokerPatternCount);
            return (RewardJokerPatternId)patternIndex;
        }

        private static float ResolveDifficultyRating(
            DifficultyProfile difficultyProfile,
            VisualVarietyProfile visualVarietyProfile)
        {
            float tierWeight = difficultyProfile.ComplexityTier switch
            {
                ComplexityTier.Low => 20f,
                ComplexityTier.Mid => 40f,
                _ => 60f
            };

            float tileWeight = difficultyProfile.TileCount * 0.35f;
            float layerWeight = difficultyProfile.LayerDepth * 8f;
            float closedWeight = difficultyProfile.ClosedTileCount * 2.5f;
            float jokerWeight = difficultyProfile.JokerCount * 6f;
            float varietyWeight = ((int)visualVarietyProfile.ArchetypeId * 1.5f)
                + (visualVarietyProfile.VariationIndex * 2f)
                + ((int)visualVarietyProfile.HolePatternId * 1.25f);

            return tierWeight + tileWeight + layerWeight + closedWeight + jokerWeight + varietyWeight;
        }

        private static int PositiveMod(int value, int modulus)
        {
            if (modulus <= 0)
            {
                return 0;
            }

            int result = value % modulus;
            return result < 0 ? result + modulus : result;
        }
    }
}
