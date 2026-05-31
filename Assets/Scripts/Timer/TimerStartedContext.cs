namespace MahjongGame.Timer
{
    public sealed class TimerStartedContext
    {
        public float AllocatedTimeSeconds { get; }

        public float RemainingTimeSeconds { get; }

        public int LevelNumber { get; }

        public TimerStartedContext(float allocatedTimeSeconds, float remainingTimeSeconds, int levelNumber)
        {
            AllocatedTimeSeconds = allocatedTimeSeconds;
            RemainingTimeSeconds = remainingTimeSeconds;
            LevelNumber = levelNumber;
        }
    }
}
