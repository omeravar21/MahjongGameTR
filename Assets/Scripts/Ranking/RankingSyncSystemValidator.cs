using System.Text;
using MahjongGame.Core;
using MahjongGame.Core.Save;
using UnityEngine;

namespace MahjongGame.Ranking
{
    public static class RankingSyncSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateTypes(reportBuilder);
            passed &= ValidateUploadAndDownload(reportBuilder);
            passed &= ValidateDirectorUsesSyncSnapshot(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Ranking sync validation completed successfully."
                : "[FAIL] Ranking sync validation found issues.");

            return passed;
        }

        private static bool ValidateTypes(StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= ValidateTypeExists(typeof(RankingSyncController), reportBuilder);
            passed &= ValidateTypeExists(typeof(LocalCachedRankingSyncProvider), reportBuilder);
            passed &= ValidateTypeExists(typeof(RankingSyncCompletedContext), reportBuilder);

            return passed;
        }

        private static bool ValidateUploadAndDownload(StringBuilder reportBuilder)
        {
            LocalCachedRankingSyncProvider provider = new LocalCachedRankingSyncProvider();
            RankingSyncSaveData syncSaveData = new RankingSyncSaveData();

            if (!provider.TryUploadScore(150_000, syncSaveData)
                || syncSaveData.lastUploadedScore != 150_000)
            {
                AppendLine(reportBuilder, "[FAIL] Ranking sync upload did not persist uploaded score.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Ranking sync upload persists uploaded score.");

            if (!provider.TryDownloadSnapshot(syncSaveData, out RankingSyncEntrySaveData[] remoteEntries)
                || remoteEntries == null
                || remoteEntries.Length == 0)
            {
                AppendLine(reportBuilder, "[FAIL] Ranking sync download did not return remote entries.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Ranking sync download returns remote entries.");
            return true;
        }

        private static bool ValidateDirectorUsesSyncSnapshot(StringBuilder reportBuilder)
        {
            if (SaveSystem.HasInstance || RankingDirector.HasInstance || RankingSyncController.HasInstance)
            {
                AppendLine(reportBuilder, "[SKIP] Ranking sync director integration requires an isolated editor session.");
                return true;
            }

            GameObject validationObject = new GameObject("RankingSyncSystemValidator_Temp");
            RankingSyncController syncController = validationObject.AddComponent<RankingSyncController>();
            RankingDirector director = validationObject.AddComponent<RankingDirector>();

            bool passed = true;

            try
            {
                RankingSyncSaveData syncSaveData = new RankingSyncSaveData();
                director.SetRankingStateForValidation(250_000, 0);

                if (!syncController.RefreshSyncDataForValidation(syncSaveData, 250_000))
                {
                    AppendLine(reportBuilder, "[FAIL] RankingSyncController refresh failed.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] RankingSyncController refresh succeeds.");
                }

                director.RefreshGlobalRank();

                if (!syncController.HasCachedRemoteSnapshot)
                {
                    AppendLine(reportBuilder, "[FAIL] Ranking sync snapshot was not populated.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] Ranking sync snapshot is populated.");
                }

                LeaderboardData leaderboardData = GlobalLeaderboardBuilder.Build(
                    250_000,
                    GlobalLeaderboardDefinition.LocalPlayerDisplayName,
                    syncController.GetActiveRemoteEntries());

                if (leaderboardData == null
                    || leaderboardData.Entries == null
                    || leaderboardData.Entries.Length == 0
                    || leaderboardData.LocalPlayerRank <= 0)
                {
                    AppendLine(reportBuilder, "[FAIL] Synced leaderboard builder did not produce leaderboard data.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] Synced leaderboard builder produces leaderboard data.");
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

        private static bool ValidateTypeExists(System.Type type, StringBuilder reportBuilder)
        {
            if (type == null)
            {
                AppendLine(reportBuilder, "[FAIL] Required ranking sync type is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] " + type.Name + " type is present.");
            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string line)
        {
            reportBuilder.AppendLine(line);
        }
    }
}
