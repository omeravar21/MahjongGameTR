#if UNITY_EDITOR
using System.IO;
using MahjongGame.Board;
using MahjongGame.Tiles;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class TilePrefabBuilder
    {
        private const string TilePrefabPath = "Assets/Prefabs/Tiles/Tile.prefab";
        private const string TileSharedSpritePath = "Assets/Sprites/TileSharedWhite.png";

        [MenuItem("MahjongGame/Build Tile Prefab")]
        public static void BuildTilePrefab()
        {
            GameObject tileObject = new GameObject("Tile");
            try
            {
                Sprite sharedSprite = LoadOrCreateSharedTileSprite();
                if (sharedSprite == null)
                {
                    Debug.LogError("[TilePrefabBuilder] Shared tile sprite is not available.");
                    return;
                }

                Tile tileComponent = tileObject.AddComponent<Tile>();
                TileView tileView = tileObject.AddComponent<TileView>();
                TileView.BuildVisualHierarchy(
                    tileObject.transform,
                    BoardGridDefinition.DefaultCellWidth,
                    BoardGridDefinition.DefaultCellHeight,
                    sharedSprite);
                tileView.CacheReferencesFromHierarchy();
                tileView.ApplyVisualState(TileState.OnBoard, TileType.Normal);
                tileView.ApplySorting(0);

                TileData previewData = new TileData(
                    0,
                    new BoardGridCoordinate(2, 3),
                    0,
                    TileType.Normal);
                tileComponent.Initialize(previewData);
                Tile.EnsureSelectionCollider(tileObject.transform);

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

        public static void BuildTilePrefabBatch()
        {
            BuildTilePrefab();
        }

        private static Sprite LoadOrCreateSharedTileSprite()
        {
            Sprite existingSprite = AssetDatabase.LoadAssetAtPath<Sprite>(TileSharedSpritePath);
            if (existingSprite != null)
            {
                return existingSprite;
            }

            string spritesDirectory = Path.GetDirectoryName(TileSharedSpritePath);
            if (!string.IsNullOrEmpty(spritesDirectory) && !Directory.Exists(spritesDirectory))
            {
                Directory.CreateDirectory(spritesDirectory);
            }

            File.WriteAllBytes(
                TileSharedSpritePath,
                new byte[]
                {
                    0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52,
                    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x08, 0x06, 0x00, 0x00, 0x00, 0x1F, 0x15, 0xC4,
                    0x89, 0x00, 0x00, 0x00, 0x0A, 0x49, 0x44, 0x41, 0x54, 0x78, 0x9C, 0x63, 0x00, 0x01, 0x00, 0x00,
                    0x05, 0x00, 0x01, 0x0D, 0x0A, 0x2D, 0xB4, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE,
                    0x42, 0x60, 0x82
                });

            AssetDatabase.ImportAsset(TileSharedSpritePath, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(TileSharedSpritePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 1f;
                importer.filterMode = FilterMode.Point;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(TileSharedSpritePath);
        }
    }
}
#endif
