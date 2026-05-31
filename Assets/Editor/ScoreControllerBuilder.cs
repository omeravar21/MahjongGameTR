#if UNITY_EDITOR
using MahjongGame.Score;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class ScoreControllerBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Score Controller")]
        public static void BuildScoreController()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[ScoreControllerBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            ScoreController scoreController = gameplayRoot.GetComponent<ScoreController>();
            if (scoreController == null)
            {
                scoreController = gameplayRoot.gameObject.AddComponent<ScoreController>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[ScoreControllerBuilder] ScoreController wired on GameplayRoot.");
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
