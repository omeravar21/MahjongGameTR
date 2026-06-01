using System.Reflection;
using System.Text;
using MahjongGame.Core;
using MahjongGame.Score;
using UnityEngine;

namespace MahjongGame.Ranking
{
    public static class GlobalPerformanceScoreSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateTypes(reportBuilder);
            passed &= ValidateTimePerformanceBonus(reportBuilder);
            passed &= ValidateLevelPerformanceTotals(reportBuilder);
            passed &= ValidateScoreControllerCalculation(reportBuilder);
            passed &= ValidateAccumulationBehavior(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Global performance score validation completed successfully."
                : "[FAIL] Global performance score validation found issues.");

            return passed;
        }

        private static bool ValidateTypes(StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= ValidateTypeExists(typeof(LevelPerformanceResult), reportBuilder);

            if (typeof(ScoreDefinition).GetMethod(
                    nameof(ScoreDefinition.ResolveTimePerformanceBonus),
                    BindingFlags.Public | BindingFlags.Static)
                == null)
            {
                AppendLine(reportBuilder, "[FAIL] ScoreDefinition.ResolveTimePerformanceBonus is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] ScoreDefinition.ResolveTimePerformanceBonus is present.");
            }

            return passed;
        }

        private static bool ValidateTimePerformanceBonus(StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= AssertBonus(
                ScoreDefinition.ResolveTimePerformanceBonus(40f, 100f),
                ScoreDefinition.TimePerformanceBonusWithin40Percent,
                "40% time tier",
                reportBuilder);
            passed &= AssertBonus(
                ScoreDefinition.ResolveTimePerformanceBonus(60f, 100f),
                ScoreDefinition.TimePerformanceBonusWithin60Percent,
                "60% time tier",
                reportBuilder);
            passed &= AssertBonus(
                ScoreDefinition.ResolveTimePerformanceBonus(80f, 100f),
                ScoreDefinition.TimePerformanceBonusWithin80Percent,
                "80% time tier",
                reportBuilder);
            passed &= AssertBonus(
                ScoreDefinition.ResolveTimePerformanceBonus(100f, 100f),
                ScoreDefinition.TimePerformanceBonusWithin100Percent,
                "100% time tier",
                reportBuilder);
            passed &= AssertBonus(
                ScoreDefinition.ResolveTimePerformanceBonus(110f, 100f),
                0,
                "over-time tier",
                reportBuilder);
            passed &= AssertBonus(
                ScoreDefinition.ResolveTimePerformanceBonus(10f, 0f),
                0,
                "zero allocated time",
                reportBuilder);

            return passed;
        }

        private static bool ValidateLevelPerformanceTotals(StringBuilder reportBuilder)
        {
            LevelPerformanceResult result = new LevelPerformanceResult(
                ScoreDefinition.BaseMatchScore * 3 + ScoreDefinition.ResolveComboBonus(2),
                ScoreDefinition.TimePerformanceBonusWithin60Percent,
                ScoreDefinition.PerfectClearBonus,
                ScoreDefinition.NoBoosterBonus);

            int expectedTotal = result.GameplayScore
                + result.TimePerformanceBonus
                + result.PerfectClearBonus
                + result.NoBoosterBonus;

            if (result.TotalPerformanceScore != expectedTotal)
            {
                AppendLine(reportBuilder, "[FAIL] LevelPerformanceResult total does not equal component sum.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] LevelPerformanceResult totals combine all performance components.");
            return true;
        }

        private static bool ValidateScoreControllerCalculation(StringBuilder reportBuilder)
        {
            GameObject validationObject = new GameObject("GlobalPerformanceScoreValidator_Score");
            ScoreController scoreController = validationObject.AddComponent<ScoreController>();

            bool passed = true;

            try
            {
                scoreController.AwardBaseMatchScoreForValidation();
                scoreController.AwardComboBonusForValidation(2);

                LevelPerformanceResult result = scoreController.CalculateLevelPerformanceResult(
                    50f,
                    100f,
                    isPerfectClear: true);

                int expectedGameplayScore = ScoreDefinition.BaseMatchScore + ScoreDefinition.ResolveComboBonus(2);
                if (result.GameplayScore != expectedGameplayScore
                    || result.TimePerformanceBonus != ScoreDefinition.TimePerformanceBonusWithin60Percent
                    || result.PerfectClearBonus != ScoreDefinition.PerfectClearBonus
                    || result.NoBoosterBonus != ScoreDefinition.NoBoosterBonus)
                {
                    AppendLine(reportBuilder, "[FAIL] ScoreController level performance calculation is incorrect.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] ScoreController calculates level performance totals correctly.");
                }
            }
            finally
            {
                DestroyValidationObject(validationObject);
            }

            return passed;
        }

        private static bool ValidateAccumulationBehavior(StringBuilder reportBuilder)
        {
            if (SaveSystem.HasInstance || RankingDirector.HasInstance)
            {
                AppendLine(
                    reportBuilder,
                    "[SKIP] SaveSystem accumulation requires an isolated editor session.");
                return true;
            }

            GameObject validationObject = new GameObject("GlobalPerformanceScoreValidator_Ranking");
            SaveSystem saveSystem = validationObject.AddComponent<SaveSystem>();
            RankingDirector rankingDirector = validationObject.AddComponent<RankingDirector>();

            bool passed = true;
            bool eventRaised = false;

            void HandleScoreChanged(GlobalPerformanceScoreChangedContext context)
            {
                if (context != null && context.NewScore > context.PreviousScore)
                {
                    eventRaised = true;
                }
            }

            RankingEvents.GlobalPerformanceScoreChanged += HandleScoreChanged;

            try
            {
                saveSystem.ResetToDefaults();

                LevelPerformanceResult firstResult = new LevelPerformanceResult(3000, 8000, 10000, 5000);
                LevelPerformanceResult secondResult = new LevelPerformanceResult(1000, 3000, 10000, 0);

                if (!rankingDirector.TryAccumulateLevelPerformance(firstResult, out long firstPrevious, out long firstNew)
                    || firstPrevious != 0
                    || firstNew != firstResult.TotalPerformanceScore)
                {
                    AppendLine(reportBuilder, "[FAIL] RankingDirector first accumulation failed.");
                    return false;
                }

                if (!rankingDirector.TryAccumulateLevelPerformance(secondResult, out long secondPrevious, out long secondNew)
                    || secondPrevious != firstNew
                    || secondNew != firstPrevious + secondResult.TotalPerformanceScore)
                {
                    AppendLine(reportBuilder, "[FAIL] RankingDirector second accumulation failed.");
                    return false;
                }

                rankingDirector.LoadFromSave();

                if (rankingDirector.GlobalPerformanceScore != secondNew)
                {
                    AppendLine(reportBuilder, "[FAIL] RankingDirector did not reload accumulated global performance score.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] RankingDirector persists accumulated global performance score.");
                }

                if (!eventRaised)
                {
                    AppendLine(reportBuilder, "[FAIL] GlobalPerformanceScoreChanged event was not raised.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] GlobalPerformanceScoreChanged event is raised on accumulation.");
                }
            }
            finally
            {
                RankingEvents.GlobalPerformanceScoreChanged -= HandleScoreChanged;
                DestroyValidationObject(validationObject);
            }

            return passed;
        }

        private static bool AssertBonus(int actual, int expected, string label, StringBuilder reportBuilder)
        {
            if (actual != expected)
            {
                AppendLine(reportBuilder, "[FAIL] " + label + " bonus resolved to " + actual + " instead of " + expected + ".");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] " + label + " bonus resolves correctly.");
            return true;
        }

        private static bool ValidateTypeExists(System.Type type, StringBuilder reportBuilder)
        {
            if (type == null)
            {
                AppendLine(reportBuilder, "[FAIL] Required global performance score type is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] " + type.Name + " type is present.");
            return true;
        }

        private static void DestroyValidationObject(GameObject validationObject)
        {
            if (validationObject == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(validationObject);
            }
            else
            {
                Object.DestroyImmediate(validationObject);
            }
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
