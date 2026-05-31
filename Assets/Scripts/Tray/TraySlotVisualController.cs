using UnityEngine;

namespace MahjongGame.Tray
{
    public static class TraySlotVisualController
    {
        private static Sprite _sharedSlotSprite;

        public static bool HasAllSlotVisuals(Transform trayContainer)
        {
            if (trayContainer == null)
            {
                return false;
            }

            for (int slotIndex = 0; slotIndex < TrayRootDefinition.SlotCount; slotIndex++)
            {
                Transform slotTransform = trayContainer.Find(TrayRootDefinition.GetSlotName(slotIndex));
                if (!HasRequiredSlotVisual(slotTransform))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool HasRequiredSlotVisual(Transform slotTransform)
        {
            if (slotTransform == null)
            {
                return false;
            }

            return slotTransform.Find(TraySlotDefinition.SlotBackgroundName) != null
                && slotTransform.Find(TraySlotDefinition.SlotTrimName) != null;
        }

        public static void BuildAllSlotVisuals(Transform trayContainer)
        {
            if (trayContainer == null)
            {
                Debug.LogWarning("[TraySlotVisualController] TrayContainer transform is not available.");
                return;
            }

            for (int slotIndex = 0; slotIndex < TrayRootDefinition.SlotCount; slotIndex++)
            {
                Transform slotTransform = trayContainer.Find(TrayRootDefinition.GetSlotName(slotIndex));
                if (slotTransform == null)
                {
                    Debug.LogWarning("[TraySlotVisualController] Missing slot anchor: " + TrayRootDefinition.GetSlotName(slotIndex));
                    continue;
                }

                BuildSlotVisual(slotTransform, slotIndex);
            }
        }

        public static void BuildSlotVisual(Transform slotTransform, int slotIndex)
        {
            if (slotTransform == null)
            {
                return;
            }

            ClearExistingSlotVisuals(slotTransform);

            float trimWidth = TraySlotDefinition.SlotWidth - TraySlotDefinition.TrimInset * 2f;
            float trimHeight = TraySlotDefinition.SlotHeight - TraySlotDefinition.TrimInset * 2f;
            Sprite slotSprite = GetSharedSlotSprite();

            CreateSlotRenderer(
                slotTransform,
                TraySlotDefinition.SlotBackgroundName,
                Vector3.zero,
                new Vector3(TraySlotDefinition.SlotWidth, TraySlotDefinition.SlotHeight, 1f),
                TraySlotDefinition.SlotBackgroundColor,
                slotSprite,
                TraySlotDefinition.GetSlotBackgroundSortingOrder(slotIndex));

            CreateSlotRenderer(
                slotTransform,
                TraySlotDefinition.SlotTrimName,
                Vector3.zero,
                new Vector3(trimWidth, trimHeight, 1f),
                TraySlotDefinition.SlotTrimColor,
                slotSprite,
                TraySlotDefinition.GetSlotTrimSortingOrder(slotIndex));
        }

        private static void ClearExistingSlotVisuals(Transform slotTransform)
        {
            DestroyChildIfPresent(slotTransform, TraySlotDefinition.SlotBackgroundName);
            DestroyChildIfPresent(slotTransform, TraySlotDefinition.SlotTrimName);
        }

        private static void DestroyChildIfPresent(Transform slotTransform, string childName)
        {
            Transform existingChild = slotTransform.Find(childName);
            if (existingChild == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(existingChild.gameObject);
            }
            else
            {
                Object.DestroyImmediate(existingChild.gameObject);
            }
        }

        private static void CreateSlotRenderer(
            Transform slotTransform,
            string childName,
            Vector3 localPosition,
            Vector3 localScale,
            Color color,
            Sprite sprite,
            int sortingOrder)
        {
            GameObject childObject = new GameObject(childName);
            childObject.transform.SetParent(slotTransform, false);
            childObject.transform.localPosition = localPosition;
            childObject.transform.localRotation = Quaternion.identity;
            childObject.transform.localScale = localScale;

            SpriteRenderer spriteRenderer = childObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = sortingOrder;
        }

        private static Sprite GetSharedSlotSprite()
        {
            if (_sharedSlotSprite != null)
            {
                return _sharedSlotSprite;
            }

            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            texture.hideFlags = HideFlags.HideAndDontSave;

            _sharedSlotSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, 1f, 1f),
                new Vector2(0.5f, 0.5f),
                1f);
            _sharedSlotSprite.hideFlags = HideFlags.HideAndDontSave;
            return _sharedSlotSprite;
        }
    }
}
