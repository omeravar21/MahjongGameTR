using System.Collections.Generic;
using MahjongGame.BoardGeneration;
using MahjongGame.Core.Save;
using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.Board
{
    public sealed class BoardSpawner : MonoBehaviour
    {
        [SerializeField] private Tile tilePrefab;

        public bool HasTilePrefab => tilePrefab != null;

        public bool Spawn(BoardData boardData)
        {
            if (boardData == null)
            {
                Debug.LogWarning("[BoardSpawner] BoardData is not available.");
                return false;
            }

            if (!boardData.IsValidated)
            {
                Debug.LogWarning("[BoardSpawner] BoardData is not validated.");
                return false;
            }

            if (tilePrefab == null)
            {
                Debug.LogWarning("[BoardSpawner] Tile prefab is not configured.");
                return false;
            }

            Transform boardRoot = transform;
            BoardLayerVisualController layerVisualController = boardRoot.GetComponent<BoardLayerVisualController>();
            if (layerVisualController == null)
            {
                Debug.LogWarning("[BoardSpawner] BoardLayerVisualController is missing on BoardRoot.");
                return false;
            }

            ClearRuntimeTiles(boardRoot);
            BoardLayerVisualController.EnforceLayerContainerOrder(boardRoot);

            int spawnedCount = 0;
            for (int index = 0; index < boardData.TileAssignments.Count; index++)
            {
                TileSymbolAssignment assignment = boardData.TileAssignments[index];
                Tile tile = InstantiateTileFromPrefab();
                if (tile == null)
                {
                    continue;
                }

                GameObject tileObject = tile.gameObject;
                tileObject.name = "Tile_"
                    + assignment.TileId
                    + "_L"
                    + assignment.Position.LayerIndex
                    + "_"
                    + assignment.Position.GridCoordinate.Column
                    + "_"
                    + assignment.Position.GridCoordinate.Row;

                TileType tileType = assignment.IsClosed
                    ? TileType.Closed
                    : assignment.IsJoker
                        ? TileType.Joker
                        : TileType.Normal;
                TileData tileData = new TileData(
                    assignment.TileId,
                    assignment.Position.GridCoordinate,
                    assignment.Position.LayerIndex,
                    tileType,
                    isClosed: assignment.IsClosed,
                    isJoker: assignment.IsJoker,
                    symbolId: assignment.SymbolId);
                tile.Initialize(tileData);
                layerVisualController.PlaceTile(
                    tile,
                    assignment.Position.LayerIndex,
                    assignment.Position.GridCoordinate);
                spawnedCount++;
            }

            Debug.Log(
                "[BoardSpawner] Spawned runtime board: level="
                + boardData.LevelNumber
                + ", tiles="
                + spawnedCount
                + ", seed="
                + boardData.Seed
                + ".");

            return spawnedCount == boardData.TileCount;
        }

        public bool RestoreBoardTiles(SavedBoardState boardState)
        {
            if (boardState == null || boardState.tiles == null || boardState.tiles.Length == 0)
            {
                Debug.LogWarning("[BoardSpawner] Saved board state is not available.");
                return false;
            }

            if (tilePrefab == null)
            {
                Debug.LogWarning("[BoardSpawner] Tile prefab is not configured.");
                return false;
            }

            Transform boardRoot = transform;
            BoardLayerVisualController layerVisualController = boardRoot.GetComponent<BoardLayerVisualController>();
            if (layerVisualController == null)
            {
                Debug.LogWarning("[BoardSpawner] BoardLayerVisualController is missing on BoardRoot.");
                return false;
            }

            ClearRuntimeTiles(boardRoot);
            BoardLayerVisualController.EnforceLayerContainerOrder(boardRoot);

            int restoredCount = 0;
            for (int index = 0; index < boardState.tiles.Length; index++)
            {
                SavedTileState savedTile = boardState.tiles[index];
                Tile tile = SpawnSingleTile(savedTile, boardRoot, layerVisualController);
                if (tile != null)
                {
                    restoredCount++;
                }
            }

            Debug.Log("[BoardSpawner] Restored runtime board from save: tiles=" + restoredCount + ".");
            return restoredCount == boardState.tiles.Length;
        }

        public Tile SpawnSingleTile(SavedTileState savedTile)
        {
            Transform boardRoot = transform;
            BoardLayerVisualController layerVisualController = boardRoot.GetComponent<BoardLayerVisualController>();
            if (layerVisualController == null)
            {
                return null;
            }

            return SpawnSingleTile(savedTile, boardRoot, layerVisualController);
        }

        private Tile SpawnSingleTile(
            SavedTileState savedTile,
            Transform boardRoot,
            BoardLayerVisualController layerVisualController)
        {
            if (savedTile == null || tilePrefab == null || layerVisualController == null)
            {
                return null;
            }

            BoardGridCoordinate gridCoordinate = new BoardGridCoordinate(savedTile.column, savedTile.row);
            if (!gridCoordinate.IsValid || !BoardLayerDefinition.IsValidLayerIndex(savedTile.layerIndex))
            {
                return null;
            }

            Tile tile = InstantiateTileFromPrefab();
            if (tile == null)
            {
                return null;
            }

            tile.gameObject.name = "Tile_"
                + savedTile.tileId
                + "_L"
                + savedTile.layerIndex
                + "_"
                + savedTile.column
                + "_"
                + savedTile.row;

            TileType tileType = savedTile.isClosed
                ? TileType.Closed
                : savedTile.isJoker
                    ? TileType.Joker
                    : TileType.Normal;
            TileData tileData = new TileData(
                savedTile.tileId,
                gridCoordinate,
                savedTile.layerIndex,
                tileType,
                isClosed: savedTile.isClosed,
                isJoker: savedTile.isJoker,
                symbolId: savedTile.symbolId);
            tile.Initialize(tileData);

            TileState restoredState = (TileState)savedTile.tileState;
            if (restoredState == TileState.InTray || restoredState == TileState.MovingToTray)
            {
                restoredState = TileState.OnBoard;
            }

            tile.SetState(restoredState);
            layerVisualController.PlaceTile(tile, savedTile.layerIndex, gridCoordinate);
            return tile;
        }

        private Tile InstantiateTileFromPrefab()
        {
            if (tilePrefab == null)
            {
                return null;
            }

            Object clone = Object.Instantiate(tilePrefab);
            return clone as Tile;
        }

        public void ClearRuntimeTiles(Transform boardRoot)
        {
            if (boardRoot == null)
            {
                return;
            }

            for (int layerIndex = 0; layerIndex < BoardLayerDefinition.MaxLayerCount; layerIndex++)
            {
                Transform layerContainer = boardRoot.Find(BoardRootController.GetLayerContainerName(layerIndex));
                if (layerContainer == null)
                {
                    continue;
                }

                List<Tile> tiles = BoardTileOccupancyQuery.CollectTilesFromTransform(
                    layerContainer,
                    includeInactive: true);
                for (int i = 0; i < tiles.Count; i++)
                {
                    Tile tile = tiles[i];
                    if (tile == null)
                    {
                        continue;
                    }

                    DestroyTileObject(tile.gameObject);
                }
            }
        }

        private static void DestroyTileObject(GameObject tileObject)
        {
            if (tileObject == null)
            {
                return;
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
