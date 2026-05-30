using System;
using MahjongGame.Core;
using MahjongGame.Core.Save;
using MahjongGame.Progression;
using UnityEngine;
using UnityEngine.UI;

namespace MahjongGame.UI
{
    [DefaultExecutionOrder(30)]
    public sealed class MainMenuNavigationController : MonoBehaviour
    {
        private static readonly Color PanelBackgroundColor = new Color(0.12f, 0.09f, 0.07f, 0.96f);
        private static readonly Color OverlayDimColor = new Color(0f, 0f, 0f, 0.45f);
        private static readonly Color ButtonColor = new Color(0.24f, 0.18f, 0.14f, 0.95f);
        private static readonly Color TextColor = new Color(0.95f, 0.91f, 0.84f, 1f);
        private static readonly Color AccentTextColor = new Color(0.75f, 0.65f, 0.45f, 1f);

        private static readonly (string ThemeId, string Label)[] LaunchThemes =
        {
            ("luxury_wood", "Luxury Wood"),
            ("bamboo_zen", "Bamboo Zen"),
            ("premium_evening", "Premium Evening"),
        };

        public static event Action LevelStartRequested;

        private enum MainMenuView
        {
            Home,
            Profile,
            Theme,
            Settings
        }

        private Transform _topBar;
        private Transform _levelButton;
        private Transform _profilePanel;
        private Transform _themePanel;
        private Transform _settingsPanel;
        private Text _profileBodyText;
        private Text _themeBodyText;
        private bool _isWired;

        private void Start()
        {
            Transform canvasTransform = MainMenuLayoutController.GetCanvasTransform();
            if (canvasTransform == null)
            {
                Debug.LogWarning("[MainMenuNavigationController] Canvas_MainMenu is not available.");
                return;
            }

            if (!HasRequiredNavigation())
            {
                BuildOverlayNavigation(canvasTransform);
            }

            CacheReferences(canvasTransform);
            WireButtons();
            ShowView(MainMenuView.Home);
        }

        public static bool HasRequiredNavigation()
        {
            Transform canvasTransform = MainMenuLayoutController.GetCanvasTransform();
            if (canvasTransform == null)
            {
                return false;
            }

            Transform overlayRoot = canvasTransform.Find("MenuOverlayRoot");
            if (overlayRoot == null)
            {
                return false;
            }

            return HasNavigationPanel(overlayRoot, "ProfilePanel")
                && HasNavigationPanel(overlayRoot, "ThemePanel")
                && HasNavigationPanel(overlayRoot, "SettingsPanel");
        }

        public static void BuildOverlayNavigation(Transform canvasTransform)
        {
            Transform existingOverlay = canvasTransform.Find("MenuOverlayRoot");
            if (existingOverlay != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(existingOverlay.gameObject);
                }
                else
                {
                    DestroyImmediate(existingOverlay.gameObject);
                }
            }

            GameObject overlayRoot = CreateRectObject("MenuOverlayRoot", canvasTransform);
            StretchFullScreen(overlayRoot.GetComponent<RectTransform>());

            CreateNavigationPanel(overlayRoot.transform, "ProfilePanel", "Profile", "Loading profile...");
            CreateNavigationPanel(overlayRoot.transform, "ThemePanel", "Theme Selection", BuildThemePlaceholderBody());
            CreateNavigationPanel(overlayRoot.transform, "SettingsPanel", "Settings", "Audio and gameplay settings will appear here.");

            overlayRoot.SetActive(false);
        }

        public static void RaiseLevelStartRequested()
        {
            LevelStartRequested?.Invoke();
        }

        private static bool HasNavigationPanel(Transform overlayRoot, string panelName)
        {
            Transform panel = overlayRoot.Find(panelName);
            return panel != null && panel.Find("Content/BackButton") != null && panel.Find("Content/BodyText") != null;
        }

        private static string BuildThemePlaceholderBody()
        {
            string selectedThemeId = ResolveSelectedThemeId();
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendLine("Launch Themes");
            builder.AppendLine();

            foreach ((string themeId, string label) in LaunchThemes)
            {
                string marker = string.Equals(themeId, selectedThemeId, StringComparison.Ordinal)
                    ? " (Selected)"
                    : string.Empty;
                builder.AppendLine("- " + label + marker);
            }

            builder.AppendLine();
            builder.AppendLine("Theme switching will be enabled in a later phase.");
            return builder.ToString();
        }

