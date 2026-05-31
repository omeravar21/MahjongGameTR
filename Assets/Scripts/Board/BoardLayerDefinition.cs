using UnityEngine;

namespace MahjongGame.Board
{
    public static class BoardLayerDefinition
    {
        public const int MaxLayerCount = BoardRootController.MaxLayerCount;
        public const int SortingOrdersPerLayer = BoardGridDefinition.TotalCellCount + 1;
        public const float LocalZStep = 0.01f;

        private static readonly float[] LayerShadowScales = { 1.0f, 0.8f, 0.6f, 0.4f };

        public static bool IsValidLayerIndex(int layerIndex)
        {
            return layerIndex >= 0 && layerIndex < MaxLayerCount;
        }

        public static float GetLayerShadowScale(int layerIndex)
        {
            if (!IsValidLayerIndex(layerIndex))
            {
                Debug.LogWarning("[BoardLayerDefinition] Layer index out of range: " + layerIndex);
                return 1f;
            }

            return LayerShadowScales[layerIndex];
        }

        public static float GetLayerLocalZ(int layerIndex)
        {
            if (!IsValidLayerIndex(layerIndex))
            {
                return 0f;
            }

            return -layerIndex * LocalZStep;
        }
    }
}
