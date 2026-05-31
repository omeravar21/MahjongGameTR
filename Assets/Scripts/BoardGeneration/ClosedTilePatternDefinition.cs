using System;
using System.Collections.Generic;
using MahjongGame.Board;
using MahjongGame.ClosedTiles;
using MahjongGame.Tiles;

namespace MahjongGame.BoardGeneration
{
    public static class ClosedTilePatternDefinition
    {
        public static ClosedTilePatternId ClampClosedTilePatternId(ClosedTilePatternId patternId)
        {
            int rawValue = (int)patternId;
            if (rawValue < 0)
            {
                return ClosedTilePatternId.CornerSingle;
            }

            if (rawValue >= VisualVarietyDefinition.LaunchClosedTilePatternCount)
            {
                return (ClosedTilePatternId)(VisualVarietyDefinition.LaunchClosedTilePatternCount - 1);
            }

            return patternId;
        }

        public static HashSet<int> SelectClosedTileIndices(
            IReadOnlyList<TileSymbolAssignment> assignments,
            ClosedTilePatternId patternId,
            int closedTileCount,
            int seed,
            int levelNumber)
        {
            HashSet<int> selected = new HashSet<int>();
            if (assignments == null
                || assignments.Count == 0
                || closedTileCount <= 0
                || !ClosedTileDefinition.IsClosedTileMechanicActive(levelNumber))
            {
                return selected;
            }

            ClosedTilePatternId clampedPatternId = ClampClosedTilePatternId(patternId);
            int targetCount = Math.Min(closedTileCount, assignments.Count);
            AddPatternIndices(selected, assignments, clampedPatternId, seed, targetCount);
            FillToCount(selected, assignments, seed, targetCount);
            return selected;
        }

        private static void AddPatternIndices(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            ClosedTilePatternId patternId,
            int seed,
            int targetCount)
        {
            switch (patternId)
            {
                case ClosedTilePatternId.CornerSingle:
                    AddCornerIndices(selected, assignments, seed, 1);
                    break;
                case ClosedTilePatternId.CornerPair:
                    AddCornerIndices(selected, assignments, seed, 2);
                    break;
                case ClosedTilePatternId.CenterPair:
                    AddNearestToCenter(selected, assignments, seed, 2);
                    break;
                case ClosedTilePatternId.CenterTriple:
                    AddNearestToCenter(selected, assignments, seed, 3);
                    break;
                case ClosedTilePatternId.HorizontalLine3:
                    AddHorizontalLine(selected, assignments, seed, 3);
                    break;
                case ClosedTilePatternId.HorizontalLine5:
                    AddHorizontalLine(selected, assignments, seed, 5);
                    break;
                case ClosedTilePatternId.VerticalLine3:
                    AddVerticalLine(selected, assignments, seed, 3);
                    break;
                case ClosedTilePatternId.LShape:
                    AddLShape(selected, assignments, seed);
                    break;
                case ClosedTilePatternId.PlusShape:
                    AddPlusShape(selected, assignments, seed);
                    break;
                case ClosedTilePatternId.DiagonalPair:
                    AddDiagonalPair(selected, assignments, seed);
                    break;
                case ClosedTilePatternId.StackedPair:
                    AddStackedPair(selected, assignments, seed);
                    break;
                case ClosedTilePatternId.HiddenUnderLayer:
                    AddHiddenUnderLayer(selected, assignments, seed, targetCount);
                    break;
                case ClosedTilePatternId.SandwichClosed:
                    AddSandwichClosed(selected, assignments, seed, targetCount);
                    break;
                case ClosedTilePatternId.MixedCornersAndCenter:
                    AddMixedCornersAndCenter(selected, assignments, seed, targetCount);
                    break;
                default:
                    AddCornerIndices(selected, assignments, seed, 1);
                    break;
            }
        }

