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

        public int SymbolId => _data != null ? _data.SymbolId : TileData.UnassignedSymbolId;

        public TileBoardPosition OriginalBoardPosition =>
            _data != null ? _data.OriginalBoardPosition : default;

        public bool IsClosed => _data != null && _data.IsClosedTile;

        public bool IsJoker => _data != null && _data.IsRewardJoker;

        public bool HasAssignedSymbol => _data != null && _data.HasAssignedSymbol;

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
            EnsureSelectionCollider();
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

            EnsureSelectionCollider(tileRoot);
        }

        public static void EnsureSelectionCollider(Transform tileRoot)
        {
            if (tileRoot == null)
            {
                return;
            }

            BoxCollider2D collider = tileRoot.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = tileRoot.gameObject.AddComponent<BoxCollider2D>();
            }

            collider.size = Vector2.one;
            collider.offset = Vector2.zero;
        }

        private void EnsureSelectionCollider()
        {
            EnsureSelectionCollider(transform);
        }

        public TileIdentity GetIdentity()
        {
            return _data != null ? _data.Identity : default;
        }

        public void Initialize(TileData data)
        {
            if (data == null)
            {
                Debug.LogWarning("[Tile] TileData is not available.");
                return;
            }

            if (!data.HasValidIdentity)
            {
                Debug.LogWarning("[Tile] TileData has an invalid identity: " + data.TileId + ".");
                return;
            }

            if (!data.OriginalBoardPosition.IsValid)
            {
                Debug.LogWarning("[Tile] TileData has an invalid original board position.");
                return;
            }

            _data = data;
            State = data.IsClosedTile ? TileState.Closed : TileState.OnBoard;

            if (tileView == null)
            {
                tileView = GetComponent<TileView>();
            }

            tileView?.CacheReferencesFromHierarchy();
            tileView?.ApplyVisualState(State, Type);
            ApplySortingOrder(data.LayerIndex, data.GridCoordinate.Row, data.GridCoordinate.Column);
        }

        public void SetState(TileState state)
        {
            State = state;
            tileView?.ApplyVisualState(State, Type);
        }

        public void SetSymbolId(int symbolId)
        {
            if (_data != null)
            {
                _data.SymbolId = symbolId;
            }
        }

        public void ApplySortingOrder(int layerIndex, int row, int column)
        {
            TileSortingController.ApplySorting(tileView, layerIndex, row, column);
            tileView?.ApplyLayerDepthVisuals(layerIndex);
        }

        public void ApplyTraySorting(int slotIndex)
        {
            TileSortingController.ApplyTraySorting(tileView, slotIndex);
        }

        public void SetColliderEnabled(bool isEnabled)
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.enabled = isEnabled;
            }
        }

        private static Sprite _sharedTileSprite;

        private const string SharedTileSpriteAssetPath = "Assets/Sprites/TileSharedWhite.png";

        private static Sprite GetSharedTileSprite()
        {
            if (_sharedTileSprite != null)
            {
                return _sharedTileSprite;
            }

            _sharedTileSprite = LoadSharedTileSpriteAsset();
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

        private static Sprite LoadSharedTileSpriteAsset()
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(SharedTileSpriteAssetPath);
#else
            return null;
#endif
        }
    }
}
