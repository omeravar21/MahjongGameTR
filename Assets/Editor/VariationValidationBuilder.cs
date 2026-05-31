#if UNITY_EDITOR
using System.Text;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class VariationValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Variation System")]
        public static void ValidateVariationSystem()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = VariationSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[VariationValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[VariationValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
