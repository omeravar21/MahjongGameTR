using UnityEngine;

namespace MahjongGame.Board
{
    [DefaultExecutionOrder(4)]
    public sealed class BoardPresentationController : MonoBehaviour
    {
        [SerializeField] private Transform boardRootTransform;

        public Transform BoardRootTransform => boardRootTransform != null ? boardRootTransform : transform;

        private void Awake()
        {
            if (boardRootTransform == null)
            {
                boardRootTransform = transform;
            }

            ApplyPresentation();
        }

        public void ApplyPresentation()
        {
            boardRootTransform.localPosition = BoardPresentationLayout.GetBoardRootLocalPosition();

            if (!BoardFrameVisualController.HasRequiredFrameVisual(boardRootTransform))
            {
                BoardFrameVisualController.BuildFrameVisual(boardRootTransform);
            }
            else
            {
                BoardLayerVisualController.EnforceBoardVisualOrder(boardRootTransform);
            }
        }

        public Bounds GetFramedWorldBounds()
        {
            Vector2 framedHalfExtents = BoardPresentationLayout.GetFramedBoardHalfExtents();
            Vector3 center = boardRootTransform.position;
            return new Bounds(center, new Vector3(framedHalfExtents.x * 2f, framedHalfExtents.y * 2f, 0.1f));
        }
    }
}
