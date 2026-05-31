#if UNITY_EDITOR
using MahjongGame.Board;
using MahjongGame.Tiles;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class TilePrefabBuilder
    {
        private const string TilePrefabPath = "Assets/Prefabs/Tiles/Tile.prefab";

        [MenuItem("MahjongGame/Build Tile Prefab")]
        public static void BuildTilePrefab()
        {
            GameObject tileObject = new GameObject("Tile");
            try
            {
                Tile tileComponent = tileObject.AddComponent<Tile>();
                TileView tileView = tileObject.AddComponent<TileView>();
                Tile.BuildTileHierarchy(tileObject.transform);
                tileView.CacheReferencesFromHierarchy();
                tileView.ApplyVisualState(TileState.OnBoard, TileType.Normal);
                tileView.ApplySorting(0);

                TileData previewData = new TileData(
                    0,
                    new BoardGridCoordinate(2, 3),
                    0,
                    TileType.Normal);
                tileComponent.Initialize(previewData);

                PrefabUtility.SaveAsPrefabAsset(tileObject, TilePrefabPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("[TilePrefabBuilder] Tile prefab saved to " + TilePrefabPath + ".");
            }
            finally
            {
                Object.DestroyImmediate(tileObject);
            }
        }
    }
}
#endif