using System.Collections.Generic;
using MahjongGame.Board;
using MahjongGame.ClosedTiles;
using MahjongGame.Rewards;
using MahjongGame.Tiles;
using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Core.Save
{
    public static class ActiveBoardStateSerializer
    {
        public static string SerializeBoardState(Transform boardRoot)
        {
            SavedBoardState boardState = new SavedBoardState();
            if (boardRoot != null)
            {
                List<SavedTileState> tiles = CollectBoardTileStates(boardRoot);
                boardState.tiles = tiles.ToArray();
            }

            return JsonUtility.ToJson(boardState);
        }

        public static string SerializeTrayState(TrayController trayController, Transform boardRoot)
        {
            SavedTrayState trayState = new SavedTrayState();
            if (trayController == null)
            {
                return JsonUtility.ToJson(trayState);
            }

            List<SavedTraySlotState> slots = new List<SavedTraySlotState>();
            IReadOnlyList<Tile> trayTiles = trayController.GetTrayTilesInSlotOrder();
            for (int slotIndex = 0; slotIndex < trayTiles.Count; slotIndex++)
            {
                Tile tile = trayTiles[slotIndex];
                if (tile == null)
                {
                    continue;
                }

                SavedTraySlotState slotState = CreateTraySlotState(slotIndex, tile);
                if (slotState != null)
                {
                    slots.Add(slotState);
                }
            }

            trayState.slots = slots.ToArray();
            return JsonUtility.ToJson(trayState);
        }

        public static string SerializeClosedTileState(ClosedTileController closedTileController, Transform boardRoot)
        {
            SavedClosedTileStateCollection collection = new SavedClosedTileStateCollection();
            if (closedTileController == null || boardRoot == null)
            {
                return JsonUtility.ToJson(collection);
            }

            List<SavedClosedTileEntry> entries = new List<SavedClosedTileEntry>();
            List<Tile> boardTiles = BoardTileOccupancyQuery.CollectOccupyingTiles(boardRoot);
            for (int index = 0; index < boardTiles.Count; index++)
            {
                Tile tile = boardTiles[index];
                if (tile == null || !tile.IsClosed)
                {
                    continue;
                }

                if (!closedTileController.TryGetClosedTileState(tile.TileId, out ClosedTileState state))
                {
                    state = ClosedTileState.Closed;
                }

                entries.Add(new SavedClosedTileEntry
                {
                    tileId = tile.TileId,
                    closedTileState = (int)state
                });
            }

            collection.entries = entries.ToArray();
            return JsonUtility.ToJson(collection);
        }

        public static string SerializeMatchedTileIds(Transform boardRoot, TrayController trayController)
        {
            SavedTileIdCollection collection = new SavedTileIdCollection { tileIds = System.Array.Empty<int>() };
            return JsonUtility.ToJson(collection);
        }

        public static string SerializeRemainingTileIds(Transform boardRoot, TrayController trayController)
        {
            List<int> remainingTileIds = new List<int>();
            if (boardRoot != null)
            {
                List<Tile> boardTiles = BoardTileOccupancyQuery.CollectOccupyingTiles(boardRoot);
                for (int index = 0; index < boardTiles.Count; index++)
                {
                    Tile tile = boardTiles[index];
                    if (tile != null)
                    {
                        remainingTileIds.Add(tile.TileId);
                    }
                }
            }

            if (trayController != null)
            {
                IReadOnlyList<Tile> trayTiles = trayController.GetTrayTilesInSlotOrder();
                for (int index = 0; index < trayTiles.Count; index++)
                {
                    Tile tile = trayTiles[index];
                    if (tile != null)
                    {
                        remainingTileIds.Add(tile.TileId);
                    }
                }
            }

            SavedTileIdCollection collection = new SavedTileIdCollection
            {
                tileIds = remainingTileIds.ToArray()
            };
            return JsonUtility.ToJson(collection);
        }

        public static void WriteBoardStateJson(
            ActiveLevelStateSaveData target,
            Transform boardRoot,
            TrayController trayController,
            ClosedTileController closedTileController)
        {
            if (target == null)
            {
                return;
            }

            target.boardStateJson = SerializeBoardState(boardRoot);
            target.trayStateJson = SerializeTrayState(trayController, boardRoot);
            target.closedTileStateJson = SerializeClosedTileState(closedTileController, boardRoot);
            target.matchedTilesJson = SerializeMatchedTileIds(boardRoot, trayController);
            target.remainingTilesJson = SerializeRemainingTileIds(boardRoot, trayController);
        }

        private static List<SavedTileState> CollectBoardTileStates(Transform boardRoot)
        {
            List<SavedTileState> tiles = new List<SavedTileState>();
            List<Tile> boardTiles = BoardTileOccupancyQuery.CollectOccupyingTiles(boardRoot);
            for (int index = 0; index < boardTiles.Count; index++)
            {
                Tile tile = boardTiles[index];
                if (tile == null)
                {
                    continue;
                }

                tiles.Add(CreateSavedTileState(tile));
            }

            return tiles;
        }

        private static SavedTileState CreateSavedTileState(Tile tile)
        {
            return new SavedTileState
            {
                tileId = tile.TileId,
                column = tile.GridCoordinate.Column,
                row = tile.GridCoordinate.Row,
                layerIndex = tile.LayerIndex,
                symbolId = tile.SymbolId,
                tileState = (int)tile.State,
                isClosed = tile.IsClosed,
                isJoker = tile.IsJoker
            };
        }

        private static SavedTraySlotState CreateTraySlotState(int slotIndex, Tile tile)
        {
            TileBoardPosition originalPosition = tile.OriginalBoardPosition;
            return new SavedTraySlotState
            {
                slotIndex = slotIndex,
                tileId = tile.TileId,
                column = originalPosition.GridCoordinate.Column,
                row = originalPosition.GridCoordinate.Row,
                layerIndex = originalPosition.LayerIndex,
                symbolId = tile.SymbolId,
                tileState = (int)tile.State,
                isClosed = tile.IsClosed,
                isJoker = tile.IsJoker
            };
        }
    }
}
