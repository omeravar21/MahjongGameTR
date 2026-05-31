using MahjongGame.Core;
using MahjongGame.Core.Save;
using MahjongGame.Session;
using UnityEngine;

namespace MahjongGame.Boosters
{
    public sealed class BoosterEconomyDirector : MonoBehaviour
    {
        private BoosterCounts _counts = BoosterCounts.CreateDefault();
        private int _totalLevelsCompleted;

        public BoosterCounts Counts => _counts;

        public int TotalLevelsCompleted => _totalLevelsCompleted;

        private void Awake()
        {
            LoadFromSave();
        }

        private void OnEnable()
        {
            SessionEvents.SessionStarted += HandleSessionStarted;
            SessionEvents.SessionEnded += HandleSessionEnded;
        }

        private void OnDisable()
        {
            SessionEvents.SessionStarted -= HandleSessionStarted;
            SessionEvents.SessionEnded -= HandleSessionEnded;
        }

        public int GetCount(BoosterType boosterType)
        {
            return _counts.GetCount(boosterType);
        }

        public bool TryGrant(BoosterType boosterType, int amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            _counts.Grant(boosterType, amount);
            PersistCounts();
            BoosterEvents.RaiseBoosterCountsChanged(new BoosterCountsChangedContext(_counts));
            return true;
        }

        public bool TryConsume(BoosterType boosterType)
        {
            if (!_counts.TryConsume(boosterType))
            {
                return false;
            }

            PersistCounts();
            BoosterEvents.RaiseBoosterCountsChanged(new BoosterCountsChangedContext(_counts));
            return true;
        }

        public void ApplyProgressionRewardOnWin()
        {
            _totalLevelsCompleted++;
            PersistProgression();

            if (_totalLevelsCompleted % BoosterDefinition.ProgressionIntervalLevels != 0)
            {
                return;
            }

            TryGrant(BoosterType.Shuffle, BoosterDefinition.RewardsPerMilestone);
            TryGrant(BoosterType.Undo, BoosterDefinition.RewardsPerMilestone);
            TryGrant(BoosterType.Hint, BoosterDefinition.RewardsPerMilestone);
            BoosterEvents.RaiseBoosterProgressionRewardGranted(
                new BoosterProgressionRewardGrantedContext(_totalLevelsCompleted));
        }

        internal bool TryApplyProgressionRewardForValidation(int completedLevelsBeforeWin)
        {
            _totalLevelsCompleted = completedLevelsBeforeWin;
            ApplyProgressionRewardOnWin();
            return _totalLevelsCompleted % BoosterDefinition.ProgressionIntervalLevels == 0;
        }

        internal void ResetCountsForValidation()
        {
            _counts = BoosterCounts.CreateDefault();
            _totalLevelsCompleted = 0;
            PersistAll();
            BoosterEvents.RaiseBoosterRuntimeReset();
        }

        private void HandleSessionStarted(SessionStartedContext context)
        {
            BoosterEvents.RaiseBoosterRuntimeReset();
        }

        private void HandleSessionEnded(SessionEndedContext context)
        {
            if (context == null || context.Reason != SessionEndReason.Win)
            {
                return;
            }

            ApplyProgressionRewardOnWin();
        }

        private void LoadFromSave()
        {
            if (!SaveSystem.HasInstance || SaveSystem.Instance.Data == null)
            {
                _counts = BoosterCounts.CreateDefault();
                _totalLevelsCompleted = 0;
                return;
            }

            PlayerSaveData saveData = SaveSystem.Instance.Data;
            saveData.EnsureDefaults();
            _counts = BoosterCounts.FromSave(saveData.boosterCounts);
            _totalLevelsCompleted = saveData.totalLevelsCompleted;
        }

        private void PersistCounts()
        {
            if (!SaveSystem.HasInstance || SaveSystem.Instance.Data == null)
            {
                return;
            }

            PlayerSaveData saveData = SaveSystem.Instance.Data;
            saveData.EnsureDefaults();
            saveData.boosterCounts ??= new BoosterCountsSaveData();
            _counts.WriteToSave(saveData.boosterCounts);
            SaveSystem.Instance.Save();
        }

        private void PersistProgression()
        {
            if (!SaveSystem.HasInstance || SaveSystem.Instance.Data == null)
            {
                return;
            }

            PlayerSaveData saveData = SaveSystem.Instance.Data;
            saveData.totalLevelsCompleted = _totalLevelsCompleted;
            SaveSystem.Instance.Save();
        }

        private void PersistAll()
        {
            if (!SaveSystem.HasInstance || SaveSystem.Instance.Data == null)
            {
                return;
            }

            PlayerSaveData saveData = SaveSystem.Instance.Data;
            saveData.EnsureDefaults();
            saveData.boosterCounts ??= new BoosterCountsSaveData();
            _counts.WriteToSave(saveData.boosterCounts);
            saveData.totalLevelsCompleted = _totalLevelsCompleted;
            SaveSystem.Instance.Save();
        }
    }
}
