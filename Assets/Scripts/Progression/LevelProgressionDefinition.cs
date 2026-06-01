namespace MahjongGame.Progression
{
    public static class LevelProgressionDefinition
    {
        public static int GetNextLevelNumber(int completedLevelNumber)
        {
            return LevelProgressData.ClampLevel(completedLevelNumber + 1);
        }

        public static bool CanAdvanceFrom(int currentLevelNumber)
        {
            int clampedLevel = LevelProgressData.ClampLevel(currentLevelNumber);
            return clampedLevel >= LevelProgressData.MinLevel
                && clampedLevel < LevelProgressData.MaxLevel;
        }

        public static bool IsMaxLevel(int levelNumber)
        {
            return LevelProgressData.ClampLevel(levelNumber) >= LevelProgressData.MaxLevel;
        }
    }
}
