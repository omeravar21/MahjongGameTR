using System;

namespace MahjongGame.Session
{
    public static class PenaltyEvents
    {
        public static event Action<TimerExpirationPenaltyContext> TimerExpirationPenaltyDetected;

        internal static void RaiseTimerExpirationPenaltyDetected(TimerExpirationPenaltyContext context)
        {
            if (context == null)
            {
                return;
            }

            TimerExpirationPenaltyDetected?.Invoke(context);
        }
    }
}