        private static string ResolveSelectedThemeId()
        {
            if (!SaveSystem.HasInstance || SaveSystem.Instance.Data?.settings == null)
            {
                return "luxury_wood";
            }

            string themeId = SaveSystem.Instance.Data.settings.selectedThemeId;
            return string.IsNullOrWhiteSpace(themeId) ? "luxury_wood" : themeId;
        }

        private void CacheReferences(Transform canvasTransform)
        {
            _topBar = canvasTransform.Find("TopBar");
            _levelButton = canvasTransform.Find("LevelButton");

            Transform overlayRoot = canvasTransform.Find("MenuOverlayRoot");
            if (overlayRoot == null)
            {
                return;
            }

            _profilePanel = overlayRoot.Find("ProfilePanel");
            _themePanel = overlayRoot.Find("ThemePanel");
            _settingsPanel = overlayRoot.Find("SettingsPanel");
            _profileBodyText = _profilePanel != null ? _profilePanel.Find("Content/BodyText")?.GetComponent<Text>() : null;
            _themeBodyText = _themePanel != null ? _themePanel.Find("Content/BodyText")?.GetComponent<Text>() : null;
        }

        private void WireButtons()
        {
            if (_isWired)
            {
                return;
            }

            WireMenuButton("TopBar/ProfileButton", () => ShowView(MainMenuView.Profile));
            WireMenuButton("TopBar/ThemeButton", () => ShowView(MainMenuView.Theme));
            WireMenuButton("TopBar/SettingsButton", () => ShowView(MainMenuView.Settings));
            WireMenuButton("LevelButton", RequestLevelStart);

            WireBackButton(_profilePanel, () => ShowView(MainMenuView.Home));
            WireBackButton(_themePanel, () => ShowView(MainMenuView.Home));
            WireBackButton(_settingsPanel, () => ShowView(MainMenuView.Home));

            _isWired = true;
        }

        private void WireMenuButton(string path, UnityEngine.Events.UnityAction action)
        {
            Transform canvasTransform = MainMenuLayoutController.GetCanvasTransform();
            if (canvasTransform == null)
            {
                return;
            }

            Transform buttonTransform = canvasTransform.Find(path);
            Button button = buttonTransform != null ? buttonTransform.GetComponent<Button>() : null;
            if (button == null)
            {
                Debug.LogWarning("[MainMenuNavigationController] Button '" + path + "' was not found.");
                return;
            }

            button.onClick.AddListener(action);
        }

        private static void WireBackButton(Transform panel, UnityEngine.Events.UnityAction action)
        {
            if (panel == null)
            {
                return;
            }

            Transform backButtonTransform = panel.Find("Content/BackButton");
            Button backButton = backButtonTransform != null ? backButtonTransform.GetComponent<Button>() : null;
            if (backButton == null)
            {
                return;
            }

            backButton.onClick.AddListener(action);
        }

        private void RequestLevelStart()
        {
            RaiseLevelStartRequested();
            Debug.Log("[MainMenuNavigationController] Level start requested.");
        }

        private void ShowView(MainMenuView view)
        {
            Transform canvasTransform = MainMenuLayoutController.GetCanvasTransform();
            Transform overlayRoot = canvasTransform != null ? canvasTransform.Find("MenuOverlayRoot") : null;
            bool isHome = view == MainMenuView.Home;

            if (_topBar != null)
            {
                _topBar.gameObject.SetActive(isHome);
            }

            if (_levelButton != null)
            {
                _levelButton.gameObject.SetActive(isHome);
            }

            if (overlayRoot != null)
            {
                overlayRoot.gameObject.SetActive(!isHome);
            }

            SetPanelActive(_profilePanel, view == MainMenuView.Profile);
            SetPanelActive(_themePanel, view == MainMenuView.Theme);
            SetPanelActive(_settingsPanel, view == MainMenuView.Settings);

            if (view == MainMenuView.Profile)
            {
                RefreshProfileBody();
            }

            if (view == MainMenuView.Theme && _themeBodyText != null)
            {
                _themeBodyText.text = BuildThemePlaceholderBody();
            }
        }

        private static void SetPanelActive(Transform panel, bool isActive)
        {
            if (panel != null)
            {
                panel.gameObject.SetActive(isActive);
            }
        }

