#if UNITY_EDITOR
using System.Text;
using MahjongGame.ClosedTiles;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class ClosedTileValidationBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Validate Closed Tile System")]
        public static void ValidateClosedTileSystem()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);

            StringBuilder reportBuilder = new StringBuilder();
            bool passed = ClosedTileSystemValidator.Validate(gameplayRoot, reportBuilder);

            if (passed)
            {
                Debug.Log("[ClosedTileValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[ClosedTileValidationBuilder] " + reportBuilder.ToString().TrimEnd());
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
