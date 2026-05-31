#if UNITY_EDITOR
using System.Text;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class HolePatternValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Hole Pattern System")]
        public static void ValidateHolePatternSystem()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = HolePatternSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[HolePatternValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[HolePatternValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
