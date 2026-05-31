namespace MahjongGame.Session
{
    public sealed class SessionData
    {
        public int SessionId { get; }

        public int LevelNumber { get; }

        public LevelSessionState State { get; }

        public bool IsActive => State == LevelSessionState.Active;

        public SessionData(int sessionId, int levelNumber, LevelSessionState state)
        {
            SessionId = sessionId;
            LevelNumber = levelNumber;
            State = state;
        }
    }
}
