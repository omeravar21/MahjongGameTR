using UnityEngine;

namespace MahjongGame.Board
{
    public static class BoardLayerLayout
    {
        public static Vector3 GetTileLocalPosition(BoardGridCoordinate coordinate, int layerIndex)
        {
            Vector3 gridPosition = BoardGridLayout.GetCellLocalPosition(
                coordinate,
                BoardGridDefinition.DefaultCellWidth,
                BoardGridDefinition.DefaultCellHeight);

            gridPosition.z = BoardLayerDefinition.GetLayerLocalZ(layerIndex);
            return gridPosition;
        }

        public static Transform GetTileParent(Transform boardRoot, int layerIndex)
        {
            if (boardRoot == null)
            {
                return null;
            }

            if (!BoardLayerDefinition.IsValidLayerIndex(layerIndex))
            {
                Debug.LogWarning("[BoardLayerLayout] Layer index out of range: " + layerIndex);
                return null;
            }

            return boardRoot.Find(BoardRootController.GetLayerContainerName(layerIndex));
        }
    }
}
