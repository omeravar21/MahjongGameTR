using MahjongGame.Core.Save;

namespace MahjongGame.Progression
{
    public sealed class PlayerProgressData
    {
        public int CurrentLevel { get; private set; } = LevelProgressData.MinLevel;
        public int HighestLevel { get; private set; } = LevelProgressData.MinLevel;
        public long GlobalPerformanceScore { get; private set; }
        public LevelProgressData CurrentLevelData { get; private set; } = new LevelProgressData();

        public static PlayerProgressData FromSave(PlayerSaveData saveData)
        {
            PlayerProgressData progressData = new PlayerProgressData();

            if (saveData == null)
            {
                return progressData;
            }

            progressData.ApplyFromSave(saveData);
            return progressData;
        }

        public void ApplyFromSave(PlayerSaveData saveData)
        {
            CurrentLevel = LevelProgressData.ClampLevel(saveData.currentLevel);
            HighestLevel = LevelProgressData.ClampLevel(saveData.highestLevel);
            GlobalPerformanceScore = saveData.globalPerformanceScore;
            CurrentLevelData = new LevelProgressData(CurrentLevel);
        }

        public void WriteToSave(PlayerSaveData saveData)
        {
            if (saveData == null)
            {
                return;
            }

            saveData.currentLevel = CurrentLevel;
            saveData.highestLevel = HighestLevel;
            saveData.globalPerformanceScore = GlobalPerformanceScore;
        }

        public void SetCurrentLevel(int levelNumber)
        {
            CurrentLevel = LevelProgressData.ClampLevel(levelNumber);
            CurrentLevelData = new LevelProgressData(CurrentLevel);

            if (CurrentLevel > HighestLevel)
            {
                HighestLevel = CurrentLevel;
            }
        }

        public void SetGlobalPerformanceScore(long score)
        {
            GlobalPerformanceScore = score < 0 ? 0 : score;
        }

        public void MarkCurrentLevelCompleted()
        {
            CurrentLevelData.isCompleted = true;
            CurrentLevelData.completionCount++;
        }
    }
}