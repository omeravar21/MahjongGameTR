#if UNITY_EDITOR
using System.Text;
using MahjongGame.Board;
using MahjongGame.BoardGeneration;
using MahjongGame.Tiles;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class BoardSpawnerValidationBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";
        private const string TilePrefabPath = "Assets/Prefabs/Tiles/Tile.prefab";

        [MenuItem("MahjongGame/Validate BoardSpawner")]
        public static void ValidateBoardSpawner()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = BoardGenerationPipelineSystemValidator.Validate(reportBuilder);

            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform boardRoot = FindBoardRoot(scene);
            if (boardRoot != null)
            {
                passed &= BoardSpawnerSystemValidator.Validate(boardRoot, reportBuilder);
            }
            else
            {
                reportBuilder.AppendLine("[FAIL] BoardRoot was not found in GameScene.");
                passed = false;
            }

            if (passed)
            {
                Debug.Log("[BoardSpawnerValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[BoardSpawnerValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }

        [MenuItem("MahjongGame/Build BoardSpawner")]
        public static void BuildBoardSpawner()
        {
            Tile tilePrefab = LoadTilePrefabTemplate();
            if (tilePrefab == null)
            {
                Debug.LogError("[BoardSpawnerValidationBuilder] Tile prefab was not found at " + TilePrefabPath + ".");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform boardRoot = FindBoardRoot(scene);
            if (boardRoot == null)
            {
                Debug.LogError("[BoardSpawnerValidationBuilder] BoardRoot was not found in GameScene.");
                return;
            }

            BoardSpawner boardSpawner = boardRoot.GetComponent<BoardSpawner>();
            if (boardSpawner == null)
            {
                boardSpawner = boardRoot.gameObject.AddComponent<BoardSpawner>();
            }

            SerializedObject spawnerObject = new SerializedObject(boardSpawner);
            spawnerObject.FindProperty("tilePrefab").objectReferenceValue = tilePrefab;
            spawnerObject.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[BoardSpawnerValidationBuilder] BoardSpawner wired on BoardRoot in GameScene.");
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

        private static Tile LoadTilePrefabTemplate()
        {
            GameObject prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(TilePrefabPath);
            return prefabRoot != null ? prefabRoot.GetComponent<Tile>() : null;
        }
    }
}
#endif
