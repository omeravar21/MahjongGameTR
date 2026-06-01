#if UNITY_EDITOR
using System.Text;
using MahjongGame.Ranking;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class RankingSyncValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Leaderboard Sync")]
        public static void ValidateLeaderboardSync()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = RankingSyncSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[RankingSyncValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[RankingSyncValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
