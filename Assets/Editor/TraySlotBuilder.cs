#if UNITY_EDITOR
using MahjongGame.Tray;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class TraySlotBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Tray Slots")]
        public static void BuildTraySlots()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[TraySlotBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            Transform trayRoot = TrayRootBuilder.EnsureTrayRoot(gameplayRoot);
            TrayRootBuilder.ApplyTrayRootPresentation(trayRoot);

            Transform trayContainer = trayRoot.Find(TrayRootDefinition.TrayContainerName);
            if (trayContainer == null)
            {
                Debug.LogError("[TraySlotBuilder] TrayContainer was not found under TrayRoot.");
                return;
            }

            TraySlotVisualController.BuildAllSlotVisuals(trayContainer);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[TraySlotBuilder] Tray slot visuals built in GameScene.");
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
