using MahjongGame.Progression;
using MahjongGame.Session;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace MahjongGame.UI
{
    public sealed class PerformanceScreenController : MonoBehaviour
    {
        public const string CanvasName = "Canvas_Game";
        public const string PanelName = "PerformanceScreenPanel";
        public const string NextLevelButtonName = "NextLevelButton";
        private const string NextLevelButtonLabel = "Next Level";
        private const string MaxLevelButtonLabel = "Max Level";

        private static readonly Color PanelColor = new Color(0.12f, 0.1f, 0.08f, 0.92f);
        private static readonly Color ButtonColor = new Color(0.24f, 0.18f, 0.14f, 0.95f);
        private static readonly Color TextColor = new Color(0.95f, 0.91f, 0.84f, 1f);

        private GameObject _panelRoot;
        private Text _scoreText;
        private Text _completionTimeText;
        private Text _timeBonusText;
        private Text _globalPerformanceScoreText;
        private Text _comboCountText;
        private Text _highestComboText;
        private Text _jokerBonusText;
        private Button _nextLevelButton;
        private LevelResultSummary _activeSummary;

        public bool IsVisible => _panelRoot != null && _panelRoot.activeSelf;

        private void Awake()
        {
            if (!HasRequiredLayout())
            {
                BuildLayout();
            }

            BindUiReferences();
            HidePanel();
        }

        private void OnEnable()
        {
            LevelResultEvents.LevelResultReady += HandleLevelResultReady;
            SessionEvents.SessionStarted += HandleSessionStarted;

            if (_nextLevelButton != null)
            {
                _nextLevelButton.onClick.AddListener(HandleNextLevelClicked);
            }
        }

        private void OnDisable()
        {
            LevelResultEvents.LevelResultReady -= HandleLevelResultReady;
            SessionEvents.SessionStarted -= HandleSessionStarted;

            if (_nextLevelButton != null)
            {
                _nextLevelButton.onClick.RemoveListener(HandleNextLevelClicked);
            }
        }

        public static bool HasRequiredLayout()
        {
            Transform canvasTransform = GetCanvasTransform();
            if (canvasTransform == null)
            {
                return false;
            }

            Transform panelTransform = canvasTransform.Find(PanelName);
            if (panelTransform == null)
            {
                return false;
            }

            return panelTransform.Find("ScoreText") != null
                && panelTransform.Find("CompletionTimeText") != null
                && panelTransform.Find("TimeBonusText") != null
                && panelTransform.Find("GlobalPerformanceScoreText") != null
                && panelTransform.Find("ComboCountText") != null
                && panelTransform.Find("HighestComboText") != null
                && panelTransform.Find("JokerBonusText") != null
                && panelTransform.Find(NextLevelButtonName) != null;
        }

        public static Transform GetCanvasTransform()
        {
            Transform uiRoot = FindUiRootTransform();
            if (uiRoot == null)
            {
                return null;
            }

            Transform canvasTransform = uiRoot.Find(CanvasName);
            return canvasTransform;
        }

        public static void BuildLayout()
        {
            Transform uiRoot = FindUiRootTransform();
            if (uiRoot == null)
            {
                Debug.LogError("[PerformanceScreenController] UIRoot was not found.");
                return;
            }

            EnsureEventSystem();
            GameObject canvasObject = EnsureCanvas(uiRoot);
            Transform canvasTransform = canvasObject.transform;

            Transform existingPanel = canvasTransform.Find(PanelName);
            if (existingPanel != null)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(existingPanel.gameObject);
                }
                else
                {
                    Object.DestroyImmediate(existingPanel.gameObject);
                }
            }

            GameObject panelObject = CreateRectObject(PanelName, canvasTransform);
            RectTransform panelRect = panelObject.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(860f, 1180f);

            Image panelImage = panelObject.AddComponent<Image>();
            panelImage.color = PanelColor;

            CreateLabelText("TitleText", panelObject.transform, new Vector2(0f, 500f), new Vector2(760f, 100f), "Level Complete", 48);
            _ = CreateMetricText("ScoreText", panelObject.transform, new Vector2(0f, 390f), "Score: 0");
            _ = CreateMetricText("TimeBonusText", panelObject.transform, new Vector2(0f, 280f), "Time Bonus: +0");
            _ = CreateMetricText("CompletionTimeText", panelObject.transform, new Vector2(0f, 170f), "Time: 0.0s");
            _ = CreateMetricText("GlobalPerformanceScoreText", panelObject.transform, new Vector2(0f, 60f), "Global Performance Score: +0");
            _ = CreateMetricText("ComboCountText", panelObject.transform, new Vector2(0f, -50f), "Combos: 0");
            _ = CreateMetricText("HighestComboText", panelObject.transform, new Vector2(0f, -160f), "Highest Combo: 0");
            _ = CreateMetricText("JokerBonusText", panelObject.transform, new Vector2(0f, -270f), "Joker Bonus: 0");
            CreateActionButton(NextLevelButtonName, panelObject.transform, new Vector2(0f, -420f), new Vector2(420f, 120f), "Next Level");
        }

        internal void ShowSummaryForValidation(LevelResultSummary summary)
        {
            ShowSummary(summary);
        }

        internal void HidePanelForValidation()
        {
            HidePanel();
        }

        internal void InvokeNextLevelForValidation()
        {
            HandleNextLevelClicked();
        }

        private void BindUiReferences()
        {
            Transform canvasTransform = GetCanvasTransform();
            if (canvasTransform == null)
            {
                return;
            }

            Transform panelTransform = canvasTransform.Find(PanelName);
            if (panelTransform == null)
            {
                return;
            }

            _panelRoot = panelTransform.gameObject;
            _scoreText = panelTransform.Find("ScoreText")?.GetComponent<Text>();
            _completionTimeText = panelTransform.Find("CompletionTimeText")?.GetComponent<Text>();
            _timeBonusText = panelTransform.Find("TimeBonusText")?.GetComponent<Text>();
            _globalPerformanceScoreText = panelTransform.Find("GlobalPerformanceScoreText")?.GetComponent<Text>();
            _comboCountText = panelTransform.Find("ComboCountText")?.GetComponent<Text>();
            _highestComboText = panelTransform.Find("HighestComboText")?.GetComponent<Text>();
            _jokerBonusText = panelTransform.Find("JokerBonusText")?.GetComponent<Text>();
            _nextLevelButton = panelTransform.Find(NextLevelButtonName)?.GetComponent<Button>();
        }

        private void HandleLevelResultReady(LevelResultSummary summary)
        {
            ShowSummary(summary);
        }

        private void HandleSessionStarted(SessionStartedContext context)
        {
            HidePanel();
        }

        private void ShowSummary(LevelResultSummary summary)
        {
            if (summary == null)
            {
                return;
            }

            _activeSummary = summary;

            if (_scoreText != null)
            {
                _scoreText.text = "Score: " + summary.Score;
            }

            if (_completionTimeText != null)
            {
                _completionTimeText.text = "Time: " + summary.CompletionTimeSeconds.ToString("0.0") + "s";
            }

            if (_timeBonusText != null)
            {
                _timeBonusText.text = "Time Bonus: +" + summary.TimePerformanceBonus;
            }

            if (_globalPerformanceScoreText != null)
            {
                _globalPerformanceScoreText.text = "Global Performance Score: +"
                    + summary.GlobalPerformanceScoreEarned;
            }

            if (_comboCountText != null)
            {
                _comboCountText.text = "Combos: " + summary.TotalComboCount;
            }

            if (_highestComboText != null)
            {
                _highestComboText.text = "Highest Combo: " + summary.HighestCombo;
            }

            if (_jokerBonusText != null)
            {
                if (summary.JokerBonusTotal > 0)
                {
                    _jokerBonusText.gameObject.SetActive(true);
                    _jokerBonusText.text = "Joker Bonus: +"
                        + summary.JokerBonusTotal
                        + " ("
                        + summary.EarlyJokerMatchCount
                        + ")";
                }
                else
                {
                    _jokerBonusText.gameObject.SetActive(false);
                }
            }

            if (_panelRoot != null)
            {
                _panelRoot.SetActive(true);
            }

            RefreshNextLevelButtonState(summary.LevelNumber);
        }

        private void RefreshNextLevelButtonState(int completedLevelNumber)
        {
            if (_nextLevelButton == null)
            {
                return;
            }

            bool canAdvance = LevelProgressionDefinition.CanAdvanceFrom(completedLevelNumber);
            _nextLevelButton.interactable = canAdvance;

            Transform labelTransform = _nextLevelButton.transform.Find("Label");
            Text labelText = labelTransform != null ? labelTransform.GetComponent<Text>() : null;
            if (labelText != null)
            {
                labelText.text = canAdvance ? NextLevelButtonLabel : MaxLevelButtonLabel;
            }
        }

        private void HidePanel()
        {
            _activeSummary = null;

            if (_panelRoot != null)
            {
                _panelRoot.SetActive(false);
            }
        }

        private void HandleNextLevelClicked()
        {
            HidePanel();

            Transform gameplayRoot = ResolveGameplayRootTransform();
            if (gameplayRoot == null)
            {
                Debug.LogWarning("[PerformanceScreenController] GameplayRoot was not found for next level flow.");
                return;
            }

            if (!PlayerProgressionDirector.HasInstance)
            {
                Debug.LogWarning("[PerformanceScreenController] PlayerProgressionDirector is not available.");
                return;
            }

            if (!PlayerProgressionDirector.Instance.TryAdvanceToNextLevel(out LevelProgressionResult result))
            {
                Debug.LogWarning(
                    "[PerformanceScreenController] Could not advance from level "
                    + result.PreviousLevel
                    + ".");
                return;
            }

            LevelRuntimeResetter.TryResetLevel(gameplayRoot);

            if (SessionDirector.HasInstance)
            {
                SessionDirector.Instance.TryStartSession(out _);
            }
        }

        private static Transform ResolveGameplayRootTransform()
        {
            Transform uiRoot = FindUiRootTransform();
            return uiRoot != null ? uiRoot.parent : null;
        }

        private static Transform FindUiRootTransform()
        {
            GameObject uiRootObject = GameObject.Find("UIRoot");
            return uiRootObject != null ? uiRootObject.transform : null;
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

        private static GameObject EnsureCanvas(Transform uiRoot)
        {
            Transform existingCanvas = uiRoot.Find(CanvasName);
            GameObject canvasObject = existingCanvas != null
                ? existingCanvas.gameObject
                : new GameObject(CanvasName);

            if (existingCanvas == null)
            {
                canvasObject.transform.SetParent(uiRoot, false);
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

        private static GameObject CreateRectObject(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static Text CreateLabelText(
            string name,
            Transform parent,
            Vector2 anchoredPosition,
            Vector2 size,
            string label,
            int fontSize)
        {
            GameObject textObject = CreateRectObject(name, parent);
            RectTransform rectTransform = textObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;

            Text text = textObject.AddComponent<Text>();
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = TextColor;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private static Text CreateMetricText(string name, Transform parent, Vector2 anchoredPosition, string label)
        {
            return CreateLabelText(name, parent, anchoredPosition, new Vector2(760f, 90f), label, 40);
        }

        private static GameObject CreateActionButton(
            string name,
            Transform parent,
            Vector2 anchoredPosition,
            Vector2 size,
            string label)
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
