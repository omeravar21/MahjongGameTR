#if UNITY_EDITOR
using System.Text;
using MahjongGame.Ranking;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class RankingValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Ranking Architecture")]
        public static void ValidateRankingArchitecture()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = RankingSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[RankingValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[RankingValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
