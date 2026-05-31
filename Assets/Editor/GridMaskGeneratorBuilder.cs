#if UNITY_EDITOR
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class GridMaskGeneratorBuilder
    {
        private const string BootScenePath = "Assets/Scenes/BootScene.unity";

        [MenuItem("MahjongGame/Build Grid Mask Generator")]
        public static void BuildGridMaskGenerator()
        {
            Scene scene = EditorSceneManager.OpenScene(BootScenePath, OpenSceneMode.Single);
            Transform bootstrapRoot = FindBootstrapRoot(scene);
            if (bootstrapRoot == null)
            {
                Debug.LogError("[GridMaskGeneratorBuilder] BootstrapRoot was not found in BootScene.");
                return;
            }

            GridMaskGenerator gridMaskGenerator = bootstrapRoot.GetComponent<GridMaskGenerator>();
            if (gridMaskGenerator == null)
            {
                gridMaskGenerator = bootstrapRoot.gameObject.AddComponent<GridMaskGenerator>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[GridMaskGeneratorBuilder] GridMaskGenerator wired on BootstrapRoot.");
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
