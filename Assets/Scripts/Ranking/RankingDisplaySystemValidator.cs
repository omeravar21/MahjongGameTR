using System.Text;
using UnityEngine;

namespace MahjongGame.Ranking
{
    public static class RankingDisplaySystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateTypeExists(reportBuilder);
            passed &= ValidateControllerLayout(reportBuilder);
            passed &= ValidateControllerRendering(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Ranking display validation completed successfully."
                : "[FAIL] Ranking display validation found issues.");

            return passed;
        }

        private static bool ValidateTypeExists(StringBuilder reportBuilder)
        {
            if (typeof(RankingUIController) == null)
            {
                AppendLine(reportBuilder, "[FAIL] RankingUIController type is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] RankingUIController type is present.");
            return true;
        }

        private static bool ValidateControllerLayout(StringBuilder reportBuilder)
        {
            GameObject validationObject = new GameObject("RankingDisplayValidator_Temp");
            validationObject.SetActive(false);
            RankingUIController uiController = validationObject.AddComponent<RankingUIController>();

            bool passed = true;

            try
            {
                if (uiController.HasRequiredLayout())
                {
                    AppendLine(reportBuilder, "[FAIL] RankingUIController should start without required layout.");
                    passed = false;
                }

                uiController.BuildLayout();

                if (!uiController.HasRequiredLayout())
                {
                    AppendLine(reportBuilder, "[FAIL] RankingUIController did not build required layout.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] RankingUIController builds required layout.");
                }
            }
            finally
            {
                DestroyValidationObject(validationObject);
            }

            return passed;
        }

        private static bool ValidateControllerRendering(StringBuilder reportBuilder)
        {
            GameObject validationObject = new GameObject("RankingDisplayValidator_Temp");
            RankingUIController uiController = validationObject.AddComponent<RankingUIController>();

            bool passed = true;

            try
            {
                uiController.BuildLayout();

                LeaderboardEntry[] entries =
                {
                    new LeaderboardEntry(1, "Alpha", 500_000, false),
                    new LeaderboardEntry(2, "Player", 250_000, true),
                    new LeaderboardEntry(3, "Beta", 100_000, false),
                };

                LeaderboardData leaderboardData = new LeaderboardData(entries, 2, 250_000);
                uiController.RefreshFromLeaderboardDataForValidation(leaderboardData);

                string summaryText = uiController.GetSummaryTextForValidation();
                if (string.IsNullOrEmpty(summaryText)
                    || summaryText.IndexOf("Your Rank: 2", System.StringComparison.Ordinal) < 0
                    || summaryText.IndexOf("Your Score: 250000", System.StringComparison.Ordinal) < 0)
                {
                    AppendLine(reportBuilder, "[FAIL] RankingUIController summary text is invalid.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] RankingUIController summary text reflects local rank and score.");
                }

                if (uiController.GetEntryRowCountForValidation() != entries.Length)
                {
                    AppendLine(reportBuilder, "[FAIL] RankingUIController did not render all leaderboard entry rows.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] RankingUIController renders leaderboard entry rows.");
                }
            }
            finally
            {
                DestroyValidationObject(validationObject);
            }

            return passed;
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
