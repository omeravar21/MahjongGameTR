using System;
using MahjongGame.BoardGeneration;
using MahjongGame.Core;
using MahjongGame.Core.Save;
using UnityEngine;

namespace MahjongGame.DailyBoard
{
    public sealed class DailyBoardDirector : MonoBehaviour
    {
        private static DailyBoardDirector _instance;

        private DailyBoardData _dailyBoardData = DailyBoardData.CreateDefault();
        private DailyBoardIdentity _currentIdentity = DailyBoardIdentity.Empty;
        private Func<DateTime> _utcNowProvider = () => DateTime.UtcNow;

        public static DailyBoardDirector Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[DailyBoardDirector] Instance is not available.");
                }

                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        public DailyBoardData DailyBoardState => _dailyBoardData;

        public DailyBoardIdentity CurrentIdentity => _currentIdentity;

        public int LastCompletedDayId => _dailyBoardData.LastCompletedDayId;

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
            RefreshDailyBoard();
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
                Debug.LogWarning("[DailyBoardDirector] SaveSystem is not ready.");
                return;
            }

            _dailyBoardData = DailyBoardData.FromSave(SaveSystem.Instance.Data);
        }

        public void RefreshDailyBoard()
        {
            int dayId = DailyBoardDefinition.GetUtcDayId(_utcNowProvider());
            _currentIdentity = DailyBoardDefinition.BuildIdentity(dayId, _dailyBoardData.LastCompletedDayId);
            DailyBoardEvents.RaiseDailyBoardRefreshed(new DailyBoardRefreshedContext(_currentIdentity));
        }

        public DailyBoardIdentity GetCurrentIdentity()
        {
            if (!_currentIdentity.IsValid())
            {
                RefreshDailyBoard();
            }

            return _currentIdentity;
        }

        public bool TryMarkCompletedToday()
        {
            int dayId = DailyBoardDefinition.GetUtcDayId(_utcNowProvider());
            if (dayId <= 0 || _dailyBoardData.LastCompletedDayId == dayId)
            {
                return false;
            }

            _dailyBoardData.SetLastCompletedDayId(dayId);
            PersistToSave();
            RefreshDailyBoard();
            return true;
        }

        public bool TryGenerateRecipe(out LevelRecipe recipe)
        {
            recipe = null;
            DailyBoardIdentity identity = GetCurrentIdentity();

            if (!identity.IsValid())
            {
                return false;
            }

            recipe = DailyBoardRecipeDefinition.GenerateRecipe(identity.DayId, identity.DailySeed);
            return recipe != null;
        }

        public bool TryGenerateBoardData(out BoardData boardData)
        {
            boardData = null;

            if (!TryGenerateRecipe(out LevelRecipe recipe))
            {
                return false;
            }

            boardData = BoardGenerationPipeline.GenerateBoardData(recipe);
            return boardData != null && boardData.IsValidated;
        }

        internal void SetStateForValidation(int lastCompletedDayId, DateTime utcNow)
        {
            _utcNowProvider = () => utcNow;
            _dailyBoardData.SetLastCompletedDayId(lastCompletedDayId);
            RefreshDailyBoard();
        }

        internal void ResetUtcNowProviderForValidation()
        {
            _utcNowProvider = () => DateTime.UtcNow;
        }

        private void PersistToSave()
        {
            if (!SaveSystem.HasInstance)
            {
                return;
            }

            _dailyBoardData.WriteToSave(SaveSystem.Instance.Data);
            SaveSystem.Instance.Save();
        }
    }
}
