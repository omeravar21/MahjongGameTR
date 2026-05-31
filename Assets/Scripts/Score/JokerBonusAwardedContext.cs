namespace MahjongGame.Score
{
    public sealed class JokerBonusAwardedContext
    {
        public int BonusPoints { get; }

        public int JokerTileId { get; }

        public int EarlyJokerMatchCount { get; }

        public int JokerBonusTotal { get; }

        public JokerBonusAwardedContext(
            int bonusPoints,
            int jokerTileId,
            int earlyJokerMatchCount,
            int jokerBonusTotal)
        {
            BonusPoints = bonusPoints < 0 ? 0 : bonusPoints;
            JokerTileId = jokerTileId;
            EarlyJokerMatchCount = earlyJokerMatchCount < 0 ? 0 : earlyJokerMatchCount;
            JokerBonusTotal = jokerBonusTotal < 0 ? 0 : jokerBonusTotal;
        }
    }
}
