#if UNITY_EDITOR
using MahjongGame.Tray;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class TrayCapacityBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Tray Capacity")]
        public static void BuildTrayCapacity()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[TrayCapacityBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            Transform trayRoot = TrayRootBuilder.EnsureTrayRoot(gameplayRoot);
            TrayRootBuilder.ApplyTrayRootPresentation(trayRoot);

            if (trayRoot.GetComponent<TrayCapacityController>() == null)
            {
                trayRoot.gameObject.AddComponent<TrayCapacityController>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[TrayCapacityBuilder] Tray capacity controller wired on TrayRoot.");
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
