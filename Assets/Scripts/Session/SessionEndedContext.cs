namespace MahjongGame.Session
{
    public sealed class SessionEndedContext
    {
        public SessionData Session { get; }

        public SessionEndReason Reason { get; }

        public SessionEndedContext(SessionData session, SessionEndReason reason)
        {
            Session = session;
            Reason = reason;
        }
    }
}
