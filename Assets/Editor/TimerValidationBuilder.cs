#if UNITY_EDITOR
using System.Text;
using MahjongGame.Timer;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class TimerValidationBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Validate Timer System")]
        public static void ValidateTimerSystem()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);

            StringBuilder reportBuilder = new StringBuilder();
            bool passed = TimerSystemValidator.Validate(gameplayRoot, reportBuilder);

            if (passed)
            {
                Debug.Log("[TimerValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[TimerValidationBuilder] " + reportBuilder.ToString().TrimEnd());
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
