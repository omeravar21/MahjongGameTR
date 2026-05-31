#if UNITY_EDITOR
using System.Text;
using MahjongGame.Matching;
using MahjongGame.Tray;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class MatchValidationBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Validate Match System")]
        public static void ValidateMatchSystem()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            Transform trayRoot = gameplayRoot != null
                ? gameplayRoot.Find(TrayRootDefinition.TrayRootName)
                : null;

            StringBuilder reportBuilder = new StringBuilder();
            bool passed = MatchSystemValidator.Validate(gameplayRoot, trayRoot, reportBuilder);

            if (passed)
            {
                Debug.Log("[MatchValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[MatchValidationBuilder] " + reportBuilder.ToString().TrimEnd());
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
