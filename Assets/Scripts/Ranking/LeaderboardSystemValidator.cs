using System.Text;
using MahjongGame.Core;
using UnityEngine;

namespace MahjongGame.Ranking
{
    public static class LeaderboardSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateTypes(reportBuilder);
            passed &= ValidateReferenceEntries(reportBuilder);
            passed &= ValidateBuilderRanks(reportBuilder);
            passed &= ValidateDirectorRefresh(reportBuilder);
            passed &= ValidateRankImprovesWithScore(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Global leaderboard validation completed successfully."
                : "[FAIL] Global leaderboard validation found issues.");

            return passed;
        }

        private static bool ValidateTypes(StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= ValidateTypeExists(typeof(GlobalLeaderboardDefinition), reportBuilder);
            passed &= ValidateTypeExists(typeof(GlobalLeaderboardBuilder), reportBuilder);
            passed &= ValidateTypeExists(typeof(RankingUIController), reportBuilder);

            return passed;
        }

        private static bool ValidateReferenceEntries(StringBuilder reportBuilder)
        {
            if (!GlobalLeaderboardDefinition.AreReferenceEntriesSortedDescending())
            {
                AppendLine(reportBuilder, "[FAIL] Reference leaderboard entries are not sorted descending.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Reference leaderboard entries are sorted descending.");
            return true;
        }

        private static bool ValidateBuilderRanks(StringBuilder reportBuilder)
        {
            bool passed = true;

            LeaderboardData zeroScoreData = GlobalLeaderboardBuilder.Build(0, GlobalLeaderboardDefinition.LocalPlayerDisplayName);
            if (zeroScoreData.LocalPlayerRank <= 0
                || zeroScoreData.Entries == null
                || zeroScoreData.Entries.Length == 0)
            {
                AppendLine(reportBuilder, "[FAIL] Zero-score local player was not placed on the leaderboard.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Zero-score local player receives a global position.");
            }

            LeaderboardData midScoreData = GlobalLeaderboardBuilder.Build(200_000, GlobalLeaderboardDefinition.LocalPlayerDisplayName);
            if (midScoreData.LocalPlayerRank <= 0 || midScoreData.LocalPlayerRank > 31)
            {
                AppendLine(reportBuilder, "[FAIL] Mid-score local player rank is outside the expected range.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Mid-score local player rank resolves correctly.");
            }

            LeaderboardData topScoreData = GlobalLeaderboardBuilder.Build(6_000_000, GlobalLeaderboardDefinition.LocalPlayerDisplayName);
            if (topScoreData.LocalPlayerRank != 1
                || topScoreData.Entries[0].RankPosition != 1
                || !topScoreData.Entries[0].IsLocalPlayer)
            {
                AppendLine(reportBuilder, "[FAIL] Top-score local player is not ranked first.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Top-score local player is ranked first.");
            }

            passed &= ValidateEntriesSortedDescending(topScoreData, reportBuilder);

            return passed;
        }

        private static bool ValidateDirectorRefresh(StringBuilder reportBuilder)
        {
            if (SaveSystem.HasInstance || RankingDirector.HasInstance)
            {
                AppendLine(reportBuilder, "[SKIP] RankingDirector refresh requires an isolated editor session.");
                return true;
            }

            GameObject validationObject = new GameObject("LeaderboardSystemValidator_Ranking");
            SaveSystem saveSystem = validationObject.AddComponent<SaveSystem>();
            RankingDirector rankingDirector = validationObject.AddComponent<RankingDirector>();

            bool passed = true;

            try
            {
                saveSystem.ResetToDefaults();
                rankingDirector.SetRankingStateForValidation(150_000, 0);
                rankingDirector.RefreshGlobalRank();

                LeaderboardData leaderboardData = rankingDirector.GetLeaderboardData();
                if (leaderboardData.LocalPlayerRank <= 0
                    || rankingDirector.CurrentGlobalRank != leaderboardData.LocalPlayerRank)
                {
                    AppendLine(reportBuilder, "[FAIL] RankingDirector refresh did not update current global rank.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] RankingDirector refresh updates current global rank.");
                }

                rankingDirector.LoadFromSave();
                if (rankingDirector.CurrentGlobalRank != leaderboardData.LocalPlayerRank)
                {
                    AppendLine(reportBuilder, "[FAIL] RankingDirector did not persist refreshed global rank.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] RankingDirector persists refreshed global rank.");
                }
            }
            finally
            {
                DestroyValidationObject(validationObject);
            }

            return passed;
        }

        private static bool ValidateRankImprovesWithScore(StringBuilder reportBuilder)
        {
            LeaderboardData lowerScoreData = GlobalLeaderboardBuilder.Build(50_000, GlobalLeaderboardDefinition.LocalPlayerDisplayName);
            LeaderboardData higherScoreData = GlobalLeaderboardBuilder.Build(500_000, GlobalLeaderboardDefinition.LocalPlayerDisplayName);

            if (higherScoreData.LocalPlayerRank <= 0
                || lowerScoreData.LocalPlayerRank <= 0
                || higherScoreData.LocalPlayerRank >= lowerScoreData.LocalPlayerRank)
            {
                AppendLine(reportBuilder, "[FAIL] Higher GPS did not improve global rank position.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Higher GPS improves global rank position.");
            return true;
        }

        private static bool ValidateEntriesSortedDescending(LeaderboardData leaderboardData, StringBuilder reportBuilder)
        {
            if (leaderboardData?.Entries == null || leaderboardData.Entries.Length < 2)
            {
                AppendLine(reportBuilder, "[PASS] Leaderboard entry list is present.");
                return true;
            }

            for (int index = 1; index < leaderboardData.Entries.Length; index++)
            {
                if (leaderboardData.Entries[index].GlobalPerformanceScore
                    > leaderboardData.Entries[index - 1].GlobalPerformanceScore)
                {
                    AppendLine(reportBuilder, "[FAIL] Displayed leaderboard entries are not sorted by score.");
                    return false;
                }
            }

            AppendLine(reportBuilder, "[PASS] Displayed leaderboard entries are sorted by score.");
            return true;
        }

        private static bool ValidateTypeExists(System.Type type, StringBuilder reportBuilder)
        {
            if (type == null)
            {
                AppendLine(reportBuilder, "[FAIL] Required leaderboard type is missing.");
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
