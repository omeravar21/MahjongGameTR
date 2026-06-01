using System;

namespace MahjongGame.DailyMissions
{
    public sealed class DailyMissionsRefreshedContext
    {
        public DailyMissionSet MissionSet { get; }

        public DailyMissionsRefreshedContext(DailyMissionSet missionSet)
        {
            MissionSet = missionSet ?? DailyMissionSet.Empty;
        }
    }

    public sealed class DailyMissionProgressChangedContext
    {
        public int SlotIndex { get; }

        public DailyMissionType MissionType { get; }

        public int PreviousValue { get; }

        public int CurrentValue { get; }

        public int TargetValue { get; }

        public bool IsCompleted { get; }

        public DailyMissionProgressChangedContext(
            int slotIndex,
            DailyMissionType missionType,
            int previousValue,
            int currentValue,
            int targetValue,
            bool isCompleted)
        {
            SlotIndex = slotIndex < 0 ? 0 : slotIndex;
            MissionType = missionType;
            PreviousValue = previousValue < 0 ? 0 : previousValue;
            CurrentValue = currentValue < 0 ? 0 : currentValue;
            TargetValue = targetValue < 1 ? 1 : targetValue;
            IsCompleted = isCompleted;
        }
    }

    public sealed class DailyMissionCompletedContext
    {
        public int SlotIndex { get; }

        public DailyMissionType MissionType { get; }

        public DailyMissionTier Tier { get; }

        public DailyMissionCompletedContext(int slotIndex, DailyMissionType missionType, DailyMissionTier tier)
        {
            SlotIndex = slotIndex < 0 ? 0 : slotIndex;
            MissionType = missionType;
            Tier = tier;
        }
    }

    public static class DailyMissionEvents
    {
        public static event Action<DailyMissionsRefreshedContext> DailyMissionsRefreshed;

        public static event Action<DailyMissionProgressChangedContext> DailyMissionProgressChanged;

        public static event Action<DailyMissionCompletedContext> DailyMissionCompleted;

        public static void RaiseDailyMissionsRefreshed(DailyMissionsRefreshedContext context)
        {
            DailyMissionsRefreshed?.Invoke(context);
        }

        public static void RaiseDailyMissionProgressChanged(DailyMissionProgressChangedContext context)
        {
            DailyMissionProgressChanged?.Invoke(context);
        }

        public static void RaiseDailyMissionCompleted(DailyMissionCompletedContext context)
        {
            DailyMissionCompleted?.Invoke(context);
        }
    }
}
