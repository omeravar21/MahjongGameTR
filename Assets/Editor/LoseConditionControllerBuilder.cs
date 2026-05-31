#if UNITY_EDITOR
using MahjongGame.Session;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class LoseConditionControllerBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Lose Condition Controller")]
        public static void BuildLoseConditionController()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[LoseConditionControllerBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            LoseConditionController loseConditionController = gameplayRoot.GetComponent<LoseConditionController>();
            if (loseConditionController == null)
            {
                loseConditionController = gameplayRoot.gameObject.AddComponent<LoseConditionController>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[LoseConditionControllerBuilder] LoseConditionController wired on GameplayRoot.");
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
