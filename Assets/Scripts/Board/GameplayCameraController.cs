using UnityEngine;

namespace MahjongGame.Board
{
    [DefaultExecutionOrder(0)]
    [RequireComponent(typeof(Camera))]
    public sealed class GameplayCameraController : MonoBehaviour
    {
        [SerializeField] private BoardPresentationController boardPresentationController;

        private Camera _camera;
        private float _lastAspect;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            ResolveBoardPresentationController();
            ApplyCameraPresentation(force: true);
        }

        private void Update()
        {
            float aspect = Screen.width / (float)Screen.height;
            if (!Mathf.Approximately(aspect, _lastAspect))
            {
                ApplyCameraPresentation(force: true);
            }
        }

        public void ApplyCameraPresentation(bool force = false)
        {
            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
            }

            if (_camera == null)
            {
                return;
            }

            ResolveBoardPresentationController();

            _camera.backgroundColor = BoardPresentationDefinition.GameplayBackgroundColor;

            Vector3 boardRootLocalPosition = boardPresentationController != null
                ? boardPresentationController.BoardRootTransform.localPosition
                : BoardPresentationLayout.GetBoardRootLocalPosition();

            transform.position = BoardPresentationLayout.GetCameraTargetPosition(boardRootLocalPosition);

            float aspect = Screen.width / (float)Screen.height;
            if (aspect <= 0f)
            {
                aspect = BoardPresentationDefinition.ReferenceAspect;
            }

            if (force || !Mathf.Approximately(aspect, _lastAspect))
            {
                _camera.orthographicSize = BoardPresentationLayout.CalculateOrthographicSize(aspect);
                _lastAspect = aspect;
            }
        }

        private void ResolveBoardPresentationController()
        {
            if (boardPresentationController != null)
            {
                return;
            }

            GameObject gameplayRootObject = GameObject.Find("GameplayRoot");
            if (gameplayRootObject == null)
            {
                return;
            }

            Transform boardRoot = gameplayRootObject.transform.Find("BoardRoot");
            if (boardRoot == null)
            {
                return;
            }

            boardPresentationController = boardRoot.GetComponent<BoardPresentationController>();
        }
    }
}
