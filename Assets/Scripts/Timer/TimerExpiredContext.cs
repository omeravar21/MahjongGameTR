namespace MahjongGame.Timer
{
    public sealed class TimerExpiredContext
    {
        public float AllocatedTimeSeconds { get; }

        public int LevelNumber { get; }

        public TimerExpiredContext(float allocatedTimeSeconds, int levelNumber)
        {
            AllocatedTimeSeconds = allocatedTimeSeconds;
            LevelNumber = levelNumber;
        }
    }
}