        private static void AddCornerIndices(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed,
            int count)
        {
            if (!TryGetLayerZeroBoundingBox(assignments, out int minColumn, out int maxColumn, out int minRow, out int maxRow))
            {
                return;
            }

            List<int> cornerCandidates = new List<int>();
            for (int index = 0; index < assignments.Count; index++)
            {
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
            for (int i = 0; i < cornerCandidates.Count && selected.Count < count; i++)
            {
                selected.Add(cornerCandidates[i]);
            }
        }

        private static void AddNearestToCenter(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed,
            int count)
        {
            List<int> layerZeroIndices = CollectLayerZeroIndices(assignments);
            float centerColumn = (BoardGridDefinition.ColumnCount - 1) / 2f;
            float centerRow = (BoardGridDefinition.RowCount - 1) / 2f;

            layerZeroIndices.Sort((left, right) =>
            {
                TileSymbolAssignment leftAssignment = assignments[left];
                TileSymbolAssignment rightAssignment = assignments[right];
                float leftScore = DistanceSquared(
                    leftAssignment.Position.GridCoordinate.Column,
                    leftAssignment.Position.GridCoordinate.Row,
                    centerColumn,
                    centerRow)
                    + (PositiveMod(seed + left, 13) * 0.001f);
                float rightScore = DistanceSquared(
                    rightAssignment.Position.GridCoordinate.Column,
                    rightAssignment.Position.GridCoordinate.Row,
                    centerColumn,
                    centerRow)
                    + (PositiveMod(seed + right, 13) * 0.001f);
                return leftScore.CompareTo(rightScore);
            });

            for (int i = 0; i < layerZeroIndices.Count && selected.Count < count; i++)
            {
                selected.Add(layerZeroIndices[i]);
            }
        }

        private static void AddHorizontalLine(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed,
            int lineLength)
        {
            Dictionary<int, List<int>> rowToIndices = new Dictionary<int, List<int>>();
            for (int index = 0; index < assignments.Count; index++)
            {
                if (assignments[index].Position.LayerIndex != 0)
                {
                    continue;
                }

                int row = assignments[index].Position.GridCoordinate.Row;
                if (!rowToIndices.ContainsKey(row))
                {
                    rowToIndices[row] = new List<int>();
                }

                rowToIndices[row].Add(index);
            }

            List<int> bestRun = new List<int>();
            foreach (KeyValuePair<int, List<int>> entry in rowToIndices)
            {
                List<int> sorted = entry.Value;
                sorted.Sort((left, right) =>
                    assignments[left].Position.GridCoordinate.Column
                        .CompareTo(assignments[right].Position.GridCoordinate.Column));

                for (int start = 0; start < sorted.Count; start++)
                {
                    List<int> run = new List<int> { sorted[start] };
                    int expectedColumn = assignments[sorted[start]].Position.GridCoordinate.Column + 1;
                    for (int next = start + 1; next < sorted.Count; next++)
                    {
                        int column = assignments[sorted[next]].Position.GridCoordinate.Column;
                        if (column == expectedColumn)
                        {
                            run.Add(sorted[next]);
                            expectedColumn++;
                        }
                        else if (column > expectedColumn)
                        {
                            break;
                        }
                    }

                    if (run.Count > bestRun.Count
                        || (run.Count > 0
                            && run.Count == bestRun.Count
                            && PositiveMod(seed + run[0], 7) < PositiveMod(seed + bestRun[0], 7)))
                    {
                        bestRun = run;
                    }
                }
            }

            int takeCount = Math.Min(lineLength, bestRun.Count);
            for (int i = 0; i < takeCount; i++)
            {
                selected.Add(bestRun[i]);
            }
        }

        private static void AddVerticalLine(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed,
            int lineLength)
        {
            Dictionary<int, List<int>> columnToIndices = new Dictionary<int, List<int>>();
            for (int index = 0; index < assignments.Count; index++)
            {
                if (assignments[index].Position.LayerIndex != 0)
                {
                    continue;
                }

                int column = assignments[index].Position.GridCoordinate.Column;
                if (!columnToIndices.ContainsKey(column))
                {
                    columnToIndices[column] = new List<int>();
                }

                columnToIndices[column].Add(index);
            }

            List<int> bestRun = new List<int>();
            foreach (KeyValuePair<int, List<int>> entry in columnToIndices)
            {
                List<int> sorted = entry.Value;
                sorted.Sort((left, right) =>
                    assignments[left].Position.GridCoordinate.Row
                        .CompareTo(assignments[right].Position.GridCoordinate.Row));

                for (int start = 0; start < sorted.Count; start++)
                {
                    List<int> run = new List<int> { sorted[start] };
                    int expectedRow = assignments[sorted[start]].Position.GridCoordinate.Row + 1;
                    for (int next = start + 1; next < sorted.Count; next++)
                    {
                        int row = assignments[sorted[next]].Position.GridCoordinate.Row;
                        if (row == expectedRow)
                        {
                            run.Add(sorted[next]);
                            expectedRow++;
                        }
                        else if (row > expectedRow)
                        {
                            break;
                        }
                    }

                    if (run.Count > bestRun.Count
                        || (run.Count > 0
                            && run.Count == bestRun.Count
                            && PositiveMod(seed + run[0], 7) < PositiveMod(seed + bestRun[0], 7)))
                    {
                        bestRun = run;
                    }
                }
            }

            int takeCount = Math.Min(lineLength, bestRun.Count);
            for (int i = 0; i < takeCount; i++)
            {
                selected.Add(bestRun[i]);
            }
        }

        private static void AddLShape(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed)
        {
            int anchorIndex = SelectCenterAnchorIndex(assignments, seed);
            if (anchorIndex < 0)
            {
                return;
            }

            selected.Add(anchorIndex);
            TileBoardPosition anchor = assignments[anchorIndex].Position;
            int horizontalIndex = FindNearestIndexOnLayerZero(
                assignments,
                anchor.GridCoordinate.Column + 1,
                anchor.GridCoordinate.Row,
                anchor.LayerIndex,
                seed,
                selected);
            int verticalIndex = FindNearestIndexOnLayerZero(
                assignments,
                anchor.GridCoordinate.Column,
                anchor.GridCoordinate.Row + 1,
                anchor.LayerIndex,
                seed + 1,
                selected);

            if (horizontalIndex >= 0)
            {
                selected.Add(horizontalIndex);
            }

            if (verticalIndex >= 0)
            {
                selected.Add(verticalIndex);
            }
        }

        private static void AddPlusShape(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed)
        {
            int anchorIndex = SelectCenterAnchorIndex(assignments, seed);
            if (anchorIndex < 0)
            {
                return;
            }

            selected.Add(anchorIndex);
            TileBoardPosition anchor = assignments[anchorIndex].Position;
            int[] columnOffsets = { 1, -1, 0, 0 };
            int[] rowOffsets = { 0, 0, 1, -1 };

            for (int offsetIndex = 0; offsetIndex < columnOffsets.Length; offsetIndex++)
            {
                int candidateIndex = FindNearestIndexOnLayerZero(
                    assignments,
                    anchor.GridCoordinate.Column + columnOffsets[offsetIndex],
                    anchor.GridCoordinate.Row + rowOffsets[offsetIndex],
                    anchor.LayerIndex,
                    seed + offsetIndex,
                    selected);
                if (candidateIndex >= 0)
                {
                    selected.Add(candidateIndex);
                }
            }
        }

        private static void AddDiagonalPair(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed)
        {
            List<int> layerZeroIndices = CollectLayerZeroIndices(assignments);
            int bestFirst = -1;
            int bestSecond = -1;
            int bestScore = int.MaxValue;

            for (int first = 0; first < layerZeroIndices.Count; first++)
            {
                for (int second = first + 1; second < layerZeroIndices.Count; second++)
                {
                    int firstIndex = layerZeroIndices[first];
                    int secondIndex = layerZeroIndices[second];
                    BoardGridCoordinate firstCoordinate = assignments[firstIndex].Position.GridCoordinate;
                    BoardGridCoordinate secondCoordinate = assignments[secondIndex].Position.GridCoordinate;
                    int deltaColumn = secondCoordinate.Column - firstCoordinate.Column;
                    int deltaRow = secondCoordinate.Row - firstCoordinate.Row;
                    if (deltaColumn == 0 || deltaRow == 0 || Math.Abs(deltaColumn) != Math.Abs(deltaRow))
                    {
                        continue;
                    }

                    int score = Math.Abs(deltaColumn) + PositiveMod(seed + firstIndex + secondIndex, 11);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestFirst = firstIndex;
                        bestSecond = secondIndex;
                    }
                }
            }

            if (bestFirst >= 0 && bestSecond >= 0)
            {
                selected.Add(bestFirst);
                selected.Add(bestSecond);
            }
        }

