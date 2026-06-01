#if UNITY_EDITOR
using System.Text;
using MahjongGame.Ranking;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class GlobalPerformanceScoreValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Global Performance Score")]
        public static void ValidateGlobalPerformanceScore()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = GlobalPerformanceScoreSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[GlobalPerformanceScoreValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[GlobalPerformanceScoreValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
