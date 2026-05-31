using UnityEngine;

namespace MahjongGame.Board
{
    public static class BoardFrameVisualController
    {
        private static Sprite _sharedFrameSprite;

        public static bool HasRequiredFrameVisual(Transform boardRoot)
        {
            if (boardRoot == null)
            {
                return false;
            }

            Transform frameRoot = boardRoot.Find(BoardPresentationDefinition.FrameRootName);
            if (frameRoot == null)
            {
                return false;
            }

            return frameRoot.Find(BoardPresentationDefinition.FrameBackgroundName) != null
                && frameRoot.Find(BoardPresentationDefinition.FrameTrimName) != null;
        }

        public static void BuildFrameVisual(Transform boardRoot)
        {
            if (boardRoot == null)
            {
                Debug.LogWarning("[BoardFrameVisualController] BoardRoot transform is not available.");
                return;
            }

            Transform existingFrameRoot = boardRoot.Find(BoardPresentationDefinition.FrameRootName);
            if (existingFrameRoot != null)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(existingFrameRoot.gameObject);
                }
                else
                {
                    Object.DestroyImmediate(existingFrameRoot.gameObject);
                }
            }

            Vector2 framedHalfExtents = BoardPresentationLayout.GetFramedBoardHalfExtents();
            float frameWidth = framedHalfExtents.x * 2f;
            float frameHeight = framedHalfExtents.y * 2f;
            float trimInset = BoardPresentationDefinition.FrameBorderThickness;
            float trimWidth = frameWidth - trimInset * 2f;
            float trimHeight = frameHeight - trimInset * 2f;

            Sprite frameSprite = GetSharedFrameSprite();

            GameObject frameRootObject = new GameObject(BoardPresentationDefinition.FrameRootName);
            frameRootObject.transform.SetParent(boardRoot, false);
            frameRootObject.transform.localPosition = Vector3.zero;
            frameRootObject.transform.localRotation = Quaternion.identity;
            frameRootObject.transform.localScale = Vector3.one;

            CreateFrameRenderer(
                frameRootObject.transform,
                BoardPresentationDefinition.FrameBackgroundName,
                Vector3.zero,
                new Vector3(frameWidth, frameHeight, 1f),
                BoardPresentationDefinition.FrameBackgroundColor,
                frameSprite,
                BoardPresentationDefinition.FrameBackgroundSortingOrder);

            CreateFrameRenderer(
                frameRootObject.transform,
                BoardPresentationDefinition.FrameTrimName,
                Vector3.zero,
                new Vector3(trimWidth, trimHeight, 1f),
                BoardPresentationDefinition.FrameTrimColor,
                frameSprite,
                BoardPresentationDefinition.FrameTrimSortingOrder);

            BoardLayerVisualController.EnforceBoardVisualOrder(boardRoot);
        }

        private static void CreateFrameRenderer(
            Transform frameRoot,
            string childName,
            Vector3 localPosition,
            Vector3 localScale,
            Color color,
            Sprite sprite,
            int sortingOrder)
        {
            GameObject childObject = new GameObject(childName);
            childObject.transform.SetParent(frameRoot, false);
            childObject.transform.localPosition = localPosition;
            childObject.transform.localRotation = Quaternion.identity;
            childObject.transform.localScale = localScale;

            SpriteRenderer spriteRenderer = childObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = sortingOrder;
        }

        private static Sprite GetSharedFrameSprite()
        {
            if (_sharedFrameSprite != null)
            {
                return _sharedFrameSprite;
            }

            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            texture.hideFlags = HideFlags.HideAndDontSave;

            _sharedFrameSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, 1f, 1f),
                new Vector2(0.5f, 0.5f),
                1f);
            _sharedFrameSprite.hideFlags = HideFlags.HideAndDontSave;
            return _sharedFrameSprite;
        }
    }
}
