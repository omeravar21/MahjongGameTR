using MahjongGame.Progression;
using UnityEngine;

namespace MahjongGame.BoardGeneration
{
    public static class DifficultyDefinition
    {
        public const int MaximumTileCount = 140;
        public const int MaximumLayerDepth = 4;
        public const int ClosedTileActivationLevel = 10;
        public const float MinimumRecommendedTimerSeconds = 90f;

        public static DifficultyProfile ResolveProfile(int levelNumber)
        {
            int clampedLevel = LevelProgressData.ClampLevel(levelNumber);

            ResolveTileCountBand(clampedLevel, out int tileMin, out int tileMax, out int tileCount);
            ResolveClosedTileBand(clampedLevel, out int closedMin, out int closedMax, out int closedCount);
            ResolveJokerBand(clampedLevel, out int jokerMin, out int jokerMax, out int jokerCount);
            int layerDepth = ResolveLayerDepth(clampedLevel);
            ComplexityTier complexityTier = ResolveComplexityTier(clampedLevel);
            float recommendedTimerSeconds = ResolveRecommendedTimerSeconds(
                tileCount,
                layerDepth,
                closedCount,
                jokerCount);

            return new DifficultyProfile(
                clampedLevel,
                tileMin,
                tileMax,
                tileCount,
                layerDepth,
                closedMin,
                closedMax,
                closedCount,
                jokerMin,
                jokerMax,
                jokerCount,
                recommendedTimerSeconds,
                complexityTier);
        }

        private static void ResolveTileCountBand(int level, out int min, out int max, out int resolved)
        {
            if (level <= 20)
            {
                min = 80;
                max = 88;
            }
            else if (level <= 100)
            {
                min = 88;
                max = 100;
            }
            else if (level <= 300)
            {
                min = 100;
                max = 112;
            }
            else if (level <= 1000)
            {
                min = 112;
                max = 126;
            }
            else
            {
                min = 120;
                max = MaximumTileCount;
            }

            float progress = ResolveBandProgress(level, GetBandStartLevel(level, min, max), GetBandEndLevel(level, min, max));
            resolved = RoundToInt(Mathf.Lerp(min, max, progress));
            resolved = Mathf.Clamp(resolved, min, MaximumTileCount);
        }

        private static int GetBandStartLevel(int level, int min, int max)
        {
            if (level <= 20)
            {
                return LevelProgressData.MinLevel;
            }

            if (level <= 100)
            {
                return 21;
            }

            if (level <= 300)
            {
                return 101;
            }

            if (level <= 1000)
            {
                return 301;
            }

            return 1001;
        }

        private static int GetBandEndLevel(int level, int min, int max)
        {
            if (level <= 20)
            {
                return 20;
            }

            if (level <= 100)
            {
                return 100;
            }

            if (level <= 300)
            {
                return 300;
            }

            if (level <= 1000)
            {
                return 1000;
            }

            return LevelProgressData.MaxLevel;
        }

        private static void ResolveClosedTileBand(int level, out int min, out int max, out int resolved)
        {
            if (level < ClosedTileActivationLevel)
            {
                min = 0;
                max = 0;
                resolved = 0;
                return;
            }

            if (level <= 20)
            {
                min = 6;
                max = 8;
            }
            else if (level <= 100)
            {
                min = 8;
                max = 10;
            }
            else if (level <= 300)
            {
                min = 10;
                max = 12;
            }
            else
            {
                min = 10;
                max = 14;
            }

            float progress = ResolveBandProgress(level, GetClosedBandStartLevel(level), GetClosedBandEndLevel(level));
            resolved = RoundToInt(Mathf.Lerp(min, max, progress));
            resolved = Mathf.Clamp(resolved, min, max);
        }

        private static int GetClosedBandStartLevel(int level)
        {
            if (level <= 20)
            {
                return ClosedTileActivationLevel;
            }

            if (level <= 100)
            {
                return 21;
            }

            if (level <= 300)
            {
                return 101;
            }

            return 301;
        }

        private static int GetClosedBandEndLevel(int level)
        {
            if (level <= 20)
            {
                return 20;
            }

            if (level <= 100)
            {
                return 100;
            }

            if (level <= 300)
            {
                return 300;
            }

            return LevelProgressData.MaxLevel;
        }

        private static void ResolveJokerBand(int level, out int min, out int max, out int resolved)
        {
            ComplexityTier tier = ResolveComplexityTier(level);
            switch (tier)
            {
                case ComplexityTier.Low:
                    min = 1;
                    max = 1;
                    resolved = 1;
                    return;
                case ComplexityTier.Mid:
                    min = 1;
                    max = 2;
                    break;
                default:
                    min = 2;
                    max = 3;
                    break;
            }

            float progress = ResolveBandProgress(level, GetJokerBandStartLevel(tier), GetJokerBandEndLevel(tier));
            resolved = RoundToInt(Mathf.Lerp(min, max, progress));
            resolved = Mathf.Clamp(resolved, min, max);
        }

        private static int GetJokerBandStartLevel(ComplexityTier tier)
        {
            switch (tier)
            {
                case ComplexityTier.Mid:
                    return 21;
                default:
                    return 101;
            }
        }

        private static int GetJokerBandEndLevel(ComplexityTier tier)
        {
            switch (tier)
            {
                case ComplexityTier.Mid:
                    return 100;
                default:
                    return LevelProgressData.MaxLevel;
            }
        }

        private static ComplexityTier ResolveComplexityTier(int level)
        {
            if (level <= 20)
            {
                return ComplexityTier.Low;
            }

            if (level <= 100)
            {
                return ComplexityTier.Mid;
            }

            return ComplexityTier.High;
        }

        private static int ResolveLayerDepth(int level)
        {
            if (level <= 20)
            {
                return RoundToInt(Mathf.Lerp(1f, 2f, ResolveBandProgress(level, LevelProgressData.MinLevel, 20)));
            }

            if (level <= 100)
            {
                return RoundToInt(Mathf.Lerp(2f, 3f, ResolveBandProgress(level, 21, 100)));
            }

            if (level <= 300)
            {
                return RoundToInt(Mathf.Lerp(3f, 4f, ResolveBandProgress(level, 101, 300)));
            }

            return MaximumLayerDepth;
        }

        private static float ResolveRecommendedTimerSeconds(
            int tileCount,
            int layerDepth,
            int closedTileCount,
            int jokerCount)
        {
            float timerSeconds = 50f
                + (tileCount * 0.75f)
                + (layerDepth * 20f)
                + (closedTileCount * 3f)
                + (jokerCount * 15f);

            return Mathf.Max(MinimumRecommendedTimerSeconds, timerSeconds);
        }

        private static float ResolveBandProgress(int level, int bandStartLevel, int bandEndLevel)
        {
            if (bandEndLevel <= bandStartLevel)
            {
                return 0f;
            }

            float progress = (level - bandStartLevel) / (float)(bandEndLevel - bandStartLevel);
            return Mathf.Clamp01(progress);
        }

        private static int RoundToInt(float value)
        {
            return Mathf.RoundToInt(value);
        }
    }
}
