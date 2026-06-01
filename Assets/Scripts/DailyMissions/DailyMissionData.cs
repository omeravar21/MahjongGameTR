using MahjongGame.Core.Save;

namespace MahjongGame.DailyMissions
{
    public sealed class DailyMissionData
    {
        public int MissionDayId { get; private set; }

        public int[] SlotMissionTypes { get; private set; } = new int[DailyMissionSaveData.SlotCount];

        public int[] SlotProgress { get; private set; } = new int[DailyMissionSaveData.SlotCount];

        public bool[] SlotCompleted { get; private set; } = new bool[DailyMissionSaveData.SlotCount];

        public bool[] SlotRewardClaimed { get; private set; } = new bool[DailyMissionSaveData.SlotCount];

        public static DailyMissionData CreateDefault()
        {
            return new DailyMissionData();
        }

        public static DailyMissionData FromSave(PlayerSaveData saveData)
        {
            DailyMissionData missionData = new DailyMissionData();

            if (saveData == null)
            {
                return missionData;
            }

            missionData.ApplyFromSave(saveData);
            return missionData;
        }

        public void ApplyFromSave(PlayerSaveData saveData)
        {
            MissionDayId = 0;
            ResetRuntimeArrays();

            if (saveData?.dailyMissions == null)
            {
                return;
            }

            DailyMissionSaveData save = saveData.dailyMissions;
            save.EnsureDefaults();
            MissionDayId = save.missionDayId < 0 ? 0 : save.missionDayId;

            for (int i = 0; i < DailyMissionSaveData.SlotCount; i++)
            {
                SlotMissionTypes[i] = save.slotMissionTypes[i];
                SlotProgress[i] = save.slotProgress[i] < 0 ? 0 : save.slotProgress[i];
                SlotCompleted[i] = save.slotCompleted[i];
                SlotRewardClaimed[i] = save.slotRewardClaimed[i];
            }
        }

        public void WriteToSave(PlayerSaveData saveData)
        {
            if (saveData == null)
            {
                return;
            }

            saveData.dailyMissions ??= new DailyMissionSaveData();
            saveData.dailyMissions.EnsureDefaults();
            saveData.dailyMissions.missionDayId = MissionDayId < 0 ? 0 : MissionDayId;

            for (int i = 0; i < DailyMissionSaveData.SlotCount; i++)
            {
                saveData.dailyMissions.slotMissionTypes[i] = SlotMissionTypes[i];
                saveData.dailyMissions.slotProgress[i] = SlotProgress[i] < 0 ? 0 : SlotProgress[i];
                saveData.dailyMissions.slotCompleted[i] = SlotCompleted[i];
                saveData.dailyMissions.slotRewardClaimed[i] = SlotRewardClaimed[i];
            }
        }

        public void ResetForDay(int dayId, DailyMissionType[] slotTypes)
        {
            MissionDayId = dayId < 0 ? 0 : dayId;
            ResetRuntimeArrays();

            if (slotTypes == null)
            {
                return;
            }

            int copyCount = System.Math.Min(slotTypes.Length, DailyMissionSaveData.SlotCount);
            for (int i = 0; i < copyCount; i++)
            {
                SlotMissionTypes[i] = (int)slotTypes[i];
            }
        }

        public DailyMissionType GetSlotMissionType(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= DailyMissionSaveData.SlotCount)
            {
                return DailyMissionType.CompleteLevels;
            }

            return (DailyMissionType)SlotMissionTypes[slotIndex];
        }

        public int GetSlotProgress(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= DailyMissionSaveData.SlotCount)
            {
                return 0;
            }

            return SlotProgress[slotIndex];
        }

        public bool IsSlotCompleted(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= DailyMissionSaveData.SlotCount)
            {
                return false;
            }

            return SlotCompleted[slotIndex];
        }

        public void SetSlotProgress(int slotIndex, int progress, bool isCompleted)
        {
            if (slotIndex < 0 || slotIndex >= DailyMissionSaveData.SlotCount)
            {
                return;
            }

            SlotProgress[slotIndex] = progress < 0 ? 0 : progress;
            SlotCompleted[slotIndex] = isCompleted;
        }

        public bool IsSlotRewardClaimed(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= DailyMissionSaveData.SlotCount)
            {
                return false;
            }

            return SlotRewardClaimed[slotIndex];
        }

        public bool TryMarkSlotRewardClaimed(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= DailyMissionSaveData.SlotCount || SlotRewardClaimed[slotIndex])
            {
                return false;
            }

            SlotRewardClaimed[slotIndex] = true;
            return true;
        }

        private void ResetRuntimeArrays()
        {
            SlotMissionTypes = new int[DailyMissionSaveData.SlotCount];
            SlotProgress = new int[DailyMissionSaveData.SlotCount];
            SlotCompleted = new bool[DailyMissionSaveData.SlotCount];
            SlotRewardClaimed = new bool[DailyMissionSaveData.SlotCount];
        }
    }
}
