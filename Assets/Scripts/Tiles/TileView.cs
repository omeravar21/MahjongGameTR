using UnityEngine;

namespace MahjongGame.Tiles
{
    public sealed class TileView : MonoBehaviour
    {
        private static readonly Color DefaultFaceColor = new Color(0.96f, 0.93f, 0.86f, 1f);
        private static readonly Color DefaultSymbolColor = new Color(0.35f, 0.28f, 0.22f, 1f);
        private static readonly Color DefaultClosedOverlayColor = new Color(0.45f, 0.35f, 0.28f, 0.85f);
        private const float DefaultSymbolScale = 0.6f;

        [SerializeField] private SpriteRenderer tileFaceRenderer;
        [SerializeField] private SpriteRenderer tileSymbolRenderer;
        [SerializeField] private SpriteRenderer closedOverlayRenderer;

        public SpriteRenderer TileFaceRenderer => tileFaceRenderer;
        public SpriteRenderer TileSymbolRenderer => tileSymbolRenderer;
        public SpriteRenderer ClosedOverlayRenderer => closedOverlayRenderer;

        public void CacheReferencesFromHierarchy()
        {
            if (tileFaceRenderer == null)
            {
                tileFaceRenderer = transform.Find("TileFace")?.GetComponent<SpriteRenderer>();
            }

            if (tileSymbolRenderer == null)
            {
                tileSymbolRenderer = transform.Find("TileSymbol")?.GetComponent<SpriteRenderer>();
            }

            if (closedOverlayRenderer == null)
            {
                closedOverlayRenderer = transform.Find("ClosedOverlay")?.GetComponent<SpriteRenderer>();
            }
        }

        public static void BuildVisualHierarchy(Transform tileRoot, float tileWidth, float tileHeight, Sprite sharedSprite)
        {
            if (tileRoot == null)
            {
                Debug.LogWarning("[TileView] Tile root transform is not available.");
                return;
            }

            tileRoot.localScale = new Vector3(tileWidth, tileHeight, 1f);

            CreateChildRenderer(tileRoot, "TileFace", Vector3.zero, Vector3.one, DefaultFaceColor, sharedSprite, 0);
            CreateChildRenderer(tileRoot, "TileSymbol", Vector3.zero, new Vector3(DefaultSymbolScale, DefaultSymbolScale, 1f), DefaultSymbolColor, sharedSprite, 1);

            Transform closedOverlay = CreateChildRenderer(tileRoot, "ClosedOverlay", Vector3.zero, Vector3.one, DefaultClosedOverlayColor, sharedSprite, 2);
            closedOverlay.gameObject.SetActive(false);
        }

        public void ApplyVisualState(TileState state, TileType type)
        {
            bool showClosedOverlay = state == TileState.Closed || type == TileType.Closed;
            bool showSymbol = state != TileState.Closed && type != TileType.Closed;

            if (closedOverlayRenderer != null)
            {
                closedOverlayRenderer.gameObject.SetActive(showClosedOverlay);
            }

            if (tileSymbolRenderer != null)
            {
                tileSymbolRenderer.enabled = showSymbol;
            }
        }

        public void ApplySorting(int baseOrder)
        {
            if (tileFaceRenderer != null)
            {
                tileFaceRenderer.sortingOrder = baseOrder;
            }

            if (tileSymbolRenderer != null)
            {
                tileSymbolRenderer.sortingOrder = baseOrder + 1;
            }

            if (closedOverlayRenderer != null)
            {
                closedOverlayRenderer.sortingOrder = baseOrder + 2;
            }
        }

        private static Transform CreateChildRenderer(
            Transform parent,
            string childName,
            Vector3 localPosition,
            Vector3 localScale,
            Color color,
            Sprite sprite,
            int sortingOrder)
        {
            Transform existingChild = parent.Find(childName);
            if (existingChild != null)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(existingChild.gameObject);
                }
                else
                {
                    Object.DestroyImmediate(existingChild.gameObject);
                }
            }

            GameObject childObject = new GameObject(childName);
            childObject.transform.SetParent(parent, false);
            childObject.transform.localPosition = localPosition;
            childObject.transform.localRotation = Quaternion.identity;
            childObject.transform.localScale = localScale;

            SpriteRenderer spriteRenderer = childObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = sortingOrder;
            return childObject.transform;
        }
    }
}