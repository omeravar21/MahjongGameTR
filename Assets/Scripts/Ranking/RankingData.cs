using MahjongGame.Core.Save;

namespace MahjongGame.Ranking
{
    public sealed class RankingData
    {
        public long GlobalPerformanceScore { get; private set; }

        public int CurrentGlobalRank { get; private set; }

        public int HighestGlobalRank { get; private set; }

        public static RankingData CreateDefault()
        {
            return new RankingData();
        }

        public static RankingData FromSave(PlayerSaveData saveData)
        {
            RankingData rankingData = new RankingData();

            if (saveData == null)
            {
                return rankingData;
            }

            rankingData.ApplyFromSave(saveData);
            return rankingData;
        }

        public void ApplyFromSave(PlayerSaveData saveData)
        {
            GlobalPerformanceScore = saveData.globalPerformanceScore < 0 ? 0 : saveData.globalPerformanceScore;
            HighestGlobalRank = saveData.highestGlobalRank < 0 ? 0 : saveData.highestGlobalRank;
            CurrentGlobalRank = HighestGlobalRank;
        }

        public void WriteToSave(PlayerSaveData saveData)
        {
            if (saveData == null)
            {
                return;
            }

            saveData.globalPerformanceScore = GlobalPerformanceScore;
            saveData.highestGlobalRank = HighestGlobalRank;
        }

        public void SetGlobalPerformanceScore(long score)
        {
            GlobalPerformanceScore = score < 0 ? 0 : score;
        }

        public void SetCurrentGlobalRank(int rankPosition)
        {
            CurrentGlobalRank = rankPosition < 0 ? 0 : rankPosition;

            if (CurrentGlobalRank > 0
                && (HighestGlobalRank <= 0 || CurrentGlobalRank < HighestGlobalRank))
            {
                HighestGlobalRank = CurrentGlobalRank;
            }
        }
    }
}
