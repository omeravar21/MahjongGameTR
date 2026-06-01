using System;

namespace MahjongGame.Ranking
{
    public sealed class GlobalPerformanceScoreChangedContext
    {
        public long PreviousScore { get; }

        public long NewScore { get; }

        public GlobalPerformanceScoreChangedContext(long previousScore, long newScore)
        {
            PreviousScore = previousScore < 0 ? 0 : previousScore;
            NewScore = newScore < 0 ? 0 : newScore;
        }
    }

    public sealed class GlobalRankChangedContext
    {
        public int PreviousRank { get; }

        public int NewRank { get; }

        public GlobalRankChangedContext(int previousRank, int newRank)
        {
            PreviousRank = previousRank < 0 ? 0 : previousRank;
            NewRank = newRank < 0 ? 0 : newRank;
        }
    }

    public static class RankingEvents
    {
        public static event Action<GlobalPerformanceScoreChangedContext> GlobalPerformanceScoreChanged;

        public static event Action<GlobalRankChangedContext> GlobalRankChanged;

        internal static void RaiseGlobalPerformanceScoreChanged(GlobalPerformanceScoreChangedContext context)
        {
            if (context == null)
            {
                return;
            }

            GlobalPerformanceScoreChanged?.Invoke(context);
        }

        internal static void RaiseGlobalRankChanged(GlobalRankChangedContext context)
        {
            if (context == null)
            {
                return;
            }

            GlobalRankChanged?.Invoke(context);
        }
    }
}
