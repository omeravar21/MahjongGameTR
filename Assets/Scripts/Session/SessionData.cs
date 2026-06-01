namespace MahjongGame.Session
{
    public sealed class SessionData
    {
        public int SessionId { get; }

        public int LevelNumber { get; }

        public LevelSessionState State { get; }

        public SessionMode Mode { get; }

        public int DailyDayId { get; }

        public bool IsActive => State == LevelSessionState.Active;

        public SessionData(
            int sessionId,
            int levelNumber,
            LevelSessionState state,
            SessionMode mode = SessionMode.Normal,
            int dailyDayId = 0)
        {
            SessionId = sessionId;
            LevelNumber = levelNumber;
            State = state;
            Mode = mode;
            DailyDayId = dailyDayId < 0 ? 0 : dailyDayId;
        }
    }
}
