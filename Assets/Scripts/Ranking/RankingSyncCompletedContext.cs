namespace MahjongGame.Ranking
{
    public sealed class RankingSyncCompletedContext
    {
        public long UploadedScore { get; }

        public int RemoteEntryCount { get; }

        public bool UsedCachedSnapshot { get; }

        public RankingSyncCompletedContext(long uploadedScore, int remoteEntryCount, bool usedCachedSnapshot)
        {
            UploadedScore = uploadedScore < 0 ? 0 : uploadedScore;
            RemoteEntryCount = remoteEntryCount < 0 ? 0 : remoteEntryCount;
            UsedCachedSnapshot = usedCachedSnapshot;
        }
    }
}
