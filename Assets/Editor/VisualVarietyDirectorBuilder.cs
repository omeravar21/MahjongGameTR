#if UNITY_EDITOR
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class VisualVarietyDirectorBuilder
    {
        private const string BootScenePath = "Assets/Scenes/BootScene.unity";

        [MenuItem("MahjongGame/Build Visual Variety Director")]
        public static void BuildVisualVarietyDirector()
        {
            Scene scene = EditorSceneManager.OpenScene(BootScenePath, OpenSceneMode.Single);
            Transform bootstrapRoot = FindBootstrapRoot(scene);
            if (bootstrapRoot == null)
            {
                Debug.LogError("[VisualVarietyDirectorBuilder] BootstrapRoot was not found in BootScene.");
                return;
            }

            VisualVarietyDirector visualVarietyDirector = bootstrapRoot.GetComponent<VisualVarietyDirector>();
            if (visualVarietyDirector == null)
            {
                visualVarietyDirector = bootstrapRoot.gameObject.AddComponent<VisualVarietyDirector>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[VisualVarietyDirectorBuilder] VisualVarietyDirector wired on BootstrapRoot.");
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
