#if UNITY_EDITOR
using MahjongGame.Matching;
using MahjongGame.Tray;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class MatchControllerBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Match Controller")]
        public static void BuildMatchController()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[MatchControllerBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            TrayController trayController = gameplayRoot.GetComponent<TrayController>();
            if (trayController == null)
            {
                Debug.LogError("[MatchControllerBuilder] TrayController is missing on GameplayRoot.");
                return;
            }

            MatchController matchController = gameplayRoot.GetComponent<MatchController>();
            if (matchController == null)
            {
                matchController = gameplayRoot.gameObject.AddComponent<MatchController>();
            }

            SerializedObject matchControllerObject = new SerializedObject(matchController);
            matchControllerObject.FindProperty("trayController").objectReferenceValue = trayController;
            matchControllerObject.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[MatchControllerBuilder] Match controller wired on GameplayRoot.");
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
