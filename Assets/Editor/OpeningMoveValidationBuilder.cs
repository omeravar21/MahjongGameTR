#if UNITY_EDITOR
using System.Text;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class OpeningMoveValidationBuilder
    {
        [MenuItem("MahjongGame/Validate OpeningMoveChecker")]
        public static void ValidateOpeningMoveChecker()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = OpeningMoveCheckerSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[OpeningMoveValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[OpeningMoveValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
