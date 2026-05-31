namespace MahjongGame.Session
{
    public sealed class TimerExpirationPenaltyContext
    {
        public float AllocatedTimeSeconds { get; }

        public int LevelNumber { get; }

        public TimerExpirationPenaltyContext(float allocatedTimeSeconds, int levelNumber)
        {
            AllocatedTimeSeconds = allocatedTimeSeconds;
            LevelNumber = levelNumber;
        }
    }
}
