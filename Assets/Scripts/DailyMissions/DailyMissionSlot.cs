namespace MahjongGame.DailyMissions
{
    public sealed class DailyMissionSlot
    {
        public int SlotIndex { get; }

        public DailyMissionType MissionType { get; }

        public DailyMissionTier Tier { get; }

        public int TargetValue { get; }

        public DailyMissionSlot(int slotIndex, DailyMissionType missionType, DailyMissionTier tier, int targetValue)
        {
            SlotIndex = slotIndex < 0 ? 0 : slotIndex;
            MissionType = missionType;
            Tier = tier;
            TargetValue = targetValue < 1 ? 1 : targetValue;
        }
    }
}
