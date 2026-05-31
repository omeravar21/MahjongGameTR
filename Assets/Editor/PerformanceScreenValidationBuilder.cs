#if UNITY_EDITOR
using System.Text;
using MahjongGame.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class PerformanceScreenValidationBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Validate Performance Screen")]
        public static void ValidatePerformanceScreen()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);

            if (!PerformanceScreenController.HasRequiredLayout())
            {
                PerformanceScreenController.BuildLayout();
            }

            StringBuilder reportBuilder = new StringBuilder();
            bool passed = PerformanceScreenSystemValidator.Validate(gameplayRoot, reportBuilder);

            if (passed)
            {
                Debug.Log("[PerformanceScreenValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[PerformanceScreenValidationBuilder] " + reportBuilder.ToString().TrimEnd());
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
