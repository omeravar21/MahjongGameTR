using System;

namespace MahjongGame.Session
{
    public static class SessionEvents
    {
        public static event Action<LevelSessionState, LevelSessionState> SessionStateChanged;

        public static event Action<SessionStartedContext> SessionStarted;

        public static event Action<SessionEndedContext> SessionEnded;

        internal static void RaiseSessionStateChanged(LevelSessionState previousState, LevelSessionState currentState)
        {
            SessionStateChanged?.Invoke(previousState, currentState);
        }

        internal static void RaiseSessionStarted(SessionStartedContext context)
        {
            if (context == null)
            {
                return;
            }

            SessionStarted?.Invoke(context);
        }

        internal static void RaiseSessionEnded(SessionEndedContext context)
        {
            if (context == null)
            {
                return;
            }

            SessionEnded?.Invoke(context);
        }
    }
}
