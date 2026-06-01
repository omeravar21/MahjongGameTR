using System.Reflection;
using System.Text;
using UnityEngine;

namespace MahjongGame.Ranking
{
    public static class RankingSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateTypes(reportBuilder);
            passed &= ValidateEvents(reportBuilder);
            passed &= ValidateDirectorBehavior(reportBuilder);
            passed &= ValidateModelBehavior(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Ranking architecture validation completed successfully."
                : "[FAIL] Ranking architecture validation found issues.");

            return passed;
        }

        private static bool ValidateTypes(StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= ValidateTypeExists(typeof(RankingDirector), reportBuilder);
            passed &= ValidateTypeExists(typeof(RankingData), reportBuilder);
            passed &= ValidateTypeExists(typeof(RankingEntry), reportBuilder);
            passed &= ValidateTypeExists(typeof(LeaderboardData), reportBuilder);
            passed &= ValidateTypeExists(typeof(LeaderboardEntry), reportBuilder);

            return passed;
        }

        private static bool ValidateEvents(StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= ValidateEventExists(
                typeof(RankingEvents),
                nameof(RankingEvents.GlobalPerformanceScoreChanged),
                reportBuilder);
            passed &= ValidateEventExists(
                typeof(RankingEvents),
                nameof(RankingEvents.GlobalRankChanged),
                reportBuilder);

            return passed;
        }

        private static bool ValidateDirectorBehavior(StringBuilder reportBuilder)
        {
            GameObject validationObject = new GameObject("RankingSystemValidator_Temp");
            RankingDirector director = validationObject.AddComponent<RankingDirector>();

            bool passed = true;

            try
            {
                director.SetRankingStateForValidation(25_000, 42);

                if (director.GlobalPerformanceScore != 25_000
                    || director.CurrentGlobalRank != 42
                    || director.HighestGlobalRank != 42)
                {
                    AppendLine(reportBuilder, "[FAIL] RankingDirector did not retain validation ranking state.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] RankingDirector retains global ranking state.");
                }

                RankingEntry localEntry = director.GetLocalRankingEntry();
                if (localEntry == null
                    || !localEntry.IsLocalPlayer
                    || localEntry.GlobalPerformanceScore != 25_000
                    || localEntry.RankPosition != 42)
                {
                    AppendLine(reportBuilder, "[FAIL] RankingDirector local ranking entry is invalid.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] RankingDirector exposes a local ranking entry.");
                }

                LeaderboardData leaderboardData = director.GetLeaderboardData();
                if (leaderboardData == null)
                {
                    AppendLine(reportBuilder, "[FAIL] RankingDirector returned null leaderboard data.");
                    passed = false;
                }
                else if (leaderboardData.Entries == null)
                {
                    AppendLine(reportBuilder, "[FAIL] RankingDirector leaderboard data has null entries.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] RankingDirector exposes leaderboard data architecture.");
                }
            }
            finally
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(validationObject);
                }
                else
                {
                    Object.DestroyImmediate(validationObject);
                }
            }

            return passed;
        }

        private static bool ValidateModelBehavior(StringBuilder reportBuilder)
        {
            bool passed = true;

            RankingData rankingData = RankingData.CreateDefault();
            rankingData.SetGlobalPerformanceScore(10_000);
            rankingData.SetCurrentGlobalRank(15);

            if (rankingData.GlobalPerformanceScore != 10_000
                || rankingData.CurrentGlobalRank != 15
                || rankingData.HighestGlobalRank != 15)
            {
                AppendLine(reportBuilder, "[FAIL] RankingData did not retain ranking values.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] RankingData retains global ranking values.");
            }

            RankingEntry rankingEntry = new RankingEntry("player_1", "Tester", 10_000, 15, true);
            LeaderboardEntry leaderboardEntry = new LeaderboardEntry(15, "Tester", 10_000, true);

            if (rankingEntry.DisplayName != "Tester"
                || leaderboardEntry.DisplayName != "Tester"
                || !rankingEntry.IsLocalPlayer
                || !leaderboardEntry.IsLocalPlayer)
            {
                AppendLine(reportBuilder, "[FAIL] Ranking and leaderboard entry models are invalid.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Ranking and leaderboard entry models are valid.");
            }

            return passed;
        }

        private static bool ValidateTypeExists(System.Type type, StringBuilder reportBuilder)
        {
            if (type == null)
            {
                AppendLine(reportBuilder, "[FAIL] Required ranking type is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] " + type.Name + " type is present.");
            return true;
        }

        private static bool ValidateEventExists(
            System.Type eventOwner,
            string eventName,
            StringBuilder reportBuilder)
        {
            FieldInfo eventBackingField = eventOwner.GetField(
                eventName,
                BindingFlags.Public | BindingFlags.Static);

            if (eventBackingField == null)
            {
                AppendLine(reportBuilder, "[FAIL] Event " + eventName + " is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Event " + eventName + " is present.");
            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
