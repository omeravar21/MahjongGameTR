namespace MahjongGame.Ranking
{
    public sealed class RankingEntry
    {
        public string PlayerId { get; }

        public string DisplayName { get; }

        public long GlobalPerformanceScore { get; }

        public int RankPosition { get; }

        public bool IsLocalPlayer { get; }

        public RankingEntry(
            string playerId,
            string displayName,
            long globalPerformanceScore,
            int rankPosition,
            bool isLocalPlayer)
        {
            PlayerId = string.IsNullOrWhiteSpace(playerId) ? string.Empty : playerId;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? "Player" : displayName;
            GlobalPerformanceScore = globalPerformanceScore < 0 ? 0 : globalPerformanceScore;
            RankPosition = rankPosition < 0 ? 0 : rankPosition;
            IsLocalPlayer = isLocalPlayer;
        }
    }
}
