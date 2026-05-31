#if UNITY_EDITOR
using MahjongGame.Tiles;
using MahjongGame.Tray;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class TrayControllerBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Tray Controller")]
        public static void BuildTrayController()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[TrayControllerBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            Transform trayRoot = TrayRootBuilder.EnsureTrayRoot(gameplayRoot);
            TrayRootBuilder.ApplyTrayRootPresentation(trayRoot);

            TrayCapacityController capacityController = trayRoot.GetComponent<TrayCapacityController>();
            if (capacityController == null)
            {
                capacityController = trayRoot.gameObject.AddComponent<TrayCapacityController>();
            }

            TrayController trayController = gameplayRoot.GetComponent<TrayController>();
            if (trayController == null)
            {
                trayController = gameplayRoot.gameObject.AddComponent<TrayController>();
            }

            SerializedObject trayControllerObject = new SerializedObject(trayController);
            trayControllerObject.FindProperty("trayRootTransform").objectReferenceValue = trayRoot;
            trayControllerObject.FindProperty("trayCapacityController").objectReferenceValue = capacityController;
            trayControllerObject.ApplyModifiedPropertiesWithoutUndo();

            TileMovementController movementController = gameplayRoot.GetComponent<TileMovementController>();
            if (movementController != null)
            {
                SerializedObject movementObject = new SerializedObject(movementController);
                movementObject.FindProperty("trayRootTransform").objectReferenceValue = trayRoot;
                movementObject.FindProperty("trayController").objectReferenceValue = trayController;
                movementObject.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[TrayControllerBuilder] Tray controller wired on GameplayRoot.");
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
