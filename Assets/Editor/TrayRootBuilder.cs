#if UNITY_EDITOR
using MahjongGame.Tray;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class TrayRootBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Tray Root")]
        public static void BuildTrayRoot()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[TrayRootBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            Transform trayRoot = EnsureTrayRoot(gameplayRoot);
            ApplyTrayRootPresentation(trayRoot);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[TrayRootBuilder] Tray root hierarchy and presentation wired in GameScene.");
        }

        public static Transform EnsureTrayRoot(Transform gameplayRoot)
        {
            Transform trayRoot = gameplayRoot.Find(TrayRootDefinition.TrayRootName);
            if (trayRoot == null)
            {
                GameObject trayRootObject = new GameObject(TrayRootDefinition.TrayRootName);
                trayRoot = trayRootObject.transform;
                trayRoot.SetParent(gameplayRoot, false);
                trayRoot.localPosition = Vector3.zero;
                trayRoot.localRotation = Quaternion.identity;
                trayRoot.localScale = Vector3.one;
            }

            return trayRoot;
        }

        public static void ApplyTrayRootPresentation(Transform trayRoot)
        {
            if (trayRoot == null)
            {
                return;
            }

            if (trayRoot.GetComponent<TrayRootController>() == null)
            {
                trayRoot.gameObject.AddComponent<TrayRootController>();
            }

            TrayRootController.BuildTrayHierarchy(trayRoot);

            Transform trayContainer = trayRoot.Find(TrayRootDefinition.TrayContainerName);
            if (trayContainer != null)
            {
                TraySlotVisualController.BuildAllSlotVisuals(trayContainer);
            }

            TrayFrameVisualController.BuildFrameVisual(trayRoot);
            TrayRootController.EnforceTrayHierarchyOrder(trayRoot);
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
