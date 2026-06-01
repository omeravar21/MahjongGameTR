namespace MahjongGame.UI
{
    public sealed class LevelResultSummary
    {
        public int LevelNumber { get; }

        public float CompletionTimeSeconds { get; }

        public int Score { get; }

        public int TotalComboCount { get; }

        public int HighestCombo { get; }

        public int EarlyJokerMatchCount { get; }

        public int JokerBonusTotal { get; }

        public int TimePerformanceBonus { get; }

        public int PerfectClearBonus { get; }

        public int NoBoosterBonus { get; }

        public int GlobalPerformanceScoreEarned { get; }

        public LevelResultSummary(
            int levelNumber,
            float completionTimeSeconds,
            int score,
            int totalComboCount,
            int earlyJokerMatchCount = 0,
            int jokerBonusTotal = 0,
            int highestCombo = 0,
            int timePerformanceBonus = 0,
            int perfectClearBonus = 0,
            int noBoosterBonus = 0,
            int globalPerformanceScoreEarned = 0)
        {
            LevelNumber = levelNumber;
            CompletionTimeSeconds = completionTimeSeconds < 0f ? 0f : completionTimeSeconds;
            Score = score < 0 ? 0 : score;
            TotalComboCount = totalComboCount < 0 ? 0 : totalComboCount;
            HighestCombo = highestCombo < 0 ? 0 : highestCombo;
            EarlyJokerMatchCount = earlyJokerMatchCount < 0 ? 0 : earlyJokerMatchCount;
            JokerBonusTotal = jokerBonusTotal < 0 ? 0 : jokerBonusTotal;
            TimePerformanceBonus = timePerformanceBonus < 0 ? 0 : timePerformanceBonus;
            PerfectClearBonus = perfectClearBonus < 0 ? 0 : perfectClearBonus;
            NoBoosterBonus = noBoosterBonus < 0 ? 0 : noBoosterBonus;
            GlobalPerformanceScoreEarned = globalPerformanceScoreEarned < 0 ? 0 : globalPerformanceScoreEarned;
        }
    }
}
