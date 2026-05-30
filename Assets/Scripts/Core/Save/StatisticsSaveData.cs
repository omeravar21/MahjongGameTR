using System;

namespace MahjongGame.Core.Save
{
    [Serializable]
    public sealed class StatisticsSaveData
    {
        public int currentGlobalRank;
        public int highestGlobalRank;
        public long totalPlayTimeSeconds;
        public int levelsCompleted;
        public int levelsFailed;
        public int highestCombo;
        public int perfectClears;
        public int boostersUsed;
        public int adsWatched;
        public long totalScoreEarned;
    }
}