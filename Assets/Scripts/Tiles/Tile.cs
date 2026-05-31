using MahjongGame.Board;
using UnityEngine;

namespace MahjongGame.Tiles
{
    public sealed class Tile : MonoBehaviour
    {
        [SerializeField] private TileView tileView;

        private TileData _data;

        public TileState State { get; private set; } = TileState.OnBoard;

        public TileData Data => _data;

        public int TileId => _data != null ? _data.TileId : -1;

        public BoardGridCoordinate GridCoordinate => _data != null ? _data.GridCoordinate : default;

        public int LayerIndex => _data != null ? _data.LayerIndex : 0;

        public TileType Type => _data != null ? _data.Type : TileType.Normal;

        private void Awake()
        {
            if (tileView == null)
            {
                tileView = GetComponent<TileView>();
            }

            if (tileView == null)
            {
                tileView = gameObject.AddComponent<TileView>();
            }

            if (!HasRequiredTileHierarchy(transform))
            {
                BuildTileHierarchy(transform);
            }

            tileView.CacheReferencesFromHierarchy();
            tileView.ApplyVisualState(State, Type);
        }

        public static bool HasRequiredTileHierarchy(Transform tileRoot)
        {
            if (tileRoot == null)
            {
                return false;
            }

            return tileRoot.Find("TileFace") != null
                && tileRoot.Find("TileSymbol") != null
                && tileRoot.Find("ClosedOverlay") != null;
        }

        public static void BuildTileHierarchy(Transform tileRoot)
        {
            if (tileRoot == null)
            {
                Debug.LogWarning("[Tile] Tile root transform is not available.");
                return;
            }

            TileView.BuildVisualHierarchy(
                tileRoot,
                BoardGridDefinition.DefaultCellWidth,
                BoardGridDefinition.DefaultCellHeight,
                GetSharedTileSprite());
        }

        public void Initialize(TileData data)
        {
            _data = data;
            State = data != null && (data.IsClosed || data.Type == TileType.Closed)
                ? TileState.Closed
                : TileState.OnBoard;

            if (tileView == null)
            {
                tileView = GetComponent<TileView>();
            }

            tileView?.CacheReferencesFromHierarchy();
            tileView?.ApplyVisualState(State, Type);

            if (data != null)
            {
                ApplySortingOrder(data.LayerIndex, data.GridCoordinate.Row, data.GridCoordinate.Column);
            }
        }

        public void SetState(TileState state)
        {
            State = state;
            tileView?.ApplyVisualState(State, Type);
        }

        public void ApplySortingOrder(int layerIndex, int row, int column)
        {
            TileSortingController.ApplySorting(tileView, layerIndex, row, column);
            tileView?.ApplyLayerDepthVisuals(layerIndex);
        }

        private static Sprite _sharedTileSprite;

        private static Sprite GetSharedTileSprite()
        {
            if (_sharedTileSprite != null)
            {
                return _sharedTileSprite;
            }

            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            texture.hideFlags = HideFlags.HideAndDontSave;

            _sharedTileSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, 1f, 1f),
                new Vector2(0.5f, 0.5f),
                1f);
            _sharedTileSprite.hideFlags = HideFlags.HideAndDontSave;
            return _sharedTileSprite;
        }
    }
}