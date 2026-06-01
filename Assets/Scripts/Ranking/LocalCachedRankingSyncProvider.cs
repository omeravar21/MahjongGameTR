using System;
using MahjongGame.Core.Save;

namespace MahjongGame.Ranking
{
    public sealed class LocalCachedRankingSyncProvider : IRankingSyncProvider
    {
        public bool TryUploadScore(long globalPerformanceScore, RankingSyncSaveData syncSaveData)
        {
            if (syncSaveData == null)
            {
                return false;
            }

            syncSaveData.EnsureDefaults();

            long normalizedScore = globalPerformanceScore < 0 ? 0 : globalPerformanceScore;
            syncSaveData.pendingUploadScore = normalizedScore;

            if (normalizedScore >= syncSaveData.lastUploadedScore)
            {
                syncSaveData.lastUploadedScore = normalizedScore;
                syncSaveData.pendingUploadScore = 0;
            }

            return true;
        }

        public bool TryDownloadSnapshot(RankingSyncSaveData syncSaveData, out RankingSyncEntrySaveData[] remoteEntries)
        {
            if (syncSaveData == null)
            {
                remoteEntries = Array.Empty<RankingSyncEntrySaveData>();
                return false;
            }

            syncSaveData.EnsureDefaults();

            if (syncSaveData.cachedRemoteEntries.Length == 0)
            {
                syncSaveData.cachedRemoteEntries = BuildInitialCacheFromReferences();
            }

            syncSaveData.lastDownloadUtcTicks = DateTime.UtcNow.Ticks;
            remoteEntries = syncSaveData.cachedRemoteEntries;
            return remoteEntries.Length > 0;
        }

        private static RankingSyncEntrySaveData[] BuildInitialCacheFromReferences()
        {
            RankingSyncEntrySaveData[] entries = new RankingSyncEntrySaveData[GlobalLeaderboardDefinition.ReferenceEntryCount];

            for (int index = 0; index < GlobalLeaderboardDefinition.ReferenceEntryCount; index++)
            {
                (string displayName, long score) = GlobalLeaderboardDefinition.GetReferenceEntry(index);
                entries[index] = new RankingSyncEntrySaveData
                {
                    displayName = displayName,
                    globalPerformanceScore = score
                };
            }

            return entries;
        }
    }
}
