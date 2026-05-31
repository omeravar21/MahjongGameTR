namespace MahjongGame.Session
{
    public sealed class SessionStartedContext
    {
        public int LevelNumber { get; }

        public int SessionId { get; }

        public SessionData Session { get; }

        public bool IsResumeSession { get; }

        public SessionStartedContext(SessionData session, bool isResumeSession = false)
        {
            Session = session;
            SessionId = session != null ? session.SessionId : -1;
            LevelNumber = session != null ? session.LevelNumber : 0;
            IsResumeSession = isResumeSession;
        }
    }
}
