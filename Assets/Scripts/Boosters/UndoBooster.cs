using MahjongGame.Board;
using MahjongGame.Tiles;
using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Boosters
{
    public sealed class UndoBooster : MonoBehaviour
    {
        [SerializeField] private Transform boardRootTransform;
        [SerializeField] private TrayController trayController;

        private UndoMoveRecord _lastMoveRecord;

        private void Awake()
        {
            ResolveComponents();
        }

        private void OnEnable()
        {
            TileMovementEvents.TileMovementStarted += HandleTileMovementStarted;
            BoosterEvents.BoosterRuntimeReset += HandleBoosterRuntimeReset;
        }

        private void OnDisable()
        {
            TileMovementEvents.TileMovementStarted -= HandleTileMovementStarted;
            BoosterEvents.BoosterRuntimeReset -= HandleBoosterRuntimeReset;
        }

        public bool HasUndoTarget => _lastMoveRecord != null && _lastMoveRecord.CanUndo();

        public bool TryExecuteUndo()
        {
            ResolveComponents();
            if (_lastMoveRecord == null || !_lastMoveRecord.CanUndo())
            {
                return false;
            }

            TrayController resolvedTrayController = ResolveTrayController();
            Transform boardRoot = ResolveBoardRootTransform();
            if (resolvedTrayController == null || boardRoot == null)
            {
                return false;
            }

            Tile tile = _lastMoveRecord.Tile;
            if (!resolvedTrayController.TryReleaseStoredTile(tile, out _))
            {
                return false;
            }

            BoardLayerVisualController layerVisualController = boardRoot.GetComponent<BoardLayerVisualController>();
            if (layerVisualController == null)
            {
                return false;
            }

            TileBoardPosition boardPosition = _lastMoveRecord.OriginalBoardPosition;
            layerVisualController.PlaceTile(
                tile,
                boardPosition.LayerIndex,
                boardPosition.GridCoordinate);

            tile.SetState(_lastMoveRecord.RestoreState);
            tile.SetColliderEnabled(true);
            tile.transform.localScale = new Vector3(
                BoardGridDefinition.DefaultCellWidth,
                BoardGridDefinition.DefaultCellHeight,
                1f);

            _lastMoveRecord = null;
            return true;
        }

        internal void SetLastMoveRecordForValidation(UndoMoveRecord moveRecord)
        {
            _lastMoveRecord = moveRecord;
        }

        private void HandleTileMovementStarted(TileMovementRequest request)
        {
            _lastMoveRecord = UndoMoveRecord.FromMovementRequest(request);
        }

        private void HandleBoosterRuntimeReset()
        {
            _lastMoveRecord = null;
        }

        private void ResolveComponents()
        {
            if (trayController == null)
            {
                Transform gameplayRoot = transform.parent;
                if (gameplayRoot != null)
                {
                    trayController = gameplayRoot.GetComponent<TrayController>();
                }
            }

            if (boardRootTransform == null)
            {
                Transform gameplayRoot = transform.parent;
                if (gameplayRoot != null)
                {
                    boardRootTransform = gameplayRoot.Find("BoardRoot");
                }
            }

            if (boardRootTransform == null)
            {
                boardRootTransform = BoardTileOccupancyQuery.ResolveBoardRootFromScene();
            }
        }

        private TrayController ResolveTrayController()
        {
            ResolveComponents();
            return trayController;
        }

        private Transform ResolveBoardRootTransform()
        {
            ResolveComponents();
            return boardRootTransform;
        }
    }
}
