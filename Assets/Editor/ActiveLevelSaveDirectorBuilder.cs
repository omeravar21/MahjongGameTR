#if UNITY_EDITOR
using MahjongGame.Session;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class ActiveLevelSaveDirectorBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Active Level Save")]
        public static void BuildActiveLevelSave()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[ActiveLevelSaveDirectorBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            if (gameplayRoot.GetComponent<ActiveLevelSaveDirector>() == null)
            {
                gameplayRoot.gameObject.AddComponent<ActiveLevelSaveDirector>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[ActiveLevelSaveDirectorBuilder] ActiveLevelSaveDirector wired on GameplayRoot.");
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
