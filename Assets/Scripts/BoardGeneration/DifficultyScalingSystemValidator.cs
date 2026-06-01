using System.Text;
using MahjongGame.Progression;
using UnityEngine;

namespace MahjongGame.BoardGeneration
{
    public static class DifficultyScalingSystemValidator
    {
        private static readonly int[] SampleLevels = { 1, 9, 10, 50, 100, 300, 1000 };
        private static readonly (int Previous, int Next)[] AdjacentTransitions =
        {
            (1, 2),
            (9, 10),
            (20, 21),
        };

        public static bool Validate(Transform gameplayRoot, StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateComponentWiring(gameplayRoot, reportBuilder);
            passed &= ValidateCurrentLevelResolution(reportBuilder);
            passed &= ValidateSampleLevelProfiles(reportBuilder);
            passed &= ValidateMonotonicSampleProgression(reportBuilder);
            passed &= ValidatePipelineMatchesProfile(reportBuilder);
            passed &= ValidateAdjacentLevelScaling(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Difficulty scaling integration validation completed successfully."
                : "[FAIL] Difficulty scaling integration validation found issues.");

            return passed;
        }

        private static bool ValidateComponentWiring(Transform gameplayRoot, StringBuilder reportBuilder)
        {
            if (gameplayRoot == null)
            {
                AppendLine(reportBuilder, "[SKIP] GameplayRoot is missing; DifficultyScalingController wiring not checked.");
                return true;
            }

            DifficultyScalingController controller = gameplayRoot.GetComponent<DifficultyScalingController>();
            if (controller == null)
            {
                AppendLine(reportBuilder, "[FAIL] DifficultyScalingController is missing on GameplayRoot.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] DifficultyScalingController is present on GameplayRoot.");
            return true;
        }

        private static bool ValidateCurrentLevelResolution(StringBuilder reportBuilder)
        {
            if (!PlayerProgressionDirector.HasInstance || !DifficultyDirector.HasInstance)
            {
                AppendLine(reportBuilder, "[SKIP] Current-level resolution requires PlayerProgressionDirector and DifficultyDirector instances.");
                return true;
            }

            int currentLevel = PlayerProgressionDirector.Instance.CurrentLevel;
            DifficultyProfile explicitProfile = DifficultyDirector.Instance.ResolveProfile(currentLevel);
            if (!DifficultyDirector.Instance.TryResolveProfileForCurrentLevel(out DifficultyProfile currentProfile))
            {
                AppendLine(reportBuilder, "[FAIL] TryResolveProfileForCurrentLevel failed while progression is available.");
                return false;
            }

            if (!ProfilesEqual(explicitProfile, currentProfile))
            {
                AppendLine(reportBuilder, "[FAIL] ResolveProfileForCurrentLevel does not match explicit level resolve.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] DifficultyDirector resolves the current progression level correctly.");
            return true;
        }

        private static bool ValidateSampleLevelProfiles(StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= ValidateSampleLevel(1, profile =>
                profile.ClosedTileCount == 0
                && profile.TileCount >= 80
                && profile.TileCount <= 88
                && profile.JokerCount == 1,
                reportBuilder);

            passed &= ValidateSampleLevel(9, profile =>
                profile.ClosedTileCount == 0,
                reportBuilder);

            passed &= ValidateSampleLevel(10, profile =>
                profile.ClosedTileCount >= 6
                && profile.ClosedTileCount <= 8,
                reportBuilder);

            passed &= ValidateSampleLevel(50, profile =>
                profile.TileCount >= 88
                && profile.TileCount <= 100
                && profile.JokerCount >= 1
                && profile.JokerCount <= 2,
                reportBuilder);

            passed &= ValidateSampleLevel(100, profile =>
                profile.ComplexityTier == ComplexityTier.Mid,
                reportBuilder);

            passed &= ValidateSampleLevel(300, profile =>
                profile.TileCount >= 100
                && profile.TileCount <= 112
                && profile.ClosedTileCount >= 10
                && profile.ClosedTileCount <= 12,
                reportBuilder);

            passed &= ValidateSampleLevel(1000, profile =>
                profile.TileCount >= 112
                && profile.TileCount <= 126
                && profile.LayerDepth <= DifficultyDefinition.MaximumLayerDepth,
                reportBuilder);

            return passed;
        }

        private static bool ValidateSampleLevel(
            int levelNumber,
            System.Func<DifficultyProfile, bool> predicate,
            StringBuilder reportBuilder)
        {
            DifficultyProfile profile = DifficultyDefinition.ResolveProfile(levelNumber);
            if (profile == null || !predicate(profile))
            {
                AppendLine(reportBuilder, "[FAIL] Level " + levelNumber + " difficulty profile failed scaling band checks.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Level " + levelNumber + " difficulty profile matches scaling bands.");
            return true;
        }

        private static bool ValidateMonotonicSampleProgression(StringBuilder reportBuilder)
        {
            DifficultyProfile previousProfile = null;

            for (int index = 0; index < SampleLevels.Length; index++)
            {
                int levelNumber = SampleLevels[index];
                DifficultyProfile profile = DifficultyDefinition.ResolveProfile(levelNumber);

                if (previousProfile != null && !HasNonDecreasingMetrics(previousProfile, profile))
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] Difficulty metrics decreased between sample levels "
                        + previousProfile.LevelNumber
                        + " and "
                        + profile.LevelNumber
                        + ".");
                    return false;
                }

                previousProfile = profile;
            }

            AppendLine(reportBuilder, "[PASS] Difficulty metrics grow across sample progression levels.");
            return true;
        }

