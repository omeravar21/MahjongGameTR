namespace MahjongGame.DailyMissions
{
    public sealed class DailyMissionProgress
    {
        public int CurrentValue { get; private set; }

        public bool IsCompleted { get; private set; }

        public DailyMissionProgress(int currentValue = 0, bool isCompleted = false)
        {
            CurrentValue = currentValue < 0 ? 0 : currentValue;
            IsCompleted = isCompleted;
        }

        public void SetCurrentValue(int currentValue)
        {
            CurrentValue = currentValue < 0 ? 0 : currentValue;
        }

        public void MarkCompleted()
        {
            IsCompleted = true;
        }
    }
}
