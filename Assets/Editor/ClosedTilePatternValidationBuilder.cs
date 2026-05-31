#if UNITY_EDITOR
using System.Text;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class ClosedTilePatternValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Closed Tile Pattern System")]
        public static void ValidateClosedTilePatternSystem()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = ClosedTilePatternSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[ClosedTilePatternValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[ClosedTilePatternValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
