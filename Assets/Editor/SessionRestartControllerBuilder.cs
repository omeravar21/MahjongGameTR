#if UNITY_EDITOR
using MahjongGame.Board;
using MahjongGame.Session;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class SessionRestartControllerBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";
        private const string TilePrefabPath = "Assets/Prefabs/Tiles/Tile.prefab";

        [MenuItem("MahjongGame/Build Session Restart Controller")]
        public static void BuildSessionRestartController()
        {
            GameObject tilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(TilePrefabPath);
            if (tilePrefab == null)
            {
                Debug.LogError("[SessionRestartControllerBuilder] Tile prefab was not found at " + TilePrefabPath + ".");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[SessionRestartControllerBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            SessionRestartController restartController = gameplayRoot.GetComponent<SessionRestartController>();
            if (restartController == null)
            {
                restartController = gameplayRoot.gameObject.AddComponent<SessionRestartController>();
            }

            Transform boardRoot = gameplayRoot.Find("BoardRoot");
            if (boardRoot == null)
            {
                Debug.LogError("[SessionRestartControllerBuilder] BoardRoot was not found under GameplayRoot.");
                return;
            }

            BoardPreviewSpawner previewSpawner = boardRoot.GetComponent<BoardPreviewSpawner>();
            if (previewSpawner == null)
            {
                previewSpawner = boardRoot.gameObject.AddComponent<BoardPreviewSpawner>();
            }

            SerializedObject spawnerObject = new SerializedObject(previewSpawner);
            spawnerObject.FindProperty("tilePrefab").objectReferenceValue = tilePrefab;
            spawnerObject.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[SessionRestartControllerBuilder] Session restart controller and board preview spawner wired.");
        }

        private static Transform FindGameplayRoot(Scene scene)
        {
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                if (rootObject.name == "GameplayRoot")
                {
                    return rootObject.transform;
                }
            }

            return null;
        }
    }
}
#endif
