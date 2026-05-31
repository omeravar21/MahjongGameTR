using UnityEngine;

namespace MahjongGame.Tray
{
    public static class TrayFrameVisualController
    {
        private static Sprite _sharedFrameSprite;

        public static bool HasRequiredFrameVisual(Transform trayRoot)
        {
            if (trayRoot == null)
            {
                return false;
            }

            Transform frameRoot = trayRoot.Find(TrayRootDefinition.FrameRootName);
            if (frameRoot == null)
            {
                return false;
            }

            return frameRoot.Find(TrayRootDefinition.FrameBackgroundName) != null
                && frameRoot.Find(TrayRootDefinition.FrameTrimName) != null;
        }

        public static void BuildFrameVisual(Transform trayRoot)
        {
            if (trayRoot == null)
            {
                Debug.LogWarning("[TrayFrameVisualController] TrayRoot transform is not available.");
                return;
            }

            Transform existingFrameRoot = trayRoot.Find(TrayRootDefinition.FrameRootName);
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

            Vector2 frameSize = TrayRootLayout.GetFrameSize();
            float trimInset = 0.08f;
            float trimWidth = frameSize.x - trimInset * 2f;
            float trimHeight = frameSize.y - trimInset * 2f;
            Sprite frameSprite = GetSharedFrameSprite();

            GameObject frameRootObject = new GameObject(TrayRootDefinition.FrameRootName);
            frameRootObject.transform.SetParent(trayRoot, false);
            frameRootObject.transform.localPosition = TrayRootLayout.GetFrameLocalPosition();
            frameRootObject.transform.localRotation = Quaternion.identity;
            frameRootObject.transform.localScale = Vector3.one;

            CreateFrameRenderer(
                frameRootObject.transform,
                TrayRootDefinition.FrameBackgroundName,
                Vector3.zero,
                new Vector3(frameSize.x, frameSize.y, 1f),
                TrayRootPresentationDefinition.FrameBackgroundColor,
                frameSprite,
                TrayRootPresentationDefinition.FrameBackgroundSortingOrder);

            CreateFrameRenderer(
                frameRootObject.transform,
                TrayRootDefinition.FrameTrimName,
                Vector3.zero,
                new Vector3(trimWidth, trimHeight, 1f),
                TrayRootPresentationDefinition.FrameTrimColor,
                frameSprite,
                TrayRootPresentationDefinition.FrameTrimSortingOrder);

            TrayRootController.EnforceTrayHierarchyOrder(trayRoot);
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
