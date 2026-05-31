using System.Collections.Generic;
using MahjongGame.Board;
using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.Boosters
{
    public sealed class ShuffleBooster : MonoBehaviour
    {
        [SerializeField] private Transform boardRootTransform;

        public bool TryExecuteShuffle(out int shuffledTileCount)
        {
            shuffledTileCount = 0;

            Transform boardRoot = ResolveBoardRootTransform();
            if (boardRoot == null)
            {
                return false;
            }

            List<Tile> boardTiles = CollectOnBoardTiles(boardRoot);
            if (boardTiles.Count < 2)
            {
                return false;
            }

            List<int> symbolIds = new List<int>(boardTiles.Count);
            for (int index = 0; index < boardTiles.Count; index++)
            {
                Tile tile = boardTiles[index];
                if (tile == null || !tile.HasAssignedSymbol)
                {
                    continue;
                }

                symbolIds.Add(tile.SymbolId);
            }

            if (symbolIds.Count < 2)
            {
                return false;
            }

            ShuffleSymbolIdsInPlace(symbolIds, Time.frameCount);
            int symbolIndex = 0;
            for (int tileIndex = 0; tileIndex < boardTiles.Count; tileIndex++)
            {
                Tile tile = boardTiles[tileIndex];
                if (tile == null || !tile.HasAssignedSymbol)
                {
                    continue;
                }

                tile.SetSymbolId(symbolIds[symbolIndex]);
                symbolIndex++;
            }

            shuffledTileCount = symbolIds.Count;
            BoosterEvents.RaiseShuffleExecuted(new ShuffleExecutedContext(shuffledTileCount));
            return true;
        }

        private Transform ResolveBoardRootTransform()
        {
            if (boardRootTransform != null)
            {
                return boardRootTransform;
            }

            Transform gameplayRoot = transform.parent;
            if (gameplayRoot != null)
            {
                boardRootTransform = gameplayRoot.Find("BoardRoot");
            }

            if (boardRootTransform == null)
            {
                boardRootTransform = BoardTileOccupancyQuery.ResolveBoardRootFromScene();
            }

            return boardRootTransform;
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

        private static void ShuffleSymbolIdsInPlace(IList<int> symbolIds, int seed)
        {
            for (int index = symbolIds.Count - 1; index > 0; index--)
            {
                int swapIndex = PositiveMod(ComputeShuffleScore(index, seed), index + 1);
                int temp = symbolIds[index];
                symbolIds[index] = symbolIds[swapIndex];
                symbolIds[swapIndex] = temp;
            }
        }

        private static int ComputeShuffleScore(int index, int seed)
        {
            unchecked
            {
                return (index * 1103515245) ^ (seed * 12345);
            }
        }

        private static int PositiveMod(int value, int modulus)
        {
            if (modulus <= 0)
            {
                return 0;
            }

            int result = value % modulus;
            return result < 0 ? result + modulus : result;
        }
    }
}
