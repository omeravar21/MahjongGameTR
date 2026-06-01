namespace MahjongGame.Ranking
{
    public sealed class LeaderboardEntry
    {
        public int RankPosition { get; }

        public string DisplayName { get; }

        public long GlobalPerformanceScore { get; }

        public bool IsLocalPlayer { get; }

        public LeaderboardEntry(
            int rankPosition,
            string displayName,
            long globalPerformanceScore,
            bool isLocalPlayer)
        {
            RankPosition = rankPosition < 0 ? 0 : rankPosition;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? "Player" : displayName;
            GlobalPerformanceScore = globalPerformanceScore < 0 ? 0 : globalPerformanceScore;
            IsLocalPlayer = isLocalPlayer;
        }
    }
}
