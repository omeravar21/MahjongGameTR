using System;
using System.Collections.Generic;
using MahjongGame.Board;
using MahjongGame.Tiles;

namespace MahjongGame.BoardGeneration
{
    public static class RewardJokerPatternDefinition
    {
        public static RewardJokerPatternId ClampRewardJokerPatternId(RewardJokerPatternId patternId)
        {
            int rawValue = (int)patternId;
            if (rawValue < 0)
            {
                return RewardJokerPatternId.BalancedSpread;
            }

            if (rawValue >= LevelRecipeDefinition.LaunchRewardJokerPatternCount)
            {
                return (RewardJokerPatternId)(LevelRecipeDefinition.LaunchRewardJokerPatternCount - 1);
            }

            return patternId;
        }

        public static HashSet<int> SelectJokerTileIndices(
            IReadOnlyList<TileSymbolAssignment> assignments,
            RewardJokerPatternId patternId,
            int jokerCount,
            int seed)
        {
            HashSet<int> selected = new HashSet<int>();
            if (assignments == null || assignments.Count == 0 || jokerCount <= 0)
            {
                return selected;
            }

            RewardJokerPatternId clampedPatternId = ClampRewardJokerPatternId(patternId);
            int targetCount = Math.Min(jokerCount, assignments.Count);
            AddPatternIndices(selected, assignments, clampedPatternId, seed, targetCount);
            FillToCount(selected, assignments, seed, targetCount);
            return selected;
        }

        private static void AddPatternIndices(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            RewardJokerPatternId patternId,
            int seed,
            int targetCount)
        {
            switch (patternId)
            {
                case RewardJokerPatternId.UpperLayerFocus:
                    AddUpperLayerIndices(selected, assignments, seed, targetCount);
                    break;
                case RewardJokerPatternId.PathAligned:
                    AddPathAlignedIndices(selected, assignments, seed, targetCount);
                    break;
                case RewardJokerPatternId.CornerAccessible:
                    AddCornerAccessibleIndices(selected, assignments, seed, targetCount);
                    break;
                default:
                    AddBalancedSpreadIndices(selected, assignments, seed, targetCount);
                    break;
            }
        }

        private static void AddBalancedSpreadIndices(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed,
            int targetCount)
        {
            List<int> eligible = CollectEligibleIndices(assignments);
            SortIndicesBySeed(eligible, seed);
            int stride = Math.Max(1, eligible.Count / Math.Max(1, targetCount));
            for (int index = 0; index < eligible.Count && selected.Count < targetCount; index += stride)
            {
                selected.Add(eligible[index]);
            }
        }

        private static void AddUpperLayerIndices(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed,
            int targetCount)
        {
            List<int> eligible = CollectEligibleIndices(assignments);
            eligible.Sort((left, right) =>
            {
                int layerCompare = assignments[right].Position.LayerIndex.CompareTo(
                    assignments[left].Position.LayerIndex);
                if (layerCompare != 0)
                {
                    return layerCompare;
                }

                return CompareSeedHash(left, right, seed);
            });

            for (int index = 0; index < eligible.Count && selected.Count < targetCount; index++)
            {
                selected.Add(eligible[index]);
            }
        }

        private static void AddPathAlignedIndices(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed,
            int targetCount)
        {
            int centerColumn = (BoardGridDefinition.ColumnCount - 1) / 2;
            List<int> eligible = CollectEligibleIndices(assignments);
            eligible.Sort((left, right) =>
            {
                TileSymbolAssignment leftAssignment = assignments[left];
                TileSymbolAssignment rightAssignment = assignments[right];
                int leftDistance = Math.Abs(leftAssignment.Position.GridCoordinate.Column - centerColumn);
                int rightDistance = Math.Abs(rightAssignment.Position.GridCoordinate.Column - centerColumn);
                int compare = leftDistance.CompareTo(rightDistance);
                if (compare != 0)
                {
                    return compare;
                }

                return CompareSeedHash(left, right, seed);
            });

            for (int index = 0; index < eligible.Count && selected.Count < targetCount; index++)
            {
                selected.Add(eligible[index]);
            }
        }

        private static void AddCornerAccessibleIndices(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed,
            int targetCount)
        {
            if (!TryGetLayerZeroBoundingBox(assignments, out int minColumn, out int maxColumn, out int minRow, out int maxRow))
            {
                AddBalancedSpreadIndices(selected, assignments, seed, targetCount);
                return;
            }

            List<int> cornerCandidates = new List<int>();
            for (int index = 0; index < assignments.Count; index++)
            {
                if (!IsEligibleJokerCandidate(assignments[index]))
                {
                    continue;
                }

                if (assignments[index].Position.LayerIndex != 0)
                {
                    continue;
                }

                int column = assignments[index].Position.GridCoordinate.Column;
                int row = assignments[index].Position.GridCoordinate.Row;
                bool isCorner = (column == minColumn || column == maxColumn)
                    && (row == minRow || row == maxRow);
                if (isCorner)
                {
                    cornerCandidates.Add(index);
                }
            }

            SortIndicesBySeed(cornerCandidates, seed);
            for (int index = 0; index < cornerCandidates.Count && selected.Count < targetCount; index++)
            {
                selected.Add(cornerCandidates[index]);
            }
        }

        private static void FillToCount(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed,
            int targetCount)
        {
            if (selected.Count >= targetCount)
            {
                return;
            }

            List<int> fallback = CollectEligibleIndices(assignments);
            SortIndicesBySeed(fallback, seed + 17);
            for (int index = 0; index < fallback.Count && selected.Count < targetCount; index++)
            {
                selected.Add(fallback[index]);
            }
        }

        private static List<int> CollectEligibleIndices(IReadOnlyList<TileSymbolAssignment> assignments)
        {
            List<int> indices = new List<int>();
            for (int index = 0; index < assignments.Count; index++)
            {
                if (IsEligibleJokerCandidate(assignments[index]))
                {
                    indices.Add(index);
                }
            }

            return indices;
        }

        private static bool IsEligibleJokerCandidate(TileSymbolAssignment assignment)
        {
            return !assignment.IsClosed;
        }

        private static bool TryGetLayerZeroBoundingBox(
            IReadOnlyList<TileSymbolAssignment> assignments,
            out int minColumn,
            out int maxColumn,
            out int minRow,
            out int maxRow)
        {
            minColumn = int.MaxValue;
            maxColumn = int.MinValue;
            minRow = int.MaxValue;
            maxRow = int.MinValue;
            bool found = false;

            for (int index = 0; index < assignments.Count; index++)
            {
                if (assignments[index].Position.LayerIndex != 0)
                {
                    continue;
                }

                found = true;
                int column = assignments[index].Position.GridCoordinate.Column;
                int row = assignments[index].Position.GridCoordinate.Row;
                if (column < minColumn)
                {
                    minColumn = column;
                }

                if (column > maxColumn)
                {
                    maxColumn = column;
                }

                if (row < minRow)
                {
                    minRow = row;
                }

                if (row > maxRow)
                {
                    maxRow = row;
                }
            }

            return found;
        }

        private static void SortIndicesBySeed(List<int> indices, int seed)
        {
            indices.Sort((left, right) => CompareSeedHash(left, right, seed));
        }

        private static int CompareSeedHash(int left, int right, int seed)
        {
            int leftHash = PositiveMod((left * 73856093) ^ (seed * 19349663), 1000003);
            int rightHash = PositiveMod((right * 73856093) ^ (seed * 19349663), 1000003);
            return leftHash.CompareTo(rightHash);
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
