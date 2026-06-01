using MahjongGame.Session;

namespace MahjongGame.Core
{
    public static class GameLaunchRequest
    {
        private static SessionMode _pendingSessionMode = SessionMode.Normal;

        public static SessionMode PendingSessionMode => _pendingSessionMode;

        public static void RequestNormalLevel()
        {
            _pendingSessionMode = SessionMode.Normal;
        }

        public static void RequestDailyBoard()
        {
            _pendingSessionMode = SessionMode.DailyBoard;
        }

        public static SessionMode ConsumePendingSessionMode()
        {
            SessionMode mode = _pendingSessionMode;
            _pendingSessionMode = SessionMode.Normal;
            return mode;
        }

        public static void ClearPendingLaunch()
        {
            _pendingSessionMode = SessionMode.Normal;
        }
    }
}
