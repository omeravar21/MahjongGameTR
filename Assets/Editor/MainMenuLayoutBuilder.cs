#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

            EnsureEventSystem();
            GameObject canvasRoot = EnsureCanvas();
            Transform canvasTransform = canvasRoot.transform;

            ClearMenuChildren(canvasTransform);

            GameObject topBar = CreateRectObject("TopBar", canvasTransform);
            SetStretchTopBar(topBar.GetComponent<RectTransform>());

            CreateMenuButton("ProfileButton", topBar.transform, new Vector2(-220f, -60f), new Vector2(180f, 80f), "Profile");
            CreateMenuButton("ThemeButton", topBar.transform, new Vector2(0f, -60f), new Vector2(180f, 80f), "Theme");
            CreateMenuButton("SettingsButton", topBar.transform, new Vector2(220f, -60f), new Vector2(180f, 80f), "Settings");

            CreateMenuButton("LevelButton", canvasTransform, Vector2.zero, new Vector2(420f, 180f), "LEVEL 1");

            RemoveDuplicateProgressionDirector(scene);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[MainMenuLayoutBuilder] Main menu layout complete.");
        }

        private static void EnsureEventSystem()
        {
            EventSystem existing = Object.FindFirstObjectByType<EventSystem>();
            if (existing != null)
            {
                if (existing.GetComponent<InputSystemUIInputModule>() == null)
                {
                    Object.DestroyImmediate(existing.GetComponent<StandaloneInputModule>());
                    existing.gameObject.AddComponent<InputSystemUIInputModule>();
                }

                return;
            }

            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();
        }

        private static GameObject EnsureCanvas()
        {
            GameObject canvasObject = GameObject.Find("Canvas_MainMenu");
            if (canvasObject == null)
            {
                canvasObject = new GameObject("Canvas_MainMenu");
            }

            RectTransform rectTransform = canvasObject.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Object.DestroyImmediate(canvasObject.GetComponent<Transform>());
                rectTransform = canvasObject.AddComponent<RectTransform>();
            }

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = canvasObject.AddComponent<Canvas>();
            }

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvasObject.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;

            if (canvasObject.GetComponent<GraphicRaycaster>() == null)
            {
                canvasObject.AddComponent<GraphicRaycaster>();
            }

            return canvasObject;
        }

        private static void ClearMenuChildren(Transform canvasTransform)
        {
            for (int i = canvasTransform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(canvasTransform.GetChild(i).gameObject);
            }
        }

        private static GameObject CreateRectObject(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void SetStretchTopBar(RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -40f);
            rectTransform.sizeDelta = new Vector2(0f, 140f);
        }

        private static GameObject CreateMenuButton(string name, Transform parent, Vector2 anchoredPosition, Vector2 size, string label)
        {
            GameObject buttonObject = CreateRectObject(name, parent);
            RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.24f, 0.18f, 0.14f, 0.95f);

            Button button = buttonObject.AddComponent<Button>();

            GameObject textObject = CreateRectObject("Label", buttonObject.transform);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text text = textObject.AddComponent<Text>();
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.95f, 0.91f, 0.84f, 1f);
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 18;
            text.resizeTextMaxSize = 42;

            return buttonObject;
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