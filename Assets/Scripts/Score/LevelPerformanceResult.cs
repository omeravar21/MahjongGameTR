namespace MahjongGame.Score
{
    public sealed class LevelPerformanceResult
    {
        public int GameplayScore { get; }

        public int TimePerformanceBonus { get; }

        public int PerfectClearBonus { get; }

        public int NoBoosterBonus { get; }

        public int TotalPerformanceScore { get; }

        public LevelPerformanceResult(
            int gameplayScore,
            int timePerformanceBonus,
            int perfectClearBonus,
            int noBoosterBonus)
        {
            GameplayScore = gameplayScore < 0 ? 0 : gameplayScore;
            TimePerformanceBonus = timePerformanceBonus < 0 ? 0 : timePerformanceBonus;
            PerfectClearBonus = perfectClearBonus < 0 ? 0 : perfectClearBonus;
            NoBoosterBonus = noBoosterBonus < 0 ? 0 : noBoosterBonus;
            TotalPerformanceScore = GameplayScore
                + TimePerformanceBonus
                + PerfectClearBonus
                + NoBoosterBonus;
        }
    }
}
