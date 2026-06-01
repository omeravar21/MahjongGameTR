#if UNITY_EDITOR
using System.Text;
using MahjongGame.Ranking;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class LeaderboardValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Global Leaderboard")]
        public static void ValidateGlobalLeaderboard()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = LeaderboardSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[LeaderboardValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[LeaderboardValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
