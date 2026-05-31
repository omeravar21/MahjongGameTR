using MahjongGame.Board;
using MahjongGame.ClosedTiles;
using UnityEngine;

namespace MahjongGame.Tiles
{
    public sealed class TileInteractionController : MonoBehaviour
    {
        [SerializeField] private TileSelectabilityChecker selectabilityChecker;
        [SerializeField] private TileMovementController movementController;
        [SerializeField] private ClosedTileController closedTileController;

        private void Awake()
        {
            if (selectabilityChecker == null)
            {
                selectabilityChecker = GetComponent<TileSelectabilityChecker>();
            }

            if (movementController == null)
            {
                movementController = GetComponent<TileMovementController>();
            }

            if (closedTileController == null)
            {
                closedTileController = GetComponent<ClosedTileController>();
            }
        }

        private void OnEnable()
        {
            TileSelectionEvents.TileSelectionRequested += HandleSelectionRequested;
        }

        private void OnDisable()
        {
            TileSelectionEvents.TileSelectionRequested -= HandleSelectionRequested;
        }

        public bool TryHandleInteraction(Tile tile, out TileInteractionResult result)
        {
            if (tile == null)
            {
                result = TileInteractionResult.Rejected(TileInteractionBlockReason.InvalidTile);
                return false;
            }

            Transform boardRoot = ResolveBoardRoot();
            if (boardRoot == null)
            {
                result = TileInteractionResult.Rejected(TileInteractionBlockReason.MissingSceneWiring);
                TileInteractionEvents.RaiseTileInteractionRejected(tile, result);
                return false;
            }

            if (!TryValidateSelectability(boardRoot, tile, out TileInteractionBlockReason selectabilityReason))
            {
                result = TileInteractionResult.Rejected(selectabilityReason);
                TileInteractionEvents.RaiseTileInteractionRejected(tile, result);
                return false;
            }

            if (closedTileController != null && closedTileController.TryRevealClosedTile(tile))
            {
                result = TileInteractionResult.Accepted();
                TileInteractionEvents.RaiseTileInteractionAccepted(tile, result);
                return true;
            }

            if (closedTileController != null && closedTileController.RequiresTrayMove(tile))
            {
                result = TileInteractionResult.Rejected(TileInteractionBlockReason.ClosedTileAwaitingSecondTap);
                TileInteractionEvents.RaiseTileInteractionRejected(tile, result);
                return false;
            }

            if (movementController == null)
            {
                result = TileInteractionResult.Rejected(TileInteractionBlockReason.MissingSceneWiring);
                TileInteractionEvents.RaiseTileInteractionRejected(tile, result);
                return false;
            }

            if (!movementController.TryBeginMovement(tile, out TileInteractionBlockReason movementReason))
            {
                result = TileInteractionResult.Rejected(movementReason);
                TileInteractionEvents.RaiseTileInteractionRejected(tile, result);
                return false;
            }

            result = TileInteractionResult.Accepted();
            TileInteractionEvents.RaiseTileInteractionAccepted(tile, result);
            return true;
        }

        private void HandleSelectionRequested(TileSelectionRequest selectionRequest)
        {
            if (selectionRequest == null)
            {
                return;
            }

            TryHandleInteraction(selectionRequest.Tile, out _);
        }

        private bool TryValidateSelectability(
            Transform boardRoot,
            Tile tile,
            out TileInteractionBlockReason blockReason)
        {
            blockReason = TileInteractionBlockReason.None;

            if (selectabilityChecker != null)
            {
                if (!selectabilityChecker.TryValidate(tile, out TileSelectabilityResult result))
                {
                    blockReason = MapSelectabilityReason(result.BlockReason);
                    return false;
                }

                return true;
            }

            if (!TileSelectabilityChecker.TryValidate(boardRoot, tile, out TileSelectabilityResult fallbackResult))
            {
                blockReason = MapSelectabilityReason(fallbackResult.BlockReason);
                return false;
            }

            return true;
        }

        private Transform ResolveBoardRoot()
        {
            if (selectabilityChecker != null && selectabilityChecker.BoardRootTransform != null)
            {
                return selectabilityChecker.BoardRootTransform;
            }

            Transform boardRoot = transform.Find("BoardRoot");
            if (boardRoot != null)
            {
                return boardRoot;
            }

            return BoardTileOccupancyQuery.ResolveBoardRootFromScene();
        }

        internal static TileInteractionBlockReason MapSelectabilityReason(TileSelectabilityBlockReason reason)
        {
            switch (reason)
            {
                case TileSelectabilityBlockReason.BlockedByUpperTile:
                    return TileInteractionBlockReason.BlockedByUpperTile;
                case TileSelectabilityBlockReason.BlockedByBothSides:
                    return TileInteractionBlockReason.BlockedByBothSides;
                case TileSelectabilityBlockReason.InvalidState:
                    return TileInteractionBlockReason.InvalidState;
                default:
                    return TileInteractionBlockReason.InvalidState;
            }
        }
    }
}
