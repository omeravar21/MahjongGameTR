using System;

namespace MahjongGame.Matching
{
    public static class MatchEvents
    {
        public static event Action<MatchRequest> MatchDetected;

        public static event Action<MatchRequest> MatchDelayCompleted;

        public static event Action<MatchExecutionContext> MatchExecuted;

        internal static void RaiseMatchDetected(MatchRequest request)
        {
            if (request == null)
            {
                return;
            }

            MatchDetected?.Invoke(request);
        }

        internal static void RaiseMatchDelayCompleted(MatchRequest request)
        {
            if (request == null)
            {
                return;
            }

            MatchDelayCompleted?.Invoke(request);
        }

        internal static void RaiseMatchExecuted(MatchExecutionContext context)
        {
            if (context == null)
            {
                return;
            }

            MatchExecuted?.Invoke(context);
        }
    }
}
