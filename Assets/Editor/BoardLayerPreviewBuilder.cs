#if UNITY_EDITOR
using MahjongGame.Board;
using MahjongGame.Tiles;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class BoardLayerPreviewBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";
        private const string TilePrefabPath = "Assets/Prefabs/Tiles/Tile.prefab";

        [MenuItem("MahjongGame/Build Layer Preview")]
        public static void BuildLayerPreview()
        {
            GameObject tilePrefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(TilePrefabPath);
            Tile tilePrefab = tilePrefabRoot != null ? tilePrefabRoot.GetComponent<Tile>() : null;
            if (tilePrefab == null)
            {
                Debug.LogError("[BoardLayerPreviewBuilder] Tile prefab was not found at " + TilePrefabPath + ".");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform boardRoot = FindBoardRoot(scene);
            if (boardRoot == null)
            {
                Debug.LogError("[BoardLayerPreviewBuilder] BoardRoot was not found in GameScene.");
                return;
            }

            BoardRootController.BuildBoardContainers(boardRoot);
            BoardGridVisualController.BuildGridVisual(
                boardRoot,
                BoardGridDefinition.DefaultCellWidth,
                BoardGridDefinition.DefaultCellHeight,
                new Color(0.35f, 0.28f, 0.22f, 0.35f));

            if (boardRoot.GetComponent<BoardLayerVisualController>() == null)
            {
                boardRoot.gameObject.AddComponent<BoardLayerVisualController>();
            }

            BoardPreviewSpawner previewSpawner = boardRoot.GetComponent<BoardPreviewSpawner>();
            if (previewSpawner == null)
            {
                previewSpawner = boardRoot.gameObject.AddComponent<BoardPreviewSpawner>();
            }

            SerializedObject spawnerObject = new SerializedObject(previewSpawner);
            spawnerObject.FindProperty("tilePrefab").objectReferenceValue = tilePrefab;
            spawnerObject.ApplyModifiedPropertiesWithoutUndo();

            BoardLayerVisualController layerVisualController = boardRoot.GetComponent<BoardLayerVisualController>();
            previewSpawner.DestroyAllRuntimeTiles(boardRoot);
            BoardLayerVisualController.EnforceLayerContainerOrder(boardRoot);

            BoardPreviewTileSpec[] previewTiles = BoardPreviewLayoutDefinition.DefaultTiles;
            for (int i = 0; i < previewTiles.Length; i++)
            {
                BoardPreviewTileSpec spec = previewTiles[i];
                GameObject tileObject = PrefabUtility.InstantiatePrefab(tilePrefabRoot) as GameObject;
                if (tileObject == null)
                {
                    Debug.LogError("[BoardLayerPreviewBuilder] Failed to instantiate tile prefab.");
                    continue;
                }

                tileObject.name = BoardPreviewLayoutDefinition.GetPreviewTileName(spec);
                Tile tile = tileObject.GetComponent<Tile>();
                if (tile == null)
                {
                    Object.DestroyImmediate(tileObject);
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

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[BoardLayerPreviewBuilder] Layer preview tiles placed in GameScene.");
        }

        private static Transform FindBoardRoot(Scene scene)
        {
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                Transform gameplayRoot = rootObject.transform.Find("GameplayRoot");
                if (gameplayRoot == null)
                {
                    continue;
                }

                Transform boardRoot = gameplayRoot.Find("BoardRoot");
                if (boardRoot != null)
                {
                    return boardRoot;
                }
            }

            return null;
        }
    }
}
#endif
