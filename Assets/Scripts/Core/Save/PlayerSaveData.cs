using System;

namespace MahjongGame.Core.Save
{
    [Serializable]
    public sealed class PlayerSaveData
    {
        public const int CurrentSaveVersion = 1;

        public int saveVersion = CurrentSaveVersion;
        public int currentLevel = 1;
        public int highestLevel = 1;
        public long globalPerformanceScore;
        public int highestGlobalRank;
        public int totalLevelsCompleted;
        public string activeSymbolSetId = string.Empty;
        public BoosterCountsSaveData boosterCounts = new BoosterCountsSaveData();
        public GameSettingsSaveData settings = new GameSettingsSaveData();
        public AudioSettingsSaveData audioSettings = new AudioSettingsSaveData();
        public StatisticsSaveData statistics = new StatisticsSaveData();
        public ActiveLevelStateSaveData activeLevelState = new ActiveLevelStateSaveData();
        public RankingSyncSaveData rankingSync = new RankingSyncSaveData();
        public DailyBoardSaveData dailyBoard = new DailyBoardSaveData();

        public static PlayerSaveData CreateDefault()
        {
            return new PlayerSaveData();
        }

        public void EnsureDefaults()
        {
            saveVersion = CurrentSaveVersion;
            boosterCounts ??= new BoosterCountsSaveData();
            settings ??= new GameSettingsSaveData();
            audioSettings ??= new AudioSettingsSaveData();
            statistics ??= new StatisticsSaveData();
            activeLevelState ??= new ActiveLevelStateSaveData();
            rankingSync ??= new RankingSyncSaveData();
            rankingSync.EnsureDefaults();
            dailyBoard ??= new DailyBoardSaveData();
            dailyBoard.EnsureDefaults();
        }
    }
}