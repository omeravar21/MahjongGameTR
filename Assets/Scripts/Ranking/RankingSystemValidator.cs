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
            passed &= GlobalPerformanceScoreSystemValidator.Validate(reportBuilder);
            passed &= LeaderboardSystemValidator.Validate(reportBuilder);
            passed &= RankingSyncSystemValidator.Validate(reportBuilder);

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
            passed &= ValidateTypeExists(typeof(GlobalLeaderboardBuilder), reportBuilder);
            passed &= ValidateTypeExists(typeof(RankingUIController), reportBuilder);
            passed &= ValidateTypeExists(typeof(RankingSyncController), reportBuilder);

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
            passed &= ValidateEventExists(
                typeof(RankingEvents),
                nameof(RankingEvents.RankingSyncCompleted),
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
                director.SetRankingStateForValidation(250_000, 0);
                director.RefreshGlobalRank();

                LeaderboardData expectedLeaderboard = director.GetLeaderboardData();
                int expectedRank = expectedLeaderboard.LocalPlayerRank;

                if (director.GlobalPerformanceScore != 250_000
                    || director.CurrentGlobalRank != expectedRank
                    || expectedRank <= 0)
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
                    || localEntry.GlobalPerformanceScore != 250_000
                    || localEntry.RankPosition != expectedRank)
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
                else if (leaderboardData.Entries == null || leaderboardData.Entries.Length == 0)
                {
                    AppendLine(reportBuilder, "[FAIL] RankingDirector leaderboard data has no entries.");
                    passed = false;
                }
                else if (leaderboardData.LocalPlayerRank != expectedRank)
                {
                    AppendLine(reportBuilder, "[FAIL] RankingDirector cached leaderboard does not reflect validation rank.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] RankingDirector exposes populated leaderboard data.");
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
            System.Reflection.EventInfo eventInfo = eventOwner.GetEvent(
                eventName,
                BindingFlags.Public | BindingFlags.Static);

            if (eventInfo == null)
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
