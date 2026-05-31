using System.Collections.Generic;
using MahjongGame.Board;
using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.Boosters
{
    public sealed class HintBooster : MonoBehaviour
    {
        [SerializeField] private Transform boardRootTransform;
        [SerializeField] private TileSelectabilityChecker selectabilityChecker;

        private HintPresentationContext _activeHint;

        private void Awake()
        {
            ResolveComponents();
        }

        private void OnEnable()
        {
            BoosterEvents.BoosterRuntimeReset += HandleBoosterRuntimeReset;
        }

        private void OnDisable()
        {
            BoosterEvents.BoosterRuntimeReset -= HandleBoosterRuntimeReset;
            ClearActiveHintPresentation();
        }

        public HintPresentationContext ActiveHint => _activeHint;

        public bool TryExecuteHint()
        {
            ResolveComponents();
            Transform boardRoot = ResolveBoardRootTransform();
            if (boardRoot == null)
            {
                return false;
            }

            if (!TryFindSelectablePair(boardRoot, out Tile firstTile, out Tile secondTile))
            {
                return false;
            }

            ClearActiveHintPresentation();
            ApplyHintPresentation(firstTile, secondTile);
            _activeHint = new HintPresentationContext(firstTile, secondTile);
            return true;
        }

        internal bool TryFindSelectablePairForValidation(
            Transform boardRoot,
            out Tile firstTile,
            out Tile secondTile)
        {
            return TryFindSelectablePair(boardRoot, out firstTile, out secondTile);
        }

        private bool TryFindSelectablePair(
            Transform boardRoot,
            out Tile firstTile,
            out Tile secondTile)
        {
            firstTile = null;
            secondTile = null;

            List<Tile> boardTiles = CollectOnBoardTiles(boardRoot);
            Dictionary<int, List<Tile>> tilesBySymbol = new Dictionary<int, List<Tile>>();

            for (int index = 0; index < boardTiles.Count; index++)
            {
                Tile tile = boardTiles[index];
                if (tile == null || !tile.HasAssignedSymbol)
                {
                    continue;
                }

                if (!IsSelectable(boardRoot, tile))
                {
                    continue;
                }

                if (!tilesBySymbol.TryGetValue(tile.SymbolId, out List<Tile> symbolTiles))
                {
                    symbolTiles = new List<Tile>();
                    tilesBySymbol[tile.SymbolId] = symbolTiles;
                }

                symbolTiles.Add(tile);
            }

            foreach (KeyValuePair<int, List<Tile>> pair in tilesBySymbol)
            {
                List<Tile> symbolTiles = pair.Value;
                if (symbolTiles.Count < 2)
                {
                    continue;
                }

                firstTile = symbolTiles[0];
                secondTile = symbolTiles[1];
                return true;
            }

            return false;
        }

        private bool IsSelectable(Transform boardRoot, Tile tile)
        {
            return TileSelectabilityChecker.TryValidate(boardRoot, tile, out TileSelectabilityResult result)
                && result.IsSelectable;
        }

        private static List<Tile> CollectOnBoardTiles(Transform boardRoot)
        {
            List<Tile> allTiles = BoardTileOccupancyQuery.CollectOccupyingTiles(boardRoot);
            List<Tile> onBoardTiles = new List<Tile>(allTiles.Count);

            for (int index = 0; index < allTiles.Count; index++)
            {
                Tile tile = allTiles[index];
                if (tile == null || !IsOnBoardTile(tile))
                {
                    continue;
                }

                onBoardTiles.Add(tile);
            }

            return onBoardTiles;
        }

        private static bool IsOnBoardTile(Tile tile)
        {
            switch (tile.State)
            {
                case TileState.OnBoard:
                case TileState.Closed:
                case TileState.Revealed:
                    return true;
                default:
                    return false;
            }
        }

        private static void ApplyHintPresentation(Tile firstTile, Tile secondTile)
        {
            ApplyHintHighlight(firstTile, true);
            ApplyHintHighlight(secondTile, true);
        }

        private static void ApplyHintHighlight(Tile tile, bool active)
        {
            if (tile == null)
            {
                return;
            }

            TileView tileView = tile.GetComponent<TileView>();
            tileView?.ApplyHintHighlight(active);
        }

        private void ClearActiveHintPresentation()
        {
            if (_activeHint == null)
            {
                return;
            }

            ApplyHintHighlight(_activeHint.FirstTile, false);
            ApplyHintHighlight(_activeHint.SecondTile, false);
            _activeHint = null;
        }

        private void HandleBoosterRuntimeReset()
        {
            ClearActiveHintPresentation();
        }

        private void ResolveComponents()
        {
            if (selectabilityChecker == null)
            {
                Transform gameplayRoot = transform.parent;
                if (gameplayRoot != null)
                {
                    selectabilityChecker = gameplayRoot.GetComponent<TileSelectabilityChecker>();
                }
            }

            if (boardRootTransform == null)
            {
                Transform gameplayRoot = transform.parent;
                if (gameplayRoot != null)
                {
                    boardRootTransform = gameplayRoot.Find("BoardRoot");
                }
            }

            if (boardRootTransform == null)
            {
                boardRootTransform = BoardTileOccupancyQuery.ResolveBoardRootFromScene();
            }
        }

        private Transform ResolveBoardRootTransform()
        {
            ResolveComponents();
            return boardRootTransform;
        }

        private TileSelectabilityChecker ResolveSelectabilityChecker()
        {
            ResolveComponents();
            return selectabilityChecker;
        }
    }
}
