using MahjongGame.Core;
using MahjongGame.Core.Save;
using UnityEngine;

namespace MahjongGame.Progression
{
    public sealed class PlayerProgressionDirector : MonoBehaviour
    {
        private static PlayerProgressionDirector _instance;

        private PlayerProgressData _progressData = new PlayerProgressData();

        public static PlayerProgressionDirector Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[PlayerProgressionDirector] Instance is not available.");
                }

                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        public PlayerProgressData ProgressData => _progressData;

        public int CurrentLevel => _progressData.CurrentLevel;

        public int HighestLevel => _progressData.HighestLevel;

        public long GlobalPerformanceScore => _progressData.GlobalPerformanceScore;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            LoadFromSave();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public void LoadFromSave()
        {
            if (!SaveSystem.HasInstance)
            {
                Debug.LogWarning("[PlayerProgressionDirector] SaveSystem is not ready.");
                return;
            }

            _progressData = PlayerProgressData.FromSave(SaveSystem.Instance.Data);
        }

        public void SetCurrentLevel(int levelNumber)
        {
            _progressData.SetCurrentLevel(levelNumber);
            PersistProgress();
        }

        public void SetGlobalPerformanceScore(long score)
        {
            _progressData.SetGlobalPerformanceScore(score);
            PersistProgress();
        }

        public LevelProgressData GetCurrentLevelData()
        {
            return new LevelProgressData(_progressData.CurrentLevel);
        }

        public int GetNextLevelNumber()
        {
            return LevelProgressionDefinition.GetNextLevelNumber(CurrentLevel);
        }

        public bool TryCompleteCurrentLevel(out LevelProgressionResult result)
        {
            int completedLevel = CurrentLevel;
            _progressData.MarkCurrentLevelCompleted();
            PersistProgress();

            ProgressionEvents.RaiseLevelCompleted(new LevelCompletedContext(completedLevel));
            result = LevelProgressionResult.Completed(completedLevel);
            return true;
        }

        public bool TryAdvanceToNextLevel(out LevelProgressionResult result)
        {
            int previousLevel = CurrentLevel;
            if (!LevelProgressionDefinition.CanAdvanceFrom(previousLevel))
            {
                result = LevelProgressionResult.FailedAtMaxLevel(previousLevel);
                return false;
            }

            int nextLevel = LevelProgressionDefinition.GetNextLevelNumber(previousLevel);
            SetCurrentLevel(nextLevel);

            ProgressionEvents.RaiseLevelAdvanced(new LevelAdvancedContext(previousLevel, nextLevel));
            result = LevelProgressionResult.Advanced(previousLevel, nextLevel);
            return true;
        }

        internal void SetCurrentLevelForValidation(int levelNumber)
        {
            _progressData.SetCurrentLevel(levelNumber);
        }

        private void PersistProgress()
        {
            if (!SaveSystem.HasInstance)
            {
                return;
            }

            _progressData.WriteToSave(SaveSystem.Instance.Data);
            SaveSystem.Instance.Save();
        }
    }
}