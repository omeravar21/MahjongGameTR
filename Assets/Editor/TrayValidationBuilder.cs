#if UNITY_EDITOR
using System.Text;
using MahjongGame.Tray;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class TrayValidationBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Validate Tray System")]
        public static void ValidateTraySystem()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            Transform trayRoot = gameplayRoot != null
                ? gameplayRoot.Find(TrayRootDefinition.TrayRootName)
                : null;

            StringBuilder reportBuilder = new StringBuilder();
            bool passed = TraySystemValidator.Validate(gameplayRoot, trayRoot, reportBuilder);

            if (passed)
            {
                Debug.Log("[TrayValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[TrayValidationBuilder] " + reportBuilder.ToString().TrimEnd());
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
