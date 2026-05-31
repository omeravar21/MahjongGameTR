using System;

namespace MahjongGame.Timer
{
    public static class TimerEvents
    {
        public static event Action<TimerStartedContext> TimerStarted;

        public static event Action<float> TimerRemainingTimeChanged;

        public static event Action TimerPaused;

        public static event Action TimerResumed;

        public static event Action<TimerExpiredContext> TimerExpired;

        internal static void RaiseTimerStarted(TimerStartedContext context)
        {
            if (context == null)
            {
                return;
            }

            TimerStarted?.Invoke(context);
        }

        internal static void RaiseTimerRemainingTimeChanged(float remainingTimeSeconds)
        {
            TimerRemainingTimeChanged?.Invoke(remainingTimeSeconds);
        }

        internal static void RaiseTimerPaused()
        {
            TimerPaused?.Invoke();
        }

        internal static void RaiseTimerResumed()
        {
            TimerResumed?.Invoke();
        }

        internal static void RaiseTimerExpired(TimerExpiredContext context)
        {
            if (context == null)
            {
                return;
            }

            TimerExpired?.Invoke(context);
        }
    }
}
