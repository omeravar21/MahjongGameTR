using System;

namespace MahjongGame.Matching
{
    public static class MatchEvents
    {
        public static event Action<MatchRequest> MatchDetected;

        internal static void RaiseMatchDetected(MatchRequest request)
        {
            if (request == null)
            {
                return;
            }

            MatchDetected?.Invoke(request);
        }
    }
}