        private void RefreshProfileBody()
        {
            if (_profileBodyText == null)
            {
                return;
            }

            int currentLevel = PlayerProgressionDirector.HasInstance
                ? PlayerProgressionDirector.Instance.CurrentLevel
                : LevelProgressData.MinLevel;
            int highestLevel = PlayerProgressionDirector.HasInstance
                ? PlayerProgressionDirector.Instance.HighestLevel
                : LevelProgressData.MinLevel;
            long globalScore = PlayerProgressionDirector.HasInstance
                ? PlayerProgressionDirector.Instance.GlobalPerformanceScore
                : 0;

            StatisticsSaveData statistics = SaveSystem.HasInstance && SaveSystem.Instance.Data != null
                ? SaveSystem.Instance.Data.statistics
                : null;

            int globalRank = statistics != null ? statistics.currentGlobalRank : 0;
            int highestCombo = statistics != null ? statistics.highestCombo : 0;
            int perfectClears = statistics != null ? statistics.perfectClears : 0;
            long totalPlayTimeSeconds = statistics != null ? statistics.totalPlayTimeSeconds : 0;

            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendLine("Current Level: " + currentLevel);
            builder.AppendLine("Highest Level: " + highestLevel);
            builder.AppendLine("Global Rank: " + globalRank);
            builder.AppendLine("Global Performance Score: " + globalScore);
            builder.AppendLine("Highest Combo: " + highestCombo);
            builder.AppendLine("Perfect Clears: " + perfectClears);
            builder.AppendLine("Total Play Time: " + FormatPlayTime(totalPlayTimeSeconds));

            _profileBodyText.text = builder.ToString();
        }

        private static string FormatPlayTime(long totalPlayTimeSeconds)
        {
            if (totalPlayTimeSeconds < 0)
            {
                totalPlayTimeSeconds = 0;
            }

            long hours = totalPlayTimeSeconds / 3600;
            long minutes = (totalPlayTimeSeconds % 3600) / 60;
            long seconds = totalPlayTimeSeconds % 60;
            return hours + "h " + minutes + "m " + seconds + "s";
        }

        private static void CreateNavigationPanel(Transform overlayRoot, string panelName, string title, string body)
        {
            GameObject panelObject = CreateRectObject(panelName, overlayRoot);
            StretchFullScreen(panelObject.GetComponent<RectTransform>());
            panelObject.SetActive(false);

            GameObject dimObject = CreateRectObject("DimBackground", panelObject.transform);
            StretchFullScreen(dimObject.GetComponent<RectTransform>());
            Image dimImage = dimObject.AddComponent<Image>();
            dimImage.color = OverlayDimColor;
            dimImage.raycastTarget = true;

            GameObject contentObject = CreateRectObject("Content", panelObject.transform);
            RectTransform contentRect = contentObject.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.1f, 0.18f);
            contentRect.anchorMax = new Vector2(0.9f, 0.82f);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;
            Image contentImage = contentObject.AddComponent<Image>();
            contentImage.color = PanelBackgroundColor;

            CreatePanelText("TitleText", contentObject.transform, title, 36, AccentTextColor, new Vector2(0f, -24f), new Vector2(0f, -90f));
            CreatePanelText("BodyText", contentObject.transform, body, 24, TextColor, new Vector2(24f, -110f), new Vector2(-24f, -120f));
            CreateBackButton(contentObject.transform);
        }

        private static void CreatePanelText(string name, Transform parent, string textValue, int fontSize, Color color, Vector2 offsetMin, Vector2 offsetMax)
        {
            GameObject textObject = CreateRectObject(name, parent);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = offsetMin;
            textRect.offsetMax = offsetMax;

            Text text = textObject.AddComponent<Text>();
            text.text = textValue;
            text.alignment = TextAnchor.UpperLeft;
            text.color = color;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
        }

        private static void CreateBackButton(Transform contentParent)
        {
            GameObject buttonObject = CreateRectObject("BackButton", contentParent);
            RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0f);
            buttonRect.anchorMax = new Vector2(0.5f, 0f);
            buttonRect.pivot = new Vector2(0.5f, 0f);
            buttonRect.anchoredPosition = new Vector2(0f, 24f);
            buttonRect.sizeDelta = new Vector2(260f, 72f);

            Image image = buttonObject.AddComponent<Image>();
            image.color = ButtonColor;
            buttonObject.AddComponent<Button>();

            GameObject labelObject = CreateRectObject("Label", buttonObject.transform);
            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            Text label = labelObject.AddComponent<Text>();
            label.text = "Back to Menu";
            label.alignment = TextAnchor.MiddleCenter;
            label.color = TextColor;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 16;
            label.resizeTextMaxSize = 28;
        }

        private static GameObject CreateRectObject(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void StretchFullScreen(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }
    }
}
