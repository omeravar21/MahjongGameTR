namespace MahjongGame.Score
{
    public sealed class MatchScoreAwardedContext
    {
        public int PointsAwarded { get; }

        public int RunningTotal { get; }

        public int MatchCount { get; }

        public MatchScoreAwardedContext(int pointsAwarded, int runningTotal, int matchCount)
        {
            PointsAwarded = pointsAwarded;
            RunningTotal = runningTotal;
            MatchCount = matchCount;
        }
    }
}
