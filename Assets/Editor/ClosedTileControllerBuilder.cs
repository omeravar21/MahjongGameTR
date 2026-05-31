#if UNITY_EDITOR
using MahjongGame.ClosedTiles;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class ClosedTileControllerBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Closed Tile Controller")]
        public static void BuildClosedTileController()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[ClosedTileControllerBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            ClosedTileController closedTileController = gameplayRoot.GetComponent<ClosedTileController>();
            if (closedTileController == null)
            {
                closedTileController = gameplayRoot.gameObject.AddComponent<ClosedTileController>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[ClosedTileControllerBuilder] ClosedTileController wired on GameplayRoot.");
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
