using System;

namespace MahjongGame.Matching
{
    public static class MatchEvents
    {
        public static event Action<MatchRequest> MatchDetected;

        public static event Action<MatchRequest> MatchDelayCompleted;

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
    }
}
