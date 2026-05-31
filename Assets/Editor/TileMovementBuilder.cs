#if UNITY_EDITOR
using MahjongGame.Tiles;
using MahjongGame.Tray;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class TileMovementBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Tile Movement")]
        public static void BuildTileMovement()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[TileMovementBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            Transform trayRoot = TrayRootBuilder.EnsureTrayRoot(gameplayRoot);
            TrayRootBuilder.ApplyTrayRootPresentation(trayRoot);

            TileMovementController movementController = gameplayRoot.GetComponent<TileMovementController>();
            if (movementController == null)
            {
                movementController = gameplayRoot.gameObject.AddComponent<TileMovementController>();
            }

            SerializedObject movementObject = new SerializedObject(movementController);
            movementObject.FindProperty("trayRootTransform").objectReferenceValue = trayRoot;
            movementObject.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[TileMovementBuilder] Tile movement controller and tray anchors wired in GameScene.");
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
