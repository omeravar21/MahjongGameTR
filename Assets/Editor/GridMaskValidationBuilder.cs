#if UNITY_EDITOR
using System.Text;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class GridMaskValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Grid Mask System")]
        public static void ValidateGridMaskSystem()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = GridMaskSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[GridMaskValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[GridMaskValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
