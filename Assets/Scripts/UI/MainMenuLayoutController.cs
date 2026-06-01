using MahjongGame.Progression;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace MahjongGame.UI
{
    [DefaultExecutionOrder(0)]
    public sealed class MainMenuLayoutController : MonoBehaviour
    {
        private static readonly Color ButtonColor = new Color(0.24f, 0.18f, 0.14f, 0.95f);
        private static readonly Color TextColor = new Color(0.95f, 0.91f, 0.84f, 1f);

        private void Awake()
        {
            if (HasRequiredLayout())
            {
                return;
            }

            BuildLayout();
        }

        public static Transform GetCanvasTransform()
        {
            GameObject canvas = GameObject.Find("Canvas_MainMenu");
            return canvas != null ? canvas.transform : null;
        }

        public static bool HasRequiredLayout()
        {
            Transform canvasTransform = GetCanvasTransform();
            if (canvasTransform == null || canvasTransform.GetComponent<Canvas>() == null)
            {
                return false;
            }

            return FindChild(canvasTransform, "TopBar/ProfileButton") != null
                && FindChild(canvasTransform, "TopBar/RankingButton") != null
                && FindChild(canvasTransform, "TopBar/ThemeButton") != null
                && FindChild(canvasTransform, "TopBar/SettingsButton") != null
                && FindChild(canvasTransform, "LevelButton") != null;
        }

        public static void BuildLayout()
        {
            EnsureEventSystem();
            GameObject canvasRoot = EnsureCanvas();
            Transform canvasTransform = canvasRoot.transform;

            ClearMenuChildren(canvasTransform);

            GameObject topBar = CreateRectObject("TopBar", canvasTransform);
            SetStretchTopBar(topBar.GetComponent<RectTransform>());

            CreateMenuButton("ProfileButton", topBar.transform, new Vector2(-247f, -60f), new Vector2(150f, 80f), "Profile");
            CreateMenuButton("RankingButton", topBar.transform, new Vector2(-82f, -60f), new Vector2(150f, 80f), "Ranking");
            CreateMenuButton("ThemeButton", topBar.transform, new Vector2(82f, -60f), new Vector2(150f, 80f), "Theme");
            CreateMenuButton("SettingsButton", topBar.transform, new Vector2(247f, -60f), new Vector2(150f, 80f), "Settings");
            CreateMenuButton("LevelButton", canvasTransform, Vector2.zero, new Vector2(420f, 180f), CurrentLevelButtonController.FormatLevelLabel(LevelProgressData.MinLevel));
        }

        private static Transform FindChild(Transform parent, string path)
        {
            return parent != null ? parent.Find(path) : null;
        }

        private static void EnsureEventSystem()
        {
            EventSystem existing = Object.FindAnyObjectByType<EventSystem>();
            if (existing != null)
            {
                if (existing.GetComponent<InputSystemUIInputModule>() == null)
                {
                    Object.Destroy(existing.GetComponent<StandaloneInputModule>());
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
                if (Application.isPlaying)
                {
                    Object.Destroy(canvasObject.GetComponent<Transform>());
                }
                else
                {
                    Object.DestroyImmediate(canvasObject.GetComponent<Transform>());
                }

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
                Transform child = canvasTransform.GetChild(i);
                if (child.name == "DoorPanel" || child.name == "MenuOverlayRoot")
                {
                    continue;
                }

                if (Application.isPlaying)
                {
                    Object.Destroy(child.gameObject);
                }
                else
                {
                    Object.DestroyImmediate(child.gameObject);
                }
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
            image.color = ButtonColor;
            buttonObject.AddComponent<Button>();

            GameObject textObject = CreateRectObject("Label", buttonObject.transform);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text text = textObject.AddComponent<Text>();
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = TextColor;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 18;
            text.resizeTextMaxSize = 42;

            return buttonObject;
        }
    }
}
