#if UNITY_EDITOR
using MahjongGame.Tiles;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class TileSelectionBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Tile Selection")]
        public static void BuildTileSelection()
        {
            WireTileSelection(scene => Debug.Log("[TileSelectionBuilder] Tile selection controller wired on GameplayRoot."));
        }

        [MenuItem("MahjongGame/Build Tile Selectability")]
        public static void BuildTileSelectability()
        {
            WireTileSelection(scene => Debug.Log("[TileSelectionBuilder] Tile selectability checker wired on GameplayRoot."));
        }

        private static void WireTileSelection(System.Action<Scene> onComplete)
        {
            TilePrefabBuilder.BuildTilePrefab();

            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[TileSelectionBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            TileSelectionController selectionController = gameplayRoot.GetComponent<TileSelectionController>();
            if (selectionController == null)
            {
                selectionController = gameplayRoot.gameObject.AddComponent<TileSelectionController>();
            }

            TileSelectabilityChecker selectabilityChecker = gameplayRoot.GetComponent<TileSelectabilityChecker>();
            if (selectabilityChecker == null)
            {
                selectabilityChecker = gameplayRoot.gameObject.AddComponent<TileSelectabilityChecker>();
            }

            Transform boardRoot = gameplayRoot.Find("BoardRoot");
            if (boardRoot != null)
            {
                SerializedObject checkerObject = new SerializedObject(selectabilityChecker);
                checkerObject.FindProperty("boardRootTransform").objectReferenceValue = boardRoot;
                checkerObject.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            onComplete?.Invoke(scene);
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