        private static bool ValidatePipelineMatchesProfile(StringBuilder reportBuilder)
        {
            bool passed = true;

            for (int index = 0; index < SampleLevels.Length; index++)
            {
                int levelNumber = SampleLevels[index];
                DifficultyProfile profile = DifficultyDefinition.ResolveProfile(levelNumber);
                LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(levelNumber);
                BoardData boardData = BoardGenerationPipeline.GenerateBoardData(levelNumber);

                if (recipe == null
                    || recipe.TileCount != profile.TileCount
                    || recipe.ClosedTileCount != profile.ClosedTileCount
                    || recipe.JokerCount != profile.JokerCount
                    || recipe.LayerDepth != profile.LayerDepth)
                {
                    AppendLine(reportBuilder, "[FAIL] Level " + levelNumber + " recipe does not match difficulty profile.");
                    passed = false;
                    continue;
                }

                if (boardData == null
                    || boardData.TileCount != profile.TileCount
                    || boardData.ClosedTileCount != profile.ClosedTileCount
                    || boardData.JokerCount != profile.JokerCount
                    || boardData.LayerDepth != profile.LayerDepth)
                {
                    AppendLine(reportBuilder, "[FAIL] Level " + levelNumber + " board data does not match difficulty profile.");
                    passed = false;
                    continue;
                }

                AppendLine(reportBuilder, "[PASS] Level " + levelNumber + " recipe and board data match difficulty profile.");
            }

            return passed;
        }

        private static bool ValidateAdjacentLevelScaling(StringBuilder reportBuilder)
        {
            bool passed = true;

            for (int index = 0; index < AdjacentTransitions.Length; index++)
            {
                (int previousLevel, int nextLevel) = AdjacentTransitions[index];
                DifficultyProfile previousProfile = DifficultyDefinition.ResolveProfile(previousLevel);
                DifficultyProfile nextProfile = DifficultyDefinition.ResolveProfile(nextLevel);

                if (!DifficultyDirector.HasDifficultyScaled(previousProfile, nextProfile))
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] Difficulty did not scale from level "
                        + previousLevel
                        + " to "
                        + nextLevel
                        + ".");
                    passed = false;
                    continue;
                }

                AppendLine(
                    reportBuilder,
                    "[PASS] Difficulty scaled from level "
                    + previousLevel
                    + " to "
                    + nextLevel
                    + ".");
            }

            return passed;
        }

        private static bool HasNonDecreasingMetrics(DifficultyProfile previous, DifficultyProfile next)
        {
            return next.TileCount >= previous.TileCount
                && next.ClosedTileCount >= previous.ClosedTileCount
                && next.JokerCount >= previous.JokerCount
                && next.LayerDepth >= previous.LayerDepth
                && next.RecommendedTimerSeconds >= previous.RecommendedTimerSeconds - 0.001f;
        }

        private static bool ProfilesEqual(DifficultyProfile left, DifficultyProfile right)
        {
            return left.LevelNumber == right.LevelNumber
                && left.TileCount == right.TileCount
                && left.ClosedTileCount == right.ClosedTileCount
                && left.JokerCount == right.JokerCount
                && left.LayerDepth == right.LayerDepth
                && Mathf.Approximately(left.RecommendedTimerSeconds, right.RecommendedTimerSeconds)
                && left.ComplexityTier == right.ComplexityTier;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
