#if UNITY_EDITOR
using MahjongGame.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class MainMenuLayoutBuilder
    {
        private const string MainMenuScenePath = "Assets/Scenes/MainMenuScene.unity";

        public static void ExecuteBuild()
        {
            BuildMainMenuLayout();
        }

        [MenuItem("MahjongGame/Build Main Menu Layout")]
        public static void BuildMainMenuLayout()
        {
            Scene scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);

            MainMenuLayoutController.BuildLayout();

            Transform canvasTransform = MainMenuLayoutController.GetCanvasTransform();
            if (canvasTransform != null)
            {
                DoorPresentationController.BuildDoorPresentation(canvasTransform);
            }

            EnsureMainMenuDirectorComponents(scene);
            RemoveDuplicateProgressionDirector(scene);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[MainMenuLayoutBuilder] Main menu layout and door presentation complete.");
        }

        private static void EnsureMainMenuDirectorComponents(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.name != "MainMenuDirector")
                {
                    continue;
                }

                if (root.GetComponent<MainMenuLayoutController>() == null)
                {
                    root.AddComponent<MainMenuLayoutController>();
                }

                if (root.GetComponent<DoorPresentationController>() == null)
                {
                    root.AddComponent<DoorPresentationController>();
                }

                return;
            }
        }

        private static void RemoveDuplicateProgressionDirector(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.name != "PlayerProgressionDirector")
                {
                    continue;
                }

                Component[] components = root.GetComponents<Component>();
                for (int i = components.Length - 1; i >= 0; i--)
                {
                    Component component = components[i];
                    if (component == null)
                    {
                        continue;
                    }

                    string typeName = component.GetType().FullName;
                    if (typeName == "MahjongGame.Progression.PlayerProgressionDirector")
                    {
                        Object.DestroyImmediate(component);
                    }
                }
            }
        }
    }
}
#endif
