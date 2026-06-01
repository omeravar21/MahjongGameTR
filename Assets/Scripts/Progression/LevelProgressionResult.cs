namespace MahjongGame.Progression
{
    public readonly struct LevelProgressionResult
    {
        public bool Success { get; }

        public int PreviousLevel { get; }

        public int NewLevel { get; }

        public bool IsMaxLevelReached { get; }

        private LevelProgressionResult(
            bool success,
            int previousLevel,
            int newLevel,
            bool isMaxLevelReached)
        {
            Success = success;
            PreviousLevel = previousLevel;
            NewLevel = newLevel;
            IsMaxLevelReached = isMaxLevelReached;
        }

        public static LevelProgressionResult Advanced(int previousLevel, int newLevel)
        {
            return new LevelProgressionResult(
                success: true,
                previousLevel: previousLevel,
                newLevel: newLevel,
                isMaxLevelReached: LevelProgressionDefinition.IsMaxLevel(newLevel));
        }

        public static LevelProgressionResult Completed(int completedLevel)
        {
            return new LevelProgressionResult(
                success: true,
                previousLevel: completedLevel,
                newLevel: completedLevel,
                isMaxLevelReached: LevelProgressionDefinition.IsMaxLevel(completedLevel));
        }

        public static LevelProgressionResult FailedAtMaxLevel(int currentLevel)
        {
            int clampedLevel = LevelProgressData.ClampLevel(currentLevel);
            return new LevelProgressionResult(
                success: false,
                previousLevel: clampedLevel,
                newLevel: clampedLevel,
                isMaxLevelReached: true);
        }
    }
}
