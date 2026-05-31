using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MahjongGame.UI
{
    [DefaultExecutionOrder(10)]
    public sealed class DoorPresentationController : MonoBehaviour
    {
        private static readonly Color BackgroundColor = new Color(0.15f, 0.11f, 0.08f, 1f);
        private static readonly Color DoorLeafColor = new Color(0.28f, 0.20f, 0.15f, 1f);
        private static readonly Color TrimColor = new Color(0.75f, 0.65f, 0.45f, 0.9f);

        [SerializeField] private bool animationPrepared;
        [SerializeField] private CanvasGroup doorCanvasGroup;
        [SerializeField] private RectTransform doorLeftRect;
        [SerializeField] private RectTransform doorRightRect;

        private Coroutine _transitionCoroutine;
        private bool _isTransitionPlaying;

        public bool IsAnimationPrepared => animationPrepared;

        public bool IsTransitionPlaying => _isTransitionPlaying;

        public CanvasGroup GetDoorCanvasGroup() => doorCanvasGroup;

        public RectTransform GetDoorLeftRect() => doorLeftRect;

        public RectTransform GetDoorRightRect() => doorRightRect;

        private void Awake()
        {
            if (HasRequiredDoorPresentation())
            {
                CacheReferences();
                return;
            }

            Transform canvasTransform = MainMenuLayoutController.GetCanvasTransform();
            if (canvasTransform == null)
            {
                Debug.LogWarning("[DoorPresentationController] Canvas_MainMenu is not available.");
                return;
            }

            BuildDoorPresentation(canvasTransform);
            CacheReferences();
        }

        public static bool HasRequiredDoorPresentation()
        {
            Transform canvasTransform = MainMenuLayoutController.GetCanvasTransform();
            if (canvasTransform == null)
            {
                return false;
            }

            Transform doorPanel = canvasTransform.Find("DoorPanel");
            if (doorPanel == null)
            {
                return false;
            }

            Transform background = doorPanel.Find("DoorBackground");
            Transform visualLayer = doorPanel.Find("DoorVisualLayer");
            if (background == null || visualLayer == null)
            {
                return false;
            }

            return background.GetComponent<Image>() != null
                && visualLayer.Find("DoorLeftPanel") != null
                && visualLayer.Find("DoorRightPanel") != null;
        }

        public static void BuildDoorPresentation(Transform canvasTransform)
        {
            Transform existingDoor = canvasTransform.Find("DoorPanel");
            if (existingDoor != null)
            {
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(existingDoor.gameObject);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(existingDoor.gameObject);
                }
            }

            GameObject doorPanel = CreateRectObject("DoorPanel", canvasTransform);
            RectTransform doorPanelRect = doorPanel.GetComponent<RectTransform>();
            StretchFullScreen(doorPanelRect);

            CanvasGroup canvasGroup = doorPanel.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            GameObject doorBackground = CreateRectObject("DoorBackground", doorPanel.transform);
            StretchFullScreen(doorBackground.GetComponent<RectTransform>());
            Image backgroundImage = doorBackground.AddComponent<Image>();
            backgroundImage.color = BackgroundColor;
            backgroundImage.raycastTarget = false;

            GameObject doorVisualLayer = CreateRectObject("DoorVisualLayer", doorPanel.transform);
            StretchFullScreen(doorVisualLayer.GetComponent<RectTransform>());

            GameObject doorLeftPanel = CreateRectObject("DoorLeftPanel", doorVisualLayer.transform);
            RectTransform leftRect = doorLeftPanel.GetComponent<RectTransform>();
            SetHorizontalHalf(leftRect, true);
            Image leftImage = doorLeftPanel.AddComponent<Image>();
            leftImage.color = DoorLeafColor;
            leftImage.raycastTarget = false;

            GameObject doorRightPanel = CreateRectObject("DoorRightPanel", doorVisualLayer.transform);
            RectTransform rightRect = doorRightPanel.GetComponent<RectTransform>();
            SetHorizontalHalf(rightRect, false);
            Image rightImage = doorRightPanel.AddComponent<Image>();
            rightImage.color = DoorLeafColor;
            rightImage.raycastTarget = false;

            GameObject doorCenterTrim = CreateRectObject("DoorCenterTrim", doorVisualLayer.transform);
            RectTransform trimRect = doorCenterTrim.GetComponent<RectTransform>();
            trimRect.anchorMin = new Vector2(0.5f, 0f);
            trimRect.anchorMax = new Vector2(0.5f, 1f);
            trimRect.pivot = new Vector2(0.5f, 0.5f);
            trimRect.anchoredPosition = Vector2.zero;
            trimRect.sizeDelta = new Vector2(8f, 0f);
            Image trimImage = doorCenterTrim.AddComponent<Image>();
            trimImage.color = TrimColor;
            trimImage.raycastTarget = false;

            doorPanel.transform.SetSiblingIndex(0);
        }

        private void CacheReferences()
        {
            Transform canvasTransform = MainMenuLayoutController.GetCanvasTransform();
            if (canvasTransform == null)
            {
                return;
            }

            Transform doorPanel = canvasTransform.Find("DoorPanel");
            if (doorPanel == null)
            {
                return;
            }

            doorCanvasGroup = doorPanel.GetComponent<CanvasGroup>();
            Transform visualLayer = doorPanel.Find("DoorVisualLayer");
            if (visualLayer == null)
            {
                return;
            }

            doorLeftRect = visualLayer.Find("DoorLeftPanel") as RectTransform;
            doorRightRect = visualLayer.Find("DoorRightPanel") as RectTransform;
            animationPrepared = doorCanvasGroup != null && doorLeftRect != null && doorRightRect != null;
        }

        public Coroutine PlayOpenTransition(float duration, Action onComplete)
        {
            if (!animationPrepared)
            {
                CacheReferences();
            }

            if (!animationPrepared)
            {
                Debug.LogWarning("[DoorPresentationController] Door animation is not prepared.");
                onComplete?.Invoke();
                return null;
            }

            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
            }

            _transitionCoroutine = StartCoroutine(OpenTransitionRoutine(duration, onComplete));
            return _transitionCoroutine;
        }

        private IEnumerator OpenTransitionRoutine(float duration, Action onComplete)
        {
            _isTransitionPlaying = true;

            Vector2 leftClosedPosition = doorLeftRect.anchoredPosition;
            Vector2 rightClosedPosition = doorRightRect.anchoredPosition;
            float slideDistance = ResolveSlideDistance();
            Vector2 leftOpenPosition = leftClosedPosition + new Vector2(-slideDistance, 0f);
            Vector2 rightOpenPosition = rightClosedPosition + new Vector2(slideDistance, 0f);
            float startAlpha = doorCanvasGroup != null ? doorCanvasGroup.alpha : 1f;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float normalizedTime = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
                float easedTime = Mathf.SmoothStep(0f, 1f, normalizedTime);

                doorLeftRect.anchoredPosition = Vector2.Lerp(leftClosedPosition, leftOpenPosition, easedTime);
                doorRightRect.anchoredPosition = Vector2.Lerp(rightClosedPosition, rightOpenPosition, easedTime);

                if (doorCanvasGroup != null && normalizedTime >= 0.8f)
                {
                    float fadeProgress = Mathf.InverseLerp(0.8f, 1f, normalizedTime);
                    doorCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, fadeProgress);
                }

                yield return null;
            }

            doorLeftRect.anchoredPosition = leftOpenPosition;
            doorRightRect.anchoredPosition = rightOpenPosition;
            if (doorCanvasGroup != null)
            {
                doorCanvasGroup.alpha = 0f;
            }

            _isTransitionPlaying = false;
            _transitionCoroutine = null;
            onComplete?.Invoke();
        }

        private float ResolveSlideDistance()
        {
            Transform canvasTransform = MainMenuLayoutController.GetCanvasTransform();
            RectTransform canvasRect = canvasTransform != null ? canvasTransform as RectTransform : null;
            if (canvasRect != null && canvasRect.rect.width > 0f)
            {
                return canvasRect.rect.width * 0.55f;
            }

            return 540f;
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

        private static void SetHorizontalHalf(RectTransform rectTransform, bool isLeftHalf)
        {
            if (isLeftHalf)
            {
                rectTransform.anchorMin = new Vector2(0f, 0f);
                rectTransform.anchorMax = new Vector2(0.5f, 1f);
            }
            else
            {
                rectTransform.anchorMin = new Vector2(0.5f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
            }

            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }
    }
}
