using System.Collections.Generic;
using MahjongGame.Core.Save;

namespace MahjongGame.Ranking
{
    public static class GlobalLeaderboardBuilder
    {
        private sealed class LeaderboardSortEntry
        {
            public string DisplayName { get; set; }

            public long GlobalPerformanceScore { get; set; }

            public bool IsLocalPlayer { get; set; }
        }

        public static LeaderboardData Build(long localPlayerScore, string localDisplayName)
        {
            return Build(localPlayerScore, localDisplayName, null);
        }

        public static LeaderboardData Build(
            long localPlayerScore,
            string localDisplayName,
            RankingSyncEntrySaveData[] remoteEntries)
        {
            List<LeaderboardSortEntry> mergedEntries = new List<LeaderboardSortEntry>();

            if (remoteEntries != null && remoteEntries.Length > 0)
            {
                for (int index = 0; index < remoteEntries.Length; index++)
                {
                    RankingSyncEntrySaveData entry = remoteEntries[index];
                    if (entry == null)
                    {
                        continue;
                    }

                    mergedEntries.Add(new LeaderboardSortEntry
                    {
                        DisplayName = entry.displayName,
                        GlobalPerformanceScore = entry.globalPerformanceScore,
                        IsLocalPlayer = false
                    });
                }
            }
            else
            {
                for (int index = 0; index < GlobalLeaderboardDefinition.ReferenceEntryCount; index++)
                {
                    (string displayName, long score) = GlobalLeaderboardDefinition.GetReferenceEntry(index);
                    mergedEntries.Add(new LeaderboardSortEntry
                    {
                        DisplayName = displayName,
                        GlobalPerformanceScore = score,
                        IsLocalPlayer = false
                    });
                }
            }

            mergedEntries.Add(new LeaderboardSortEntry
            {
                DisplayName = localDisplayName,
                GlobalPerformanceScore = localPlayerScore < 0 ? 0 : localPlayerScore,
                IsLocalPlayer = true
            });

            return BuildSortedLeaderboard(mergedEntries, localPlayerScore);
        }

        private static LeaderboardData BuildSortedLeaderboard(
            List<LeaderboardSortEntry> mergedEntries,
            long localPlayerScore)
        {
            mergedEntries.Sort((left, right) =>
            {
                int scoreComparison = right.GlobalPerformanceScore.CompareTo(left.GlobalPerformanceScore);
                if (scoreComparison != 0)
                {
                    return scoreComparison;
                }

                if (left.IsLocalPlayer == right.IsLocalPlayer)
                {
                    return string.Compare(left.DisplayName, right.DisplayName, System.StringComparison.Ordinal);
                }

                return left.IsLocalPlayer ? -1 : 1;
            });

            int localPlayerRank = 0;
            long localPlayerResolvedScore = localPlayerScore < 0 ? 0 : localPlayerScore;
            int displayCount = System.Math.Min(mergedEntries.Count, GlobalLeaderboardDefinition.MaxDisplayedEntries);
            LeaderboardEntry[] displayedEntries = new LeaderboardEntry[displayCount];

            for (int index = 0; index < mergedEntries.Count; index++)
            {
                LeaderboardSortEntry entry = mergedEntries[index];
                int rankPosition = index + 1;

                if (entry.IsLocalPlayer)
                {
                    localPlayerRank = rankPosition;
                    localPlayerResolvedScore = entry.GlobalPerformanceScore;
                }

                if (index < displayCount)
                {
                    displayedEntries[index] = new LeaderboardEntry(
                        rankPosition,
                        entry.DisplayName,
                        entry.GlobalPerformanceScore,
                        entry.IsLocalPlayer);
                }
            }

            return new LeaderboardData(displayedEntries, localPlayerRank, localPlayerResolvedScore);
        }
    }
}
