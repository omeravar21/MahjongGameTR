#if UNITY_EDITOR
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class LevelRecipeGeneratorBuilder
    {
        private const string BootScenePath = "Assets/Scenes/BootScene.unity";

        [MenuItem("MahjongGame/Build Level Recipe Generator")]
        public static void BuildLevelRecipeGenerator()
        {
            Scene scene = EditorSceneManager.OpenScene(BootScenePath, OpenSceneMode.Single);
            Transform bootstrapRoot = FindBootstrapRoot(scene);
            if (bootstrapRoot == null)
            {
                Debug.LogError("[LevelRecipeGeneratorBuilder] BootstrapRoot was not found in BootScene.");
                return;
            }

            LevelRecipeGenerator levelRecipeGenerator = bootstrapRoot.GetComponent<LevelRecipeGenerator>();
            if (levelRecipeGenerator == null)
            {
                levelRecipeGenerator = bootstrapRoot.gameObject.AddComponent<LevelRecipeGenerator>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[LevelRecipeGeneratorBuilder] LevelRecipeGenerator wired on BootstrapRoot.");
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
