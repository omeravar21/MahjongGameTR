using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.Board
{
    [DefaultExecutionOrder(6)]
    public sealed class BoardLayerVisualController : MonoBehaviour
    {
        [SerializeField] private Transform boardRootTransform;

        public Transform BoardRootTransform => boardRootTransform != null ? boardRootTransform : transform;

        private void Awake()
        {
            if (boardRootTransform == null)
            {
                boardRootTransform = transform;
            }

            if (!BoardRootController.HasRequiredBoardHierarchy(boardRootTransform))
            {
                BoardRootController.BuildBoardContainers(boardRootTransform);
            }

            EnforceBoardVisualOrder(boardRootTransform);
        }

        public static void EnforceBoardVisualOrder(Transform boardRoot)
        {
            if (boardRoot == null)
            {
                return;
            }

            Transform frameRoot = boardRoot.Find(BoardPresentationDefinition.FrameRootName);
            Transform gridRoot = boardRoot.Find(BoardGridDefinition.GridRootName);
            int nextSiblingIndex = 0;

            if (frameRoot != null)
            {
                frameRoot.SetSiblingIndex(nextSiblingIndex);
                nextSiblingIndex++;
            }

            if (gridRoot != null)
            {
                gridRoot.SetSiblingIndex(nextSiblingIndex);
                nextSiblingIndex++;
            }

            for (int layerIndex = 0; layerIndex < BoardLayerDefinition.MaxLayerCount; layerIndex++)
            {
                Transform layerContainer = boardRoot.Find(BoardRootController.GetLayerContainerName(layerIndex));
                if (layerContainer != null)
                {
                    layerContainer.SetSiblingIndex(nextSiblingIndex);
                    nextSiblingIndex++;
                }
            }
        }

        public static void EnforceLayerContainerOrder(Transform boardRoot)
        {
            EnforceBoardVisualOrder(boardRoot);
        }

        public void PlaceTile(Tile tile, int layerIndex, BoardGridCoordinate coordinate)
        {
            if (tile == null)
            {
                Debug.LogWarning("[BoardLayerVisualController] Tile is not available.");
                return;
            }

            Transform layerParent = BoardLayerLayout.GetTileParent(BoardRootTransform, layerIndex);
            if (layerParent == null)
            {
                Debug.LogWarning("[BoardLayerVisualController] Layer container is not available for index " + layerIndex + ".");
                return;
            }

            Transform tileTransform = tile.transform;
            tileTransform.SetParent(layerParent, false);
            tileTransform.localPosition = BoardLayerLayout.GetTileLocalPosition(coordinate, layerIndex);
            tileTransform.localRotation = Quaternion.identity;

            tile.ApplySortingOrder(layerIndex, coordinate.Row, coordinate.Column);
        }
    }
}
