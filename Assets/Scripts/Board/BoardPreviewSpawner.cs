using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.Board
{
    public sealed class BoardPreviewSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;

        public bool TryResetBoard()
        {
            Transform boardRoot = transform;
            DestroyAllRuntimeTiles(boardRoot);
            return SpawnDefaultPreview(boardRoot);
        }

        public void DestroyAllRuntimeTiles(Transform boardRoot)
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

        private bool SpawnDefaultPreview(Transform boardRoot)
        {
            if (boardRoot == null || tilePrefab == null)
            {
                Debug.LogWarning("[BoardPreviewSpawner] Board root or tile prefab is not configured.");
                return false;
            }

            BoardLayerVisualController layerVisualController = boardRoot.GetComponent<BoardLayerVisualController>();
            if (layerVisualController == null)
            {
                Debug.LogWarning("[BoardPreviewSpawner] BoardLayerVisualController is missing on BoardRoot.");
                return false;
            }

            BoardPreviewTileSpec[] previewTiles = BoardPreviewLayoutDefinition.DefaultTiles;
            BoardLayerVisualController.EnforceLayerContainerOrder(boardRoot);

            for (int i = 0; i < previewTiles.Length; i++)
            {
                BoardPreviewTileSpec spec = previewTiles[i];
                GameObject tileObject = Instantiate(tilePrefab);
                if (tileObject == null)
                {
                    continue;
                }

                tileObject.name = BoardPreviewLayoutDefinition.GetPreviewTileName(spec);
                Tile tile = tileObject.GetComponent<Tile>();
                if (tile == null)
                {
                    DestroyTileObject(tileObject);
                    continue;
                }

                BoardGridCoordinate coordinate = new BoardGridCoordinate(spec.Column, spec.Row);
                TileData tileData = new TileData(
                    spec.TileId,
                    coordinate,
                    spec.LayerIndex,
                    TileType.Normal,
                    symbolId: spec.TileId % 10);
                tile.Initialize(tileData);
                layerVisualController.PlaceTile(tile, spec.LayerIndex, coordinate);
            }

            return true;
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
