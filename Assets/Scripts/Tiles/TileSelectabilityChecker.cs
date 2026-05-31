using MahjongGame.Board;
using UnityEngine;

namespace MahjongGame.Tiles
{
    public sealed class TileSelectabilityChecker : MonoBehaviour
    {
        [SerializeField] private Transform boardRootTransform;

        public Transform BoardRootTransform => ResolveBoardRootTransform();

        private void Awake()
        {
            ResolveBoardRootTransform();
        }

        public bool TryValidate(Tile tile, out TileSelectabilityResult result)
        {
            return TryValidate(ResolveBoardRootTransform(), tile, out result);
        }

        public static bool TryValidate(Transform boardRoot, Tile tile, out TileSelectabilityResult result)
        {
            if (!TileSelectionController.CanReceiveSelectionRequest(tile))
            {
                result = TileSelectabilityResult.Blocked(TileSelectabilityBlockReason.InvalidState);
                return false;
            }

            if (boardRoot == null)
            {
                boardRoot = BoardTileOccupancyQuery.ResolveBoardRoot(tile)
                    ?? BoardTileOccupancyQuery.ResolveBoardRootFromScene();
            }

            if (boardRoot == null)
            {
                result = TileSelectabilityResult.Blocked(TileSelectabilityBlockReason.InvalidState);
                return false;
            }

            if (BoardTileOccupancyQuery.HasUpperBlockingTile(boardRoot, tile))
            {
                result = TileSelectabilityResult.Blocked(TileSelectabilityBlockReason.BlockedByUpperTile);
                return false;
            }

            if (BoardTileOccupancyQuery.HasBothSidesBlocked(boardRoot, tile))
            {
                result = TileSelectabilityResult.Blocked(TileSelectabilityBlockReason.BlockedByBothSides);
                return false;
            }

            result = TileSelectabilityResult.Selectable();
            return true;
        }

        public static bool IsSelectable(Transform boardRoot, Tile tile)
        {
            return TryValidate(boardRoot, tile, out TileSelectabilityResult result) && result.IsSelectable;
        }

        private Transform ResolveBoardRootTransform()
        {
            if (boardRootTransform != null && BoardRootController.HasRequiredBoardHierarchy(boardRootTransform))
            {
                return boardRootTransform;
            }

            Transform gameplayBoardRoot = transform.Find("BoardRoot");
            if (gameplayBoardRoot != null && BoardRootController.HasRequiredBoardHierarchy(gameplayBoardRoot))
            {
                boardRootTransform = gameplayBoardRoot;
                return boardRootTransform;
            }

            BoardRootController boardRootController = GetComponentInChildren<BoardRootController>();
            if (boardRootController != null)
            {
                boardRootTransform = boardRootController.BoardRootTransform;
                return boardRootTransform;
            }

            boardRootTransform = BoardTileOccupancyQuery.ResolveBoardRootFromScene();
            return boardRootTransform;
        }
    }
}
