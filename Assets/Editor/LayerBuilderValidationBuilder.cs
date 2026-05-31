#if UNITY_EDITOR
using System.Text;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class LayerBuilderValidationBuilder
    {
        [MenuItem("MahjongGame/Validate LayerBuilder")]
        public static void ValidateLayerBuilder()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = LayerBuilderSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[LayerBuilderValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[LayerBuilderValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
