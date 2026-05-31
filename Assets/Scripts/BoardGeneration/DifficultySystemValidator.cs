using System.Text;
using MahjongGame.Progression;
using UnityEngine;

namespace MahjongGame.BoardGeneration
{
    public static class DifficultySystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateSampleLevel(1, reportBuilder, profile =>
                profile.ClosedTileCount == 0
                && profile.TileCount >= 80
                && profile.TileCount <= 88
                && profile.JokerCount == 1
                && profile.RecommendedTimerSeconds > 0f);

            passed &= ValidateSampleLevel(10, reportBuilder, profile =>
                profile.ClosedTileCount >= 6
                && profile.ClosedTileCount <= 8
                && profile.ClosedTileMin == 6
                && profile.ClosedTileMax == 8);

            passed &= ValidateSampleLevel(50, reportBuilder, profile =>
                profile.TileCount >= 88
                && profile.TileCount <= 100
                && profile.JokerCount >= 1
                && profile.JokerCount <= 2
                && profile.ComplexityTier == ComplexityTier.Mid);

            passed &= ValidateSampleLevel(300, reportBuilder, profile =>
                profile.TileCount >= 100
                && profile.TileCount <= 112
                && profile.ClosedTileCount >= 10
                && profile.ClosedTileCount <= 12);

            passed &= ValidateSampleLevel(1000, reportBuilder, profile =>
                profile.TileCount >= 112
                && profile.TileCount <= 126
                && profile.TileCount <= DifficultyDefinition.MaximumTileCount
                && profile.LayerDepth <= DifficultyDefinition.MaximumLayerDepth);

            passed &= ValidateSampleLevel(LevelProgressData.MaxLevel, reportBuilder, profile =>
                profile.TileCount <= DifficultyDefinition.MaximumTileCount
                && profile.RecommendedTimerSeconds >= DifficultyDefinition.MinimumRecommendedTimerSeconds);

            passed &= ValidateMonotonicTimerGrowth(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Difficulty system validation completed successfully."
                : "[FAIL] Difficulty system validation found issues.");

            return passed;
        }

        private static bool ValidateSampleLevel(
            int levelNumber,
            StringBuilder reportBuilder,
            System.Func<DifficultyProfile, bool> predicate)
        {
            DifficultyProfile profile = DifficultyDefinition.ResolveProfile(levelNumber);
            if (profile == null)
            {
                AppendLine(reportBuilder, "[FAIL] Level " + levelNumber + " did not resolve a difficulty profile.");
                return false;
            }

            if (!predicate(profile))
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] Level "
                    + levelNumber
                    + " profile failed checks: tiles="
                    + profile.TileCount
                    + ", closed="
                    + profile.ClosedTileCount
                    + ", jokers="
                    + profile.JokerCount
                    + ", timer="
                    + profile.RecommendedTimerSeconds
                    + ", tier="
                    + profile.ComplexityTier
                    + ".");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Level " + levelNumber + " difficulty profile is valid.");
            return true;
        }

        private static bool ValidateMonotonicTimerGrowth(StringBuilder reportBuilder)
        {
            float previousTimer = 0f;
            int[] sampleLevels = { 1, 10, 50, 100, 300, 1000 };

            for (int index = 0; index < sampleLevels.Length; index++)
            {
                int levelNumber = sampleLevels[index];
                float timerSeconds = DifficultyDefinition.ResolveProfile(levelNumber).RecommendedTimerSeconds;

                if (timerSeconds < previousTimer)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] Timer recommendation decreased between progression samples at level "
                        + levelNumber
                        + ".");
                    return false;
                }

                previousTimer = timerSeconds;
            }

            AppendLine(reportBuilder, "[PASS] Timer recommendations grow across sample progression levels.");
            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
