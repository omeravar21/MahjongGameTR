using MahjongGame.BoardGeneration;
using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.Board
{
    public sealed class BoardSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;

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
                GameObject tileObject = Instantiate(tilePrefab);
                if (tileObject == null)
                {
                    continue;
                }

                tileObject.name = "Tile_"
                    + assignment.TileId
                    + "_L"
                    + assignment.Position.LayerIndex
                    + "_"
                    + assignment.Position.GridCoordinate.Column
                    + "_"
                    + assignment.Position.GridCoordinate.Row;

                Tile tile = tileObject.GetComponent<Tile>();
                if (tile == null)
                {
                    DestroyTileObject(tileObject);
                    continue;
                }

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

                Tile[] tiles = layerContainer.GetComponentsInChildren<Tile>(includeInactive: true);
                for (int i = 0; i < tiles.Length; i++)
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
