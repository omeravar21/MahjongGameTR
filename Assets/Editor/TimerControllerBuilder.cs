#if UNITY_EDITOR
using MahjongGame.Timer;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class TimerControllerBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Timer Controller")]
        public static void BuildTimerController()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[TimerControllerBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            Transform timerRoot = gameplayRoot.Find("TimerRoot");
            if (timerRoot == null)
            {
                Debug.LogError("[TimerControllerBuilder] TimerRoot was not found under GameplayRoot.");
                return;
            }

            TimerController timerController = timerRoot.GetComponent<TimerController>();
            if (timerController == null)
            {
                timerController = timerRoot.gameObject.AddComponent<TimerController>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[TimerControllerBuilder] TimerController wired on TimerRoot.");
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
