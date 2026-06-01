using MahjongGame.Boosters;
using MahjongGame.Core;
using MahjongGame.Core.Save;
using MahjongGame.DailyBoard;
using MahjongGame.DailyMissions;
using MahjongGame.Ranking;
using MahjongGame.Score;
using UnityEngine;

namespace MahjongGame.DailyRewards
{
    public sealed class DailyRewardDirector : MonoBehaviour
    {
        private static DailyRewardDirector _instance;

        public static DailyRewardDirector Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[DailyRewardDirector] Instance is not available.");
                }

                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

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

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public bool TryGrantGlobalPerformanceScore(int amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            if (!RankingDirector.HasInstance)
            {
                Debug.LogWarning("[DailyRewardDirector] RankingDirector is not available.");
                return false;
            }

            LevelPerformanceResult performanceResult = new LevelPerformanceResult(0, 0, 0, amount);
            return RankingDirector.Instance.TryAccumulateLevelPerformance(performanceResult, out _, out _);
        }

        public bool TryGrantBoosters(int shuffle, int undo, int hint)
        {
            if (shuffle <= 0 && undo <= 0 && hint <= 0)
            {
                return false;
            }

            BoosterEconomyDirector economyDirector = Object.FindAnyObjectByType<BoosterEconomyDirector>();
            if (economyDirector != null)
            {
                bool granted = false;
                if (shuffle > 0)
                {
                    granted |= economyDirector.TryGrant(BoosterType.Shuffle, shuffle);
                }

                if (undo > 0)
                {
                    granted |= economyDirector.TryGrant(BoosterType.Undo, undo);
                }

                if (hint > 0)
                {
                    granted |= economyDirector.TryGrant(BoosterType.Hint, hint);
                }

                return granted;
            }

            if (!SaveSystem.HasInstance || SaveSystem.Instance.Data == null)
            {
                Debug.LogWarning("[DailyRewardDirector] SaveSystem is not available.");
                return false;
            }

            PlayerSaveData saveData = SaveSystem.Instance.Data;
            saveData.EnsureDefaults();
            saveData.boosterCounts ??= new BoosterCountsSaveData();

            BoosterCounts counts = BoosterCounts.FromSave(saveData.boosterCounts);
            if (shuffle > 0)
            {
                counts.Grant(BoosterType.Shuffle, shuffle);
            }

            if (undo > 0)
            {
                counts.Grant(BoosterType.Undo, undo);
            }

            if (hint > 0)
            {
                counts.Grant(BoosterType.Hint, hint);
            }

            counts.WriteToSave(saveData.boosterCounts);
            SaveSystem.Instance.Save();
            BoosterEvents.RaiseBoosterCountsChanged(new BoosterCountsChangedContext(counts));
            return true;
        }

        public bool TryGrantDailyBoardCompletionRewards()
        {
            bool granted = TryGrantGlobalPerformanceScore(
                DailyBoardRewardDefinition.GetCompletionGlobalPerformanceScore());

            granted |= TryGrantBoosters(
                DailyBoardRewardDefinition.GetCompletionShuffleBoosterReward(),
                DailyBoardRewardDefinition.GetCompletionUndoBoosterReward(),
                DailyBoardRewardDefinition.GetCompletionHintBoosterReward());

            return granted;
        }

        public bool TryGrantMissionCompletionRewards(DailyMissionTier tier)
        {
            bool granted = TryGrantGlobalPerformanceScore(
                DailyMissionRewardDefinition.GetGlobalPerformanceScore(tier));

            granted |= TryGrantBoosters(
                DailyMissionRewardDefinition.GetShuffleReward(tier),
                DailyMissionRewardDefinition.GetUndoReward(tier),
                DailyMissionRewardDefinition.GetHintReward(tier));

            return granted;
        }

        internal void EnsureValidationInstance()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }
    }
}
