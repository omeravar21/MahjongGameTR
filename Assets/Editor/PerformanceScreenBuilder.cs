#if UNITY_EDITOR
using MahjongGame.Progression;
using MahjongGame.UI;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class PerformanceScreenBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Performance Screen")]
        public static void BuildPerformanceScreen()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[PerformanceScreenBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            Transform uiRoot = gameplayRoot.Find("UIRoot");
            if (uiRoot == null)
            {
                Debug.LogError("[PerformanceScreenBuilder] UIRoot was not found under GameplayRoot.");
                return;
            }

            LevelResultController levelResultController = gameplayRoot.GetComponent<LevelResultController>();
            if (levelResultController == null)
            {
                levelResultController = gameplayRoot.gameObject.AddComponent<LevelResultController>();
            }

            LevelCompletionController levelCompletionController = gameplayRoot.GetComponent<LevelCompletionController>();
            if (levelCompletionController == null)
            {
                levelCompletionController = gameplayRoot.gameObject.AddComponent<LevelCompletionController>();
            }

            DifficultyScalingController difficultyScalingController = gameplayRoot.GetComponent<DifficultyScalingController>();
            if (difficultyScalingController == null)
            {
                difficultyScalingController = gameplayRoot.gameObject.AddComponent<DifficultyScalingController>();
            }

            PerformanceScreenController performanceScreenController = uiRoot.GetComponent<PerformanceScreenController>();
            if (performanceScreenController == null)
            {
                performanceScreenController = uiRoot.gameObject.AddComponent<PerformanceScreenController>();
            }

            PerformanceScreenController.BuildLayout();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[PerformanceScreenBuilder] Performance screen wired on GameScene.");
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
