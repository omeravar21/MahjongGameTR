using System;

namespace MahjongGame.Core.Save
{
    [Serializable]
    public sealed class RankingSyncEntrySaveData
    {
        public string displayName = string.Empty;
        public long globalPerformanceScore;
    }

    [Serializable]
    public sealed class RankingSyncSaveData
    {
        public long lastUploadedScore;
        public long pendingUploadScore;
        public long lastDownloadUtcTicks;
        public RankingSyncEntrySaveData[] cachedRemoteEntries = Array.Empty<RankingSyncEntrySaveData>();

        public void EnsureDefaults()
        {
            cachedRemoteEntries ??= Array.Empty<RankingSyncEntrySaveData>();
        }
    }
}
