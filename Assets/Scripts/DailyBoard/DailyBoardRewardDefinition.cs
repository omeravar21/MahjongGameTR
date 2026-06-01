namespace MahjongGame.DailyBoard
{
    public static class DailyBoardRewardDefinition
    {
        public const int CompletionGlobalPerformanceScore = 2500;

        public const int CompletionShuffleBoosterReward = 0;

        public const int CompletionUndoBoosterReward = 0;

        public const int CompletionHintBoosterReward = 0;

        public static int GetCompletionGlobalPerformanceScore()
        {
            return CompletionGlobalPerformanceScore;
        }
    }
}
