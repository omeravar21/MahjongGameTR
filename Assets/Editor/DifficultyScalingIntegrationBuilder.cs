#if UNITY_EDITOR
using System.Text;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class DifficultyScalingIntegrationBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Difficulty Scaling Integration")]
        public static void BuildDifficultyScalingIntegration()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[DifficultyScalingIntegrationBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            if (gameplayRoot.GetComponent<DifficultyScalingController>() == null)
            {
                gameplayRoot.gameObject.AddComponent<DifficultyScalingController>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            StringBuilder reportBuilder = new StringBuilder();
            bool passed = DifficultyScalingSystemValidator.Validate(gameplayRoot, reportBuilder);
            if (passed)
            {
                Debug.Log("[DifficultyScalingIntegrationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[DifficultyScalingIntegrationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }

        [MenuItem("MahjongGame/Validate Difficulty Scaling Integration")]
        public static void ValidateDifficultyScalingIntegration()
        {
            Scene scene = EditorSceneManager.GetActiveScene();
            Transform gameplayRoot = FindGameplayRoot(scene);
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = DifficultyScalingSystemValidator.Validate(gameplayRoot, reportBuilder);

            if (passed)
            {
                Debug.Log("[DifficultyScalingIntegrationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[DifficultyScalingIntegrationBuilder] " + reportBuilder.ToString().TrimEnd());
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
