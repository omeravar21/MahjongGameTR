#if UNITY_EDITOR
using System.Text;
using MahjongGame.Tiles;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class TileInteractionBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Tile Interaction")]
        public static void BuildTileInteraction()
        {
            TileSelectionBuilder.BuildTileSelection();
            TileMovementBuilder.BuildTileMovement();

            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[TileInteractionBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            TileInteractionController interactionController = gameplayRoot.GetComponent<TileInteractionController>();
            if (interactionController == null)
            {
                interactionController = gameplayRoot.gameObject.AddComponent<TileInteractionController>();
            }

            TileSelectabilityChecker selectabilityChecker = gameplayRoot.GetComponent<TileSelectabilityChecker>();
            TileMovementController movementController = gameplayRoot.GetComponent<TileMovementController>();

            SerializedObject interactionObject = new SerializedObject(interactionController);
            interactionObject.FindProperty("selectabilityChecker").objectReferenceValue = selectabilityChecker;
            interactionObject.FindProperty("movementController").objectReferenceValue = movementController;
            interactionObject.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[TileInteractionBuilder] Tile interaction controller wired on GameplayRoot.");
        }

        [MenuItem("MahjongGame/Validate Tile Interaction")]
        public static void ValidateTileInteraction()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = TileInteractionSceneValidator.ValidateGameplayRoot(gameplayRoot, reportBuilder);

            if (passed)
            {
                Debug.Log("[TileInteractionBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[TileInteractionBuilder] " + reportBuilder.ToString().TrimEnd());
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