        private static void AddStackedPair(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed)
        {
            Dictionary<long, List<int>> cellStacks = new Dictionary<long, List<int>>();
            for (int index = 0; index < assignments.Count; index++)
            {
                TileSymbolAssignment assignment = assignments[index];
                long key = PackCellKey(
                    assignment.Position.GridCoordinate.Column,
                    assignment.Position.GridCoordinate.Row);
                if (!cellStacks.ContainsKey(key))
                {
                    cellStacks[key] = new List<int>();
                }

                cellStacks[key].Add(index);
            }

            int bestFirst = -1;
            int bestSecond = -1;
            int bestScore = int.MaxValue;
            foreach (KeyValuePair<long, List<int>> entry in cellStacks)
            {
                if (entry.Value.Count < 2)
                {
                    continue;
                }

                entry.Value.Sort((left, right) =>
                    assignments[left].Position.LayerIndex.CompareTo(assignments[right].Position.LayerIndex));

                int firstIndex = entry.Value[0];
                int secondIndex = entry.Value[1];
                int score = assignments[secondIndex].Position.LayerIndex
                    + PositiveMod(seed + firstIndex, 17);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestFirst = firstIndex;
                    bestSecond = secondIndex;
                }
            }

            if (bestFirst >= 0 && bestSecond >= 0)
            {
                selected.Add(bestFirst);
                selected.Add(bestSecond);
            }
        }

