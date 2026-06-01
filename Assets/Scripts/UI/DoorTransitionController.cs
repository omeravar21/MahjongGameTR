using MahjongGame.Core;
using UnityEngine;
using UnityEngine.UI;

namespace MahjongGame.UI
{
    [DefaultExecutionOrder(40)]
    public sealed class DoorTransitionController : MonoBehaviour
    {
        private const float DoorOpenDurationSeconds = 1.0f;

        [SerializeField] private DoorPresentationController doorPresentationController;

        private bool _isTransitioning;
        private GraphicRaycaster _canvasRaycaster;

        private void Awake()
        {
            if (doorPresentationController == null)
            {
                doorPresentationController = GetComponent<DoorPresentationController>();
            }
        }

        private void OnEnable()
        {
            MainMenuNavigationController.LevelStartRequested += HandleLevelStartRequested;
            MainMenuNavigationController.DailyBoardStartRequested += HandleDailyBoardStartRequested;
        }

        private void OnDisable()
        {
            MainMenuNavigationController.LevelStartRequested -= HandleLevelStartRequested;
            MainMenuNavigationController.DailyBoardStartRequested -= HandleDailyBoardStartRequested;
        }

        private void HandleLevelStartRequested()
        {
            BeginTransition();
        }

        private void HandleDailyBoardStartRequested()
        {
            BeginTransition();
        }

        private void BeginTransition()
        {
            if (_isTransitioning)
            {
                return;
            }

            if (doorPresentationController == null)
            {
                doorPresentationController = GetComponent<DoorPresentationController>();
            }

            if (doorPresentationController == null)
            {
                Debug.LogError("[DoorTransitionController] DoorPresentationController is not available.");
                return;
            }

            _isTransitioning = true;
            SetMenuInteractionEnabled(false);
            doorPresentationController.PlayOpenTransition(DoorOpenDurationSeconds, HandleDoorOpenComplete);
        }

        private void HandleDoorOpenComplete()
        {
            if (!SceneLoadController.HasInstance)
            {
                Debug.LogError("[DoorTransitionController] SceneLoadController is not available.");
                _isTransitioning = false;
                SetMenuInteractionEnabled(true);
                return;
            }

            SceneLoadController.Instance.LoadGame();
        }

        private void SetMenuInteractionEnabled(bool isEnabled)
        {
            Transform canvasTransform = MainMenuLayoutController.GetCanvasTransform();
            if (canvasTransform == null)
            {
                return;
            }

            if (_canvasRaycaster == null)
            {
                _canvasRaycaster = canvasTransform.GetComponent<GraphicRaycaster>();
            }

            if (_canvasRaycaster != null)
            {
                _canvasRaycaster.enabled = isEnabled;
            }
        }
    }
}
