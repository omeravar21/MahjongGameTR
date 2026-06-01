using System;
using MahjongGame.Core;
using MahjongGame.Core.Save;
using MahjongGame.Session;
using UnityEngine;

namespace MahjongGame.DailyMissions
{
    public sealed class DailyMissionDirector : MonoBehaviour
    {
        private static DailyMissionDirector _instance;

        private DailyMissionData _missionData = DailyMissionData.CreateDefault();
        private DailyMissionSet _currentSet = DailyMissionSet.Empty;
        private Func<DateTime> _utcNowProvider = () => DateTime.UtcNow;

        public static DailyMissionDirector Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[DailyMissionDirector] Instance is not available.");
                }

                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        public DailyMissionData MissionState => _missionData;

        public DailyMissionSet CurrentSet => _currentSet;

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
            RefreshDailyMissions();
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
                Debug.LogWarning("[DailyMissionDirector] SaveSystem is not ready.");
                return;
            }

            _missionData = DailyMissionData.FromSave(SaveSystem.Instance.Data);
        }

        public void RefreshDailyMissions()
        {
            int dayId = DailyMissionDefinition.GetUtcDayId(_utcNowProvider());
            int missionSeed = DailyMissionDefinition.ComputeMissionSeed(dayId);

            if (_missionData.MissionDayId != dayId)
            {
                DailyMissionSet generatedSet = DailyMissionSetGenerator.GenerateSet(dayId, missionSeed);
                DailyMissionType[] slotTypes = DailyMissionSetGenerator.ExtractSlotTypes(generatedSet);
                _missionData.ResetForDay(dayId, slotTypes);
                PersistToSave();
            }

            _currentSet = BuildCurrentSet(dayId, missionSeed);
            DailyMissionEvents.RaiseDailyMissionsRefreshed(new DailyMissionsRefreshedContext(_currentSet));
        }

        public DailyMissionSet GetCurrentSet()
        {
            if (!_currentSet.IsValid())
            {
                RefreshDailyMissions();
            }

            return _currentSet;
        }

        public bool TryApplyProgress(
            DailyMissionType missionType,
            int amount,
            out DailyMissionProgressChangedContext changeContext)
        {
            changeContext = null;

            if (amount <= 0 || !_currentSet.IsValid())
            {
                return false;
            }

            bool changed = false;

            for (int i = 0; i < _currentSet.Entries.Length; i++)
            {
                DailyMissionEntry entry = _currentSet.Entries[i];
                if (entry.Slot.MissionType != missionType || entry.Progress.IsCompleted)
                {
                    continue;
                }

                int previousValue = entry.Progress.CurrentValue;
                int nextValue = previousValue + amount;
                if (nextValue > entry.Slot.TargetValue)
                {
                    nextValue = entry.Slot.TargetValue;
                }

                if (nextValue == previousValue)
                {
                    continue;
                }

                bool isCompleted = nextValue >= entry.Slot.TargetValue;
                entry.Progress.SetCurrentValue(nextValue);
                _missionData.SetSlotProgress(i, nextValue, isCompleted);

                changeContext = new DailyMissionProgressChangedContext(
                    i,
                    missionType,
                    previousValue,
                    nextValue,
                    entry.Slot.TargetValue,
                    isCompleted);
                DailyMissionEvents.RaiseDailyMissionProgressChanged(changeContext);

                if (isCompleted)
                {
                    entry.Progress.MarkCompleted();
                    DailyMissionEvents.RaiseDailyMissionCompleted(
                        new DailyMissionCompletedContext(i, missionType, entry.Slot.Tier));
                }

                changed = true;
            }

            if (changed)
            {
                PersistToSave();
                _currentSet = BuildCurrentSet(_currentSet.DayId, _currentSet.MissionSeed);
            }

            return changed;
        }

        public bool TryApplyComboIncreased(int highestCombo)
        {
            if (highestCombo < 0 || !_currentSet.IsValid())
            {
                return false;
            }

            bool changed = false;

            for (int i = 0; i < _currentSet.Entries.Length; i++)
            {
                DailyMissionEntry entry = _currentSet.Entries[i];
                if (entry.Slot.MissionType != DailyMissionType.CreateCombos || entry.Progress.IsCompleted)
                {
                    continue;
                }

                int previousValue = entry.Progress.CurrentValue;
                int nextValue = entry.Slot.Tier == DailyMissionTier.Hard
                    ? Math.Max(previousValue, highestCombo)
                    : previousValue + 1;

                if (nextValue > entry.Slot.TargetValue)
                {
                    nextValue = entry.Slot.TargetValue;
                }

                if (nextValue == previousValue)
                {
                    continue;
                }

                bool isCompleted = nextValue >= entry.Slot.TargetValue;
                entry.Progress.SetCurrentValue(nextValue);
                _missionData.SetSlotProgress(i, nextValue, isCompleted);

                DailyMissionEvents.RaiseDailyMissionProgressChanged(
                    new DailyMissionProgressChangedContext(
                        i,
                        DailyMissionType.CreateCombos,
                        previousValue,
                        nextValue,
                        entry.Slot.TargetValue,
                        isCompleted));

                if (isCompleted)
                {
                    entry.Progress.MarkCompleted();
                    DailyMissionEvents.RaiseDailyMissionCompleted(
                        new DailyMissionCompletedContext(i, DailyMissionType.CreateCombos, entry.Slot.Tier));
                }

                changed = true;
            }

            if (changed)
            {
                PersistToSave();
                _currentSet = BuildCurrentSet(_currentSet.DayId, _currentSet.MissionSeed);
            }

            return changed;
        }

        public bool TryApplySessionWin(
            SessionEndedContext context,
            int boostersUsedThisSession,
            float completionTimeSeconds,
            float allocatedTimeSeconds)
        {
            if (context == null || context.Reason != SessionEndReason.Win || context.Session == null)
            {
                return false;
            }

            bool changed = false;

            if (context.Session.Mode == SessionMode.Normal)
            {
                changed |= TryApplyProgress(DailyMissionType.CompleteLevels, 1, out _);

                if (boostersUsedThisSession <= 0)
                {
                    changed |= TryApplyProgress(DailyMissionType.FinishWithoutBoosters, 1, out _);
                }

                if (allocatedTimeSeconds > 0f && completionTimeSeconds <= allocatedTimeSeconds)
                {
                    changed |= TryApplyProgress(DailyMissionType.FinishUnderTargetTime, 1, out _);
                }
            }
            else if (context.Session.Mode == SessionMode.DailyBoard)
            {
                changed |= TryApplyDailyBoardCompleted();
            }

            return changed;
        }

        public bool TryApplyDailyBoardCompleted()
        {
            return TryApplyProgress(DailyMissionType.CompleteDailyBoard, 1, out _);
        }

        internal void SetStateForValidation(int dayId, DateTime utcNow)
        {
            _utcNowProvider = () => utcNow;
            _missionData = DailyMissionData.CreateDefault();
            RefreshDailyMissions();
        }

        internal void ResetUtcNowProviderForValidation()
        {
            _utcNowProvider = () => DateTime.UtcNow;
        }

        internal DailyMissionSet BuildCurrentSetForValidation(int dayId, int missionSeed)
        {
            return BuildCurrentSet(dayId, missionSeed);
        }

        private DailyMissionSet BuildCurrentSet(int dayId, int missionSeed)
        {
            DailyMissionSet generatedSet = DailyMissionSetGenerator.GenerateSet(dayId, missionSeed);
            if (!generatedSet.IsValid())
            {
                return DailyMissionSet.Empty;
            }

            DailyMissionEntry[] entries = new DailyMissionEntry[DailyMissionSet.SlotCount];
            for (int i = 0; i < DailyMissionSet.SlotCount; i++)
            {
                DailyMissionEntry generatedEntry = generatedSet.Entries[i];
                int progressValue = _missionData.GetSlotProgress(i);
                bool isCompleted = _missionData.IsSlotCompleted(i)
                    || progressValue >= generatedEntry.Slot.TargetValue;
                DailyMissionProgress progress = new DailyMissionProgress(progressValue, isCompleted);
                entries[i] = new DailyMissionEntry(generatedEntry.Slot, progress);
            }

            return new DailyMissionSet(dayId, missionSeed, entries);
        }

        private void PersistToSave()
        {
            if (!SaveSystem.HasInstance)
            {
                return;
            }

            _missionData.WriteToSave(SaveSystem.Instance.Data);
            SaveSystem.Instance.Save();
        }
    }
}
