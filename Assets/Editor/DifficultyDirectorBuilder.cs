#if UNITY_EDITOR
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class DifficultyDirectorBuilder
    {
        private const string BootScenePath = "Assets/Scenes/BootScene.unity";

        [MenuItem("MahjongGame/Build Difficulty Director")]
        public static void BuildDifficultyDirector()
        {
            Scene scene = EditorSceneManager.OpenScene(BootScenePath, OpenSceneMode.Single);
            Transform bootstrapRoot = FindBootstrapRoot(scene);
            if (bootstrapRoot == null)
            {
                Debug.LogError("[DifficultyDirectorBuilder] BootstrapRoot was not found in BootScene.");
                return;
            }

            DifficultyDirector difficultyDirector = bootstrapRoot.GetComponent<DifficultyDirector>();
            if (difficultyDirector == null)
            {
                difficultyDirector = bootstrapRoot.gameObject.AddComponent<DifficultyDirector>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[DifficultyDirectorBuilder] DifficultyDirector wired on BootstrapRoot.");
        }

        private static Transform FindBootstrapRoot(Scene scene)
        {
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                if (rootObject.name == "BootstrapRoot")
                {
                    return rootObject.transform;
                }
            }

            return null;
        }
    }
}
#endif
