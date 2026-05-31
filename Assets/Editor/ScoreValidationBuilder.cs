#if UNITY_EDITOR
using System.Text;
using MahjongGame.Score;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class ScoreValidationBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Validate Score System")]
        public static void ValidateScoreSystem()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);

            StringBuilder reportBuilder = new StringBuilder();
            bool passed = ScoreSystemValidator.Validate(gameplayRoot, reportBuilder);

            if (passed)
            {
                Debug.Log("[ScoreValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[ScoreValidationBuilder] " + reportBuilder.ToString().TrimEnd());
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