        private static void AddHiddenUnderLayer(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed,
            int targetCount)
        {
            List<int> upperLayerIndices = new List<int>();
            for (int index = 0; index < assignments.Count; index++)
            {
                if (assignments[index].Position.LayerIndex >= 1)
                {
                    upperLayerIndices.Add(index);
                }
            }

            SortIndicesBySeed(upperLayerIndices, seed);
            for (int i = 0; i < upperLayerIndices.Count && selected.Count < targetCount; i++)
            {
                selected.Add(upperLayerIndices[i]);
            }
        }

        private static void AddSandwichClosed(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed,
            int targetCount)
        {
            HashSet<long> occupiedLayerZeroCells = new HashSet<long>();
            for (int index = 0; index < assignments.Count; index++)
            {
                TileSymbolAssignment assignment = assignments[index];
                if (assignment.Position.LayerIndex != 0)
                {
                    continue;
                }

                occupiedLayerZeroCells.Add(PackCellKey(
                    assignment.Position.GridCoordinate.Column,
                    assignment.Position.GridCoordinate.Row));
            }

            List<int> sandwichCandidates = new List<int>();
            for (int index = 0; index < assignments.Count; index++)
            {
                TileSymbolAssignment assignment = assignments[index];
                if (assignment.Position.LayerIndex != 0)
                {
                    continue;
                }

                int column = assignment.Position.GridCoordinate.Column;
                int row = assignment.Position.GridCoordinate.Row;
                long leftKey = PackCellKey(column - 1, row);
                long rightKey = PackCellKey(column + 1, row);
                if (occupiedLayerZeroCells.Contains(leftKey) && occupiedLayerZeroCells.Contains(rightKey))
                {
                    sandwichCandidates.Add(index);
                }
            }

            SortIndicesBySeed(sandwichCandidates, seed);
            for (int i = 0; i < sandwichCandidates.Count && selected.Count < targetCount; i++)
            {
                selected.Add(sandwichCandidates[i]);
            }
        }

