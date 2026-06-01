namespace MahjongGame.DailyMissions
{
    public sealed class DailyMissionEntry
    {
        public DailyMissionSlot Slot { get; }

        public DailyMissionProgress Progress { get; }

        public DailyMissionEntry(DailyMissionSlot slot, DailyMissionProgress progress)
        {
            Slot = slot;
            Progress = progress ?? new DailyMissionProgress();
        }

        public bool IsComplete()
        {
            return Progress.IsCompleted || Progress.CurrentValue >= Slot.TargetValue;
        }
    }
}
