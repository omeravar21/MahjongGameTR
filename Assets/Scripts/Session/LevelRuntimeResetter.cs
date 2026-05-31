using MahjongGame.Board;
using MahjongGame.Matching;
using MahjongGame.Tiles;
using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Session
{
    public static class LevelRuntimeResetter
    {
        public static bool TryResetLevel(Transform gameplayRoot)
        {
            if (gameplayRoot == null)
            {
                return false;
            }

            MatchController matchController = gameplayRoot.GetComponent<MatchController>();
            if (matchController != null)
            {
                matchController.ResetProcessingState();
            }

            TileMovementController movementController = gameplayRoot.GetComponent<TileMovementController>();
            if (movementController != null)
            {
                movementController.ResetMovementState();
            }

            TrayController trayController = gameplayRoot.GetComponent<TrayController>();
            if (trayController != null)
            {
                trayController.ResetRuntimeState();
            }

            DestroyOrphanTiles(gameplayRoot);

            Transform boardRoot = gameplayRoot.Find("BoardRoot");
            if (boardRoot == null)
            {
                Debug.LogWarning("[LevelRuntimeResetter] BoardRoot was not found under GameplayRoot.");
                return false;
            }

            BoardPreviewSpawner previewSpawner = boardRoot.GetComponent<BoardPreviewSpawner>();
            if (previewSpawner == null)
            {
                Debug.LogWarning("[LevelRuntimeResetter] BoardPreviewSpawner is missing on BoardRoot.");
                return false;
            }

            return previewSpawner.TryResetBoard();
        }

        private static void DestroyOrphanTiles(Transform gameplayRoot)
        {
            Tile[] tiles = gameplayRoot.GetComponentsInChildren<Tile>(includeInactive: true);
            for (int i = 0; i < tiles.Length; i++)
            {
                Tile tile = tiles[i];
                if (tile == null)
                {
                    continue;
                }

                GameObject tileObject = tile.gameObject;
                if (tileObject == null)
                {
                    continue;
                }

                if (Application.isPlaying)
                {
                    Object.Destroy(tileObject);
                }
                else
                {
                    Object.DestroyImmediate(tileObject);
                }
            }
        }
    }
}
