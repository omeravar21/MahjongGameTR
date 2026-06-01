namespace MahjongGame.DailyBoard
{
    public static class DailyBoardRewardDefinition
    {
        public const int CompletionGlobalPerformanceScore = 2500;

        public const int CompletionShuffleBoosterReward = 1;

        public const int CompletionUndoBoosterReward = 1;

        public const int CompletionHintBoosterReward = 1;

        public static int GetCompletionGlobalPerformanceScore()
        {
            return CompletionGlobalPerformanceScore;
        }

        public static int GetCompletionShuffleBoosterReward()
        {
            return CompletionShuffleBoosterReward;
        }

        public static int GetCompletionUndoBoosterReward()
        {
            return CompletionUndoBoosterReward;
        }

        public static int GetCompletionHintBoosterReward()
        {
            return CompletionHintBoosterReward;
        }
    }
}
