using MahjongGame.Core.Save;

namespace MahjongGame.Ranking
{
    public interface IRankingSyncProvider
    {
        bool TryUploadScore(long globalPerformanceScore, RankingSyncSaveData syncSaveData);

        bool TryDownloadSnapshot(RankingSyncSaveData syncSaveData, out RankingSyncEntrySaveData[] remoteEntries);
    }
}
