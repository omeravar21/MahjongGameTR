using MahjongGame.Board;
using MahjongGame.ClosedTiles;
using MahjongGame.Rewards;
using MahjongGame.Tiles;
using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Core.Save
{
    public static class ActiveBoardStateRestorer
    {
        public static bool TryRestore(
            Transform gameplayRoot,
            Transform boardRoot,
            BoardSpawner boardSpawner,
            TrayController trayController,
            ClosedTileController closedTileController,
            RewardDirector rewardDirector,
            ActiveLevelStateSaveData savedState)
        {
            if (gameplayRoot == null || boardRoot == null || boardSpawner == null || savedState == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(savedState.boardStateJson))
            {
                return false;
            }

            SavedBoardState boardState = JsonUtility.FromJson<SavedBoardState>(savedState.boardStateJson);
            if (boardState == null || boardState.tiles == null || boardState.tiles.Length == 0)
            {
                return false;
            }

            if (!boardSpawner.RestoreBoardTiles(boardState))
            {
                return false;
            }

            RegisterSpecialTiles(boardRoot, closedTileController, rewardDirector);
            RestoreTrayTiles(boardRoot, boardSpawner, trayController, savedState.trayStateJson);
            RestoreClosedTileStates(closedTileController, boardRoot, savedState.closedTileStateJson);
            return true;
        }

        private static void RegisterSpecialTiles(
            Transform boardRoot,
            ClosedTileController closedTileController,
            RewardDirector rewardDirector)
        {
            if (closedTileController != null)
            {
                closedTileController.ResetRuntimeState();
            }

            JokerTileController jokerTileController = rewardDirector != null
                ? rewardDirector.GetJokerTileController()
                : null;
            if (jokerTileController != null)
            {
                jokerTileController.ResetRuntimeState();
            }

            System.Collections.Generic.List<Tile> occupyingTiles =
                BoardTileOccupancyQuery.CollectOccupyingTiles(boardRoot);
            for (int index = 0; index < occupyingTiles.Count; index++)
            {
                Tile tile = occupyingTiles[index];
                if (tile == null)
                {
                    continue;
                }

                if (tile.IsClosed && closedTileController != null)
                {
                    closedTileController.TryRegisterClosedTile(tile);
                }

                if (tile.IsJoker && jokerTileController != null)
                {
                    jokerTileController.TryRegisterJokerTile(tile);
                }
            }
        }

        private static void RestoreTrayTiles(
            Transform boardRoot,
            BoardSpawner boardSpawner,
            TrayController trayController,
            string trayStateJson)
        {
            if (trayController == null || string.IsNullOrEmpty(trayStateJson))
            {
                return;
            }

            SavedTrayState trayState = JsonUtility.FromJson<SavedTrayState>(trayStateJson);
            if (trayState == null || trayState.slots == null)
            {
                return;
            }

            Transform trayRoot = trayController.transform.Find(TrayRootDefinition.TrayRootName);
            if (trayRoot == null)
            {
                trayRoot = GameObject.Find(TrayRootDefinition.TrayRootName)?.transform;
            }

            Transform trayContainer = trayRoot != null
                ? trayRoot.Find(TrayRootDefinition.TrayContainerName)
                : null;
            if (trayContainer == null)
            {
                return;
            }

            System.Collections.Generic.Dictionary<int, Tile> tilesById =
                BuildTileLookup(boardRoot, trayController);

            for (int index = 0; index < trayState.slots.Length; index++)
            {
                SavedTraySlotState slotState = trayState.slots[index];
                if (slotState == null || !TrayRootDefinition.IsValidSlotIndex(slotState.slotIndex))
                {
                    continue;
                }

                Tile tile = ResolveOrSpawnTrayTile(boardSpawner, boardRoot, tilesById, slotState);
                if (tile == null)
                {
                    continue;
                }

                Transform slotTransform = trayContainer.Find(TrayRootDefinition.GetSlotName(slotState.slotIndex));
                if (slotTransform == null)
                {
                    continue;
                }

                trayController.TryRestoreStoredTile(tile, slotState.slotIndex, slotTransform);
            }
        }

        private static Tile ResolveOrSpawnTrayTile(
            BoardSpawner boardSpawner,
            Transform boardRoot,
            System.Collections.Generic.Dictionary<int, Tile> tilesById,
            SavedTraySlotState slotState)
        {
            if (tilesById.TryGetValue(slotState.tileId, out Tile existingTile) && existingTile != null)
            {
                RemoveTileFromBoard(boardRoot, existingTile);
                return existingTile;
            }

            SavedTileState savedTile = new SavedTileState
            {
                tileId = slotState.tileId,
                column = slotState.column,
                row = slotState.row,
                layerIndex = slotState.layerIndex,
                symbolId = slotState.symbolId,
                tileState = slotState.tileState,
                isClosed = slotState.isClosed,
                isJoker = slotState.isJoker
            };

            return boardSpawner.SpawnSingleTile(savedTile);
        }

        private static void RemoveTileFromBoard(Transform boardRoot, Tile tile)
        {
            if (boardRoot == null || tile == null)
            {
                return;
            }

            Transform layerContainer = boardRoot.Find(BoardRootController.GetLayerContainerName(tile.LayerIndex));
            if (layerContainer != null && tile.transform.parent == layerContainer)
            {
                tile.transform.SetParent(boardRoot, true);
            }
        }

        private static System.Collections.Generic.Dictionary<int, Tile> BuildTileLookup(
            Transform boardRoot,
            TrayController trayController)
        {
            System.Collections.Generic.Dictionary<int, Tile> tilesById =
                new System.Collections.Generic.Dictionary<int, Tile>();

            if (boardRoot != null)
            {
                System.Collections.Generic.List<Tile> boardTiles =
                    BoardTileOccupancyQuery.CollectOccupyingTiles(boardRoot);
                for (int index = 0; index < boardTiles.Count; index++)
                {
                    Tile tile = boardTiles[index];
                    if (tile != null)
                    {
                        tilesById[tile.TileId] = tile;
                    }
                }
            }

            if (trayController != null)
            {
                System.Collections.Generic.IReadOnlyList<Tile> trayTiles = trayController.GetTrayTilesInSlotOrder();
                for (int index = 0; index < trayTiles.Count; index++)
                {
                    Tile tile = trayTiles[index];
                    if (tile != null)
                    {
                        tilesById[tile.TileId] = tile;
                    }
                }
            }

            return tilesById;
        }

        private static void RestoreClosedTileStates(
            ClosedTileController closedTileController,
            Transform boardRoot,
            string closedTileStateJson)
        {
            if (closedTileController == null || string.IsNullOrEmpty(closedTileStateJson))
            {
                return;
            }

            SavedClosedTileStateCollection collection =
                JsonUtility.FromJson<SavedClosedTileStateCollection>(closedTileStateJson);
            if (collection == null || collection.entries == null)
            {
                return;
            }

            System.Collections.Generic.List<Tile> boardTiles =
                BoardTileOccupancyQuery.CollectOccupyingTiles(boardRoot);
            System.Collections.Generic.Dictionary<int, Tile> tilesById =
                new System.Collections.Generic.Dictionary<int, Tile>();
            for (int index = 0; index < boardTiles.Count; index++)
            {
                Tile tile = boardTiles[index];
                if (tile != null)
                {
                    tilesById[tile.TileId] = tile;
                }
            }

            for (int index = 0; index < collection.entries.Length; index++)
            {
                SavedClosedTileEntry entry = collection.entries[index];
                if (entry == null)
                {
                    continue;
                }

                tilesById.TryGetValue(entry.tileId, out Tile tile);
                ClosedTileState closedState = (ClosedTileState)entry.closedTileState;
                closedTileController.TrySetClosedTileStateForValidation(entry.tileId, closedState);
                if (tile != null)
                {
                    if (closedState == ClosedTileState.Revealed)
                    {
                        tile.SetState(TileState.Revealed);
                    }
                    else if (closedState == ClosedTileState.Closed)
                    {
                        tile.SetState(TileState.Closed);
                    }
                }
            }
        }
    }
}
