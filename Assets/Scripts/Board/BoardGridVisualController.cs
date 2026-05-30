using UnityEngine;

namespace MahjongGame.Board
{
    [DefaultExecutionOrder(5)]
    public sealed class BoardGridVisualController : MonoBehaviour
    {
        private static readonly Color DefaultCellColor = new Color(0.35f, 0.28f, 0.22f, 0.35f);

        [SerializeField] private Transform boardRootTransform;
        [SerializeField] private float cellWidth = BoardGridDefinition.DefaultCellWidth;
        [SerializeField] private float cellHeight = BoardGridDefinition.DefaultCellHeight;
        [SerializeField] private Color cellColor = DefaultCellColor;

        private static Sprite _sharedCellSprite;

        private void Awake()
        {
            if (boardRootTransform == null)
            {
                boardRootTransform = transform;
            }

            if (!HasRequiredGridVisual(boardRootTransform))
            {
                BuildGridVisual(boardRootTransform, cellWidth, cellHeight, cellColor);
            }
        }

        public static bool HasRequiredGridVisual(Transform boardRoot)
        {
            if (boardRoot == null)
            {
                return false;
            }

            Transform gridRoot = boardRoot.Find(BoardGridDefinition.GridRootName);
            if (gridRoot == null)
            {
                return false;
            }

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    if (gridRoot.Find(BoardGridDefinition.GetCellName(column, row)) == null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static void BuildGridVisual(Transform boardRoot, float cellWidth, float cellHeight, Color cellColor)
        {
            if (boardRoot == null)
            {
                Debug.LogWarning("[BoardGridVisualController] BoardRoot transform is not available.");
                return;
            }

            Transform existingGridRoot = boardRoot.Find(BoardGridDefinition.GridRootName);
            if (existingGridRoot != null)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(existingGridRoot.gameObject);
                }
                else
                {
                    Object.DestroyImmediate(existingGridRoot.gameObject);
                }
            }

            GameObject gridRootObject = new GameObject(BoardGridDefinition.GridRootName);
            gridRootObject.transform.SetParent(boardRoot, false);
            gridRootObject.transform.localPosition = Vector3.zero;
            gridRootObject.transform.localRotation = Quaternion.identity;
            gridRootObject.transform.localScale = Vector3.one;

            Sprite cellSprite = GetSharedCellSprite();

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    CreateGridCell(gridRootObject.transform, column, row, cellWidth, cellHeight, cellColor, cellSprite);
                }
            }

            gridRootObject.transform.SetAsFirstSibling();
        }

        private static void CreateGridCell(
            Transform gridRoot,
            int column,
            int row,
            float cellWidth,
            float cellHeight,
            Color cellColor,
            Sprite cellSprite)
        {
            GameObject cellObject = new GameObject(BoardGridDefinition.GetCellName(column, row));
            cellObject.transform.SetParent(gridRoot, false);
            cellObject.transform.localPosition = BoardGridLayout.GetCellLocalPosition(column, row, cellWidth, cellHeight);
            cellObject.transform.localRotation = Quaternion.identity;
            cellObject.transform.localScale = new Vector3(cellWidth, cellHeight, 1f);

            SpriteRenderer spriteRenderer = cellObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = cellSprite;
            spriteRenderer.color = cellColor;
            spriteRenderer.sortingOrder = 0;
        }

        private static Sprite GetSharedCellSprite()
        {
            if (_sharedCellSprite != null)
            {
                return _sharedCellSprite;
            }

            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            texture.hideFlags = HideFlags.HideAndDontSave;

            _sharedCellSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, 1f, 1f),
                new Vector2(0.5f, 0.5f),
                1f);
            _sharedCellSprite.hideFlags = HideFlags.HideAndDontSave;
            return _sharedCellSprite;
        }
    }
}
