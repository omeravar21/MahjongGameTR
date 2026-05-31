#if UNITY_EDITOR
using MahjongGame.Boosters;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class BoosterEconomyDirectorBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Booster Economy")]
        public static void BuildBoosterEconomy()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[BoosterEconomyDirectorBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            if (gameplayRoot.GetComponent<BoosterEconomyDirector>() == null)
            {
                gameplayRoot.gameObject.AddComponent<BoosterEconomyDirector>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[BoosterEconomyDirectorBuilder] BoosterEconomyDirector wired on GameplayRoot.");
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
