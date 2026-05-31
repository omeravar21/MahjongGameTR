namespace MahjongGame.UI
{
    public sealed class LevelResultSummary
    {
        public int LevelNumber { get; }

        public float CompletionTimeSeconds { get; }

        public int Score { get; }

        public int TotalComboCount { get; }

        public LevelResultSummary(int levelNumber, float completionTimeSeconds, int score, int totalComboCount)
        {
            LevelNumber = levelNumber;
            CompletionTimeSeconds = completionTimeSeconds < 0f ? 0f : completionTimeSeconds;
            Score = score < 0 ? 0 : score;
            TotalComboCount = totalComboCount < 0 ? 0 : totalComboCount;
        }
    }
}
