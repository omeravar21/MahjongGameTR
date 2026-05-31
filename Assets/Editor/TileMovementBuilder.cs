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

            Transform trayRoot = gameplayRoot.Find("TrayRoot");
            if (trayRoot == null)
            {
                Debug.LogError("[TileMovementBuilder] TrayRoot was not found in GameScene.");
                return;
            }

            Transform trayContainer = EnsureTrayContainer(trayRoot);
            EnsureTraySlots(trayContainer);

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

        private static Transform EnsureTrayContainer(Transform trayRoot)
        {
            Transform trayContainer = trayRoot.Find(TrayMovementLayout.TrayContainerName);
            if (trayContainer != null)
            {
                return trayContainer;
            }

            GameObject containerObject = new GameObject(TrayMovementLayout.TrayContainerName);
            containerObject.transform.SetParent(trayRoot, false);
            containerObject.transform.localPosition = Vector3.zero;
            containerObject.transform.localRotation = Quaternion.identity;
            containerObject.transform.localScale = Vector3.one;
            return containerObject.transform;
        }

        private static void EnsureTraySlots(Transform trayContainer)
        {
            for (int slotIndex = 0; slotIndex < TrayMovementLayout.SlotCount; slotIndex++)
            {
                string slotName = TrayMovementLayout.GetSlotName(slotIndex);
                Transform slotTransform = trayContainer.Find(slotName);
                if (slotTransform == null)
                {
                    GameObject slotObject = new GameObject(slotName);
                    slotTransform = slotObject.transform;
                    slotTransform.SetParent(trayContainer, false);
                }

                slotTransform.localPosition = TrayMovementLayout.GetSlotLocalPosition(slotIndex);
                slotTransform.localRotation = Quaternion.identity;
                slotTransform.localScale = Vector3.one;
            }
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
