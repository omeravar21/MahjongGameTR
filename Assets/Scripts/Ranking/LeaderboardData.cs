namespace MahjongGame.Ranking
{
    public sealed class LeaderboardData
    {
        public static readonly LeaderboardData Empty = new LeaderboardData(
            System.Array.Empty<LeaderboardEntry>(),
            0,
            0);

        public LeaderboardEntry[] Entries { get; }

        public int LocalPlayerRank { get; }

        public long LocalPlayerScore { get; }

        public LeaderboardData(
            LeaderboardEntry[] entries,
            int localPlayerRank,
            long localPlayerScore)
        {
            Entries = entries ?? System.Array.Empty<LeaderboardEntry>();
            LocalPlayerRank = localPlayerRank < 0 ? 0 : localPlayerRank;
            LocalPlayerScore = localPlayerScore < 0 ? 0 : localPlayerScore;
        }
    }
}
