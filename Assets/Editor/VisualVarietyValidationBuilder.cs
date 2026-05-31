#if UNITY_EDITOR
using System.Text;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class VisualVarietyValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Visual Variety System")]
        public static void ValidateVisualVarietySystem()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = VisualVarietySystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[VisualVarietyValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[VisualVarietyValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