        private static void AddMixedCornersAndCenter(
            HashSet<int> selected,
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed,
            int targetCount)
        {
            HashSet<int> cornerScratch = new HashSet<int>();
            AddCornerIndices(cornerScratch, assignments, seed, targetCount);
            List<int> cornerIndices = new List<int>(cornerScratch);
            SortIndicesBySeed(cornerIndices, seed);

            HashSet<int> centerScratch = new HashSet<int>();
            AddNearestToCenter(centerScratch, assignments, seed + 3, targetCount);
            List<int> centerIndices = new List<int>(centerScratch);
            SortIndicesBySeed(centerIndices, seed + 5);

            int cornerIndex = 0;
            int centerIndex = 0;
            bool pickCorner = PositiveMod(seed, 2) == 0;
            while (selected.Count < targetCount && (cornerIndex < cornerIndices.Count || centerIndex < centerIndices.Count))
            {
                if (pickCorner && cornerIndex < cornerIndices.Count)
                {
                    selected.Add(cornerIndices[cornerIndex]);
                    cornerIndex++;
                }
                else if (centerIndex < centerIndices.Count)
                {
                    selected.Add(centerIndices[centerIndex]);
                    centerIndex++;
                }
                else if (cornerIndex < cornerIndices.Count)
                {
                    selected.Add(cornerIndices[cornerIndex]);
                    cornerIndex++;
                }

                pickCorner = !pickCorner;
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

            List<int> fallbackIndices = CollectLayerZeroIndices(assignments);
            SortIndicesBySeed(fallbackIndices, seed + 97);
            for (int i = 0; i < fallbackIndices.Count && selected.Count < targetCount; i++)
            {
                selected.Add(fallbackIndices[i]);
            }

            if (selected.Count >= targetCount)
            {
                return;
            }

            List<int> anyLayerIndices = new List<int>();
            for (int index = 0; index < assignments.Count; index++)
            {
                anyLayerIndices.Add(index);
            }

            SortIndicesBySeed(anyLayerIndices, seed + 193);
            for (int i = 0; i < anyLayerIndices.Count && selected.Count < targetCount; i++)
            {
                selected.Add(anyLayerIndices[i]);
            }
        }

        private static int SelectCenterAnchorIndex(
            IReadOnlyList<TileSymbolAssignment> assignments,
            int seed)
        {
            List<int> layerZeroIndices = CollectLayerZeroIndices(assignments);
            if (layerZeroIndices.Count == 0)
            {
                return -1;
            }

            float centerColumn = (BoardGridDefinition.ColumnCount - 1) / 2f;
            float centerRow = (BoardGridDefinition.RowCount - 1) / 2f;
            int bestIndex = layerZeroIndices[0];
            float bestScore = float.MaxValue;
            for (int i = 0; i < layerZeroIndices.Count; i++)
            {
                int index = layerZeroIndices[i];
                TileSymbolAssignment assignment = assignments[index];
                float score = DistanceSquared(
                    assignment.Position.GridCoordinate.Column,
                    assignment.Position.GridCoordinate.Row,
                    centerColumn,
                    centerRow)
                    + (PositiveMod(seed + index, 13) * 0.001f);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestIndex = index;
                }
            }

            return bestIndex;
        }

        private static int FindNearestIndexOnLayerZero(
            IReadOnlyList<TileSymbolAssignment> assignments,
            int targetColumn,
            int targetRow,
            int layerIndex,
            int seed,
            HashSet<int> excluded)
        {
            int bestIndex = -1;
            float bestScore = float.MaxValue;
            for (int index = 0; index < assignments.Count; index++)
            {
                if (excluded.Contains(index))
                {
                    continue;
                }

                TileSymbolAssignment assignment = assignments[index];
                if (assignment.Position.LayerIndex != layerIndex)
                {
                    continue;
                }

                float score = DistanceSquared(
                    assignment.Position.GridCoordinate.Column,
                    assignment.Position.GridCoordinate.Row,
                    targetColumn,
                    targetRow)
                    + (PositiveMod(seed + index, 17) * 0.001f);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestIndex = index;
                }
            }

            return bestScore <= 2.5f ? bestIndex : -1;
        }

        private static List<int> CollectLayerZeroIndices(IReadOnlyList<TileSymbolAssignment> assignments)
        {
            List<int> indices = new List<int>();
            for (int index = 0; index < assignments.Count; index++)
            {
                if (assignments[index].Position.LayerIndex == 0)
                {
                    indices.Add(index);
                }
            }

            return indices;
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
            indices.Sort((left, right) =>
            {
                int leftHash = PositiveMod((left * 73856093) ^ (seed * 19349663), 1000003);
                int rightHash = PositiveMod((right * 73856093) ^ (seed * 19349663), 1000003);
                return leftHash.CompareTo(rightHash);
            });
        }

        private static float DistanceSquared(int column, int row, float targetColumn, float targetRow)
        {
            float deltaColumn = column - targetColumn;
            float deltaRow = row - targetRow;
            return (deltaColumn * deltaColumn) + (deltaRow * deltaRow);
        }

        private static long PackCellKey(int column, int row)
        {
            return ((long)column << 32) | (uint)row;
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
