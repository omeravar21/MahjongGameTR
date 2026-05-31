using System;

namespace MahjongGame.Score
{
    public static class ScoreEvents
    {
        public static event Action<ScoreChangedContext> ScoreChanged;

        public static event Action<MatchScoreAwardedContext> MatchScoreAwarded;

        internal static void RaiseScoreChanged(ScoreChangedContext context)
        {
            if (context == null)
            {
                return;
            }

            ScoreChanged?.Invoke(context);
        }

        internal static void RaiseMatchScoreAwarded(MatchScoreAwardedContext context)
        {
            if (context == null)
            {
                return;
            }

            MatchScoreAwarded?.Invoke(context);
        }
    }
}
