using System.Collections.Generic;
using MahjongGame.Board;
using MahjongGame.Tiles;

namespace MahjongGame.BoardGeneration
{
    public static class TilePairDistributionDefinition
    {
        public const int LaunchSymbolCount = 72;

        public static IReadOnlyList<TileSymbolAssignment> BuildAssignments(
            IReadOnlyList<TileBoardPosition> positions,
            int levelNumber,
            int seed)
        {
            if (positions == null || positions.Count == 0)
            {
                return new TileSymbolAssignment[0];
            }

            int effectiveTileCount = positions.Count;
            if (effectiveTileCount % 2 != 0)
            {
                effectiveTileCount--;
            }

            if (effectiveTileCount <= 0)
            {
                return new TileSymbolAssignment[0];
            }

            int pairCount = effectiveTileCount / 2;
            List<int> selectedSymbolIds = SelectSymbolIds(pairCount, levelNumber, seed);
            List<int> shuffledSymbolIds = BuildShuffledPairList(selectedSymbolIds, seed);
            ReduceAdjacentClustering(positions, shuffledSymbolIds, effectiveTileCount, seed);

            TileSymbolAssignment[] assignments = new TileSymbolAssignment[effectiveTileCount];
            for (int index = 0; index < effectiveTileCount; index++)
            {
                assignments[index] = new TileSymbolAssignment(
                    index,
                    positions[index],
                    shuffledSymbolIds[index]);
            }

            return assignments;
        }

        public static int CountDistinctSymbols(IReadOnlyList<TileSymbolAssignment> assignments)
        {
            if (assignments == null || assignments.Count == 0)
            {
                return 0;
            }

            HashSet<int> distinctSymbols = new HashSet<int>();
            for (int index = 0; index < assignments.Count; index++)
            {
                distinctSymbols.Add(assignments[index].SymbolId);
            }

            return distinctSymbols.Count;
        }

        private static List<int> SelectSymbolIds(int pairCount, int levelNumber, int seed)
        {
            List<int> symbolIds = new List<int>(pairCount);
            int startIndex = PositiveMod((levelNumber * 17) + (seed * 31), LaunchSymbolCount);

            for (int index = 0; index < pairCount; index++)
            {
                symbolIds.Add((startIndex + index) % LaunchSymbolCount);
            }

            return symbolIds;
        }

        private static List<int> BuildShuffledPairList(IReadOnlyList<int> selectedSymbolIds, int seed)
        {
            List<int> pairList = new List<int>(selectedSymbolIds.Count * 2);

            for (int index = 0; index < selectedSymbolIds.Count; index++)
            {
                int symbolId = selectedSymbolIds[index];
                pairList.Add(symbolId);
                pairList.Add(symbolId);
            }

            ShuffleInPlace(pairList, seed);
            return pairList;
        }

        private static void ReduceAdjacentClustering(
            IReadOnlyList<TileBoardPosition> positions,
            List<int> symbolIds,
            int effectiveTileCount,
            int seed)
        {
            for (int pass = 0; pass < effectiveTileCount; pass++)
            {
                int index = PositiveMod(seed + pass, effectiveTileCount);

                if (!HasAdjacentSameSymbol(positions, symbolIds, index))
                {
                    continue;
                }

                int swapIndex = FindSwapCandidate(positions, symbolIds, index, effectiveTileCount);
                if (swapIndex < 0)
                {
                    continue;
                }

                int temp = symbolIds[index];
                symbolIds[index] = symbolIds[swapIndex];
                symbolIds[swapIndex] = temp;
            }
        }

        private static bool HasAdjacentSameSymbol(
            IReadOnlyList<TileBoardPosition> positions,
            IReadOnlyList<int> symbolIds,
            int index)
        {
            int symbolId = symbolIds[index];

            for (int neighborIndex = 0; neighborIndex < positions.Count; neighborIndex++)
            {
                if (neighborIndex == index)
                {
                    continue;
                }

                if (AreAdjacent(positions[index], positions[neighborIndex])
                    && symbolIds[neighborIndex] == symbolId)
                {
                    return true;
                }
            }

            return false;
        }

        private static int FindSwapCandidate(
            IReadOnlyList<TileBoardPosition> positions,
            IReadOnlyList<int> symbolIds,
            int index,
            int effectiveTileCount)
        {
            int symbolId = symbolIds[index];

            for (int candidateIndex = 0; candidateIndex < effectiveTileCount; candidateIndex++)
            {
                if (candidateIndex == index || symbolIds[candidateIndex] == symbolId)
                {
                    continue;
                }

                if (AreAdjacent(positions[index], positions[candidateIndex]))
                {
                    continue;
                }

                return candidateIndex;
            }

            return -1;
        }

        private static bool AreAdjacent(TileBoardPosition left, TileBoardPosition right)
        {
            if (left.LayerIndex != right.LayerIndex)
            {
                return false;
            }

            int columnDelta = left.GridCoordinate.Column - right.GridCoordinate.Column;
            int rowDelta = left.GridCoordinate.Row - right.GridCoordinate.Row;

            if (columnDelta == 0 && (rowDelta == 1 || rowDelta == -1))
            {
                return true;
            }

            if (rowDelta == 0 && (columnDelta == 1 || columnDelta == -1))
            {
                return true;
            }

            return false;
        }

        private static void ShuffleInPlace(IList<int> values, int seed)
        {
            for (int index = values.Count - 1; index > 0; index--)
            {
                int swapIndex = PositiveMod(ComputeShuffleScore(index, seed), index + 1);
                int temp = values[index];
                values[index] = values[swapIndex];
                values[swapIndex] = temp;
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
