#if UNITY_EDITOR
using System.Text;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class BoardQualityValidationBuilder
    {
        [MenuItem("MahjongGame/Validate BoardQualityChecker")]
        public static void ValidateBoardQualityChecker()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = BoardQualityCheckerSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[BoardQualityValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[BoardQualityValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
