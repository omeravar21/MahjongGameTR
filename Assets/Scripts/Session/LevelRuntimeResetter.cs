using System.Collections.Generic;
using MahjongGame.Board;
using MahjongGame.BoardGeneration;
using MahjongGame.Matching;
using MahjongGame.Progression;
using MahjongGame.Tiles;
using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Session
{
    public static class LevelRuntimeResetter
    {
        public static bool TryResetRuntimeState(Transform gameplayRoot)
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
            return true;
        }

        public static bool TryResetLevel(Transform gameplayRoot)
        {
            if (gameplayRoot == null)
            {
                return false;
            }

            TryResetRuntimeState(gameplayRoot);

            Transform boardRoot = gameplayRoot.Find("BoardRoot");
            if (boardRoot == null)
            {
                Debug.LogWarning("[LevelRuntimeResetter] BoardRoot was not found under GameplayRoot.");
                return false;
            }

            BoardSpawner boardSpawner = boardRoot.GetComponent<BoardSpawner>();
            if (boardSpawner == null)
            {
                Debug.LogWarning("[LevelRuntimeResetter] BoardSpawner is missing on BoardRoot.");
                return false;
            }

            int levelNumber = PlayerProgressionDirector.HasInstance
                ? PlayerProgressionDirector.Instance.CurrentLevel
                : LevelProgressData.MinLevel;
            BoardData boardData = BoardGenerationPipeline.GenerateBoardData(levelNumber);
            return boardSpawner.Spawn(boardData);
        }

        private static void DestroyOrphanTiles(Transform gameplayRoot)
        {
            List<Tile> tiles = BoardTileOccupancyQuery.CollectTilesFromTransform(
                gameplayRoot,
                includeInactive: true);
            for (int i = 0; i < tiles.Count; i++)
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
