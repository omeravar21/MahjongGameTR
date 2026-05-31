using System;
using MahjongGame.Board;

namespace MahjongGame.BoardGeneration
{
    public static class HolePatternDefinition
    {
        public static bool[] ApplyHolePattern(bool[] variationPattern, HolePatternId holePatternId, int seed)
        {
            if (variationPattern == null || variationPattern.Length != BoardGridDefinition.TotalCellCount)
            {
                return CreateEmptyPattern();
            }

            bool[] holedPattern = ApplyHoleRule(CopyPattern(variationPattern), holePatternId, seed);
            if (ArchetypePatternDefinition.CountActiveCells(holedPattern)
                < ArchetypePatternDefinition.MinimumActiveCellCount)
            {
                return CopyPattern(variationPattern);
            }

            return holedPattern;
        }

        public static HolePatternId ClampHolePatternId(HolePatternId holePatternId)
        {
            int rawValue = (int)holePatternId;
            if (rawValue < 0)
            {
                return HolePatternId.SingleCenter;
            }

            if (rawValue >= VisualVarietyDefinition.LaunchHolePatternCount)
            {
                return (HolePatternId)(VisualVarietyDefinition.LaunchHolePatternCount - 1);
            }

            return holePatternId;
        }

        private static bool[] ApplyHoleRule(bool[] pattern, HolePatternId holePatternId, int seed)
        {
            switch (ClampHolePatternId(holePatternId))
            {
                case HolePatternId.SingleCenter:
                    return ApplySingleCenter(pattern);
                case HolePatternId.DualCorner:
                    return ApplyDualCorner(pattern);
                case HolePatternId.CrossChannel:
                    return ApplyCrossChannel(pattern, seed);
                case HolePatternId.SideNotch:
                    return ApplySideNotch(pattern, seed);
                case HolePatternId.RingGap:
                    return ApplyRingGap(pattern);
                case HolePatternId.SplitSegment:
                    return ApplySplitSegment(pattern, seed);
                default:
                    return pattern;
            }
        }

        private static bool[] ApplySingleCenter(bool[] pattern)
        {
            if (!TryGetBoundingBox(pattern, out int minColumn, out int maxColumn, out int minRow, out int maxRow))
            {
                return pattern;
            }

            float centerColumn = (minColumn + maxColumn) / 2f;
            float centerRow = (minRow + maxRow) / 2f;
            RemoveClosestActiveCell(pattern, centerColumn, centerRow);
            return pattern;
        }

        private static bool[] ApplyDualCorner(bool[] pattern)
        {
            if (!TryGetBoundingBox(pattern, out int minColumn, out int maxColumn, out int minRow, out int maxRow))
            {
                return pattern;
            }

            RemoveClosestActiveCell(pattern, minColumn, minRow);
            RemoveClosestActiveCell(pattern, maxColumn, maxRow);
            return pattern;
        }

        private static bool[] ApplyCrossChannel(bool[] pattern, int seed)
        {
            if (!TryGetBoundingBox(pattern, out int minColumn, out int maxColumn, out int minRow, out int maxRow))
            {
                return pattern;
            }

            int centerColumn = (minColumn + maxColumn) / 2;
            int centerRow = (minRow + maxRow) / 2;
            bool verticalChannel = PositiveMod(seed, 2) == 0;

            if (verticalChannel)
            {
                for (int row = minRow; row <= maxRow; row++)
                {
                    int index = ToFlatIndex(centerColumn, row);
                    if (pattern[index])
                    {
                        pattern[index] = false;
                    }
                }
            }
            else
            {
                for (int column = minColumn; column <= maxColumn; column++)
                {
                    int index = ToFlatIndex(column, centerRow);
                    if (pattern[index])
                    {
                        pattern[index] = false;
                    }
                }
            }

            return pattern;
        }

        private static bool[] ApplySideNotch(bool[] pattern, int seed)
        {
            if (!TryGetBoundingBox(pattern, out int minColumn, out int maxColumn, out int minRow, out int maxRow))
            {
                return pattern;
            }

            int side = PositiveMod(seed, 4);
            switch (side)
            {
                case 0:
                    RemoveActiveCellsOnColumn(pattern, minColumn);
                    break;
                case 1:
                    RemoveActiveCellsOnColumn(pattern, maxColumn);
                    break;
                case 2:
                    RemoveActiveCellsOnRow(pattern, minRow);
                    break;
                default:
                    RemoveActiveCellsOnRow(pattern, maxRow);
                    break;
            }

            return pattern;
        }

        private static bool[] ApplyRingGap(bool[] pattern)
        {
            bool[] result = CopyPattern(pattern);

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    int index = ToFlatIndex(column, row);
                    if (!pattern[index] || !IsInteriorCell(pattern, column, row))
                    {
                        continue;
                    }

                    result[index] = false;
                }
            }

            return result;
        }

        private static bool[] ApplySplitSegment(bool[] pattern, int seed)
        {
            if (!TryGetBoundingBox(pattern, out int minColumn, out int maxColumn, out int minRow, out int maxRow))
            {
                return pattern;
            }

            float centerColumn = (minColumn + maxColumn) / 2f;
            float centerRow = (minRow + maxRow) / 2f;

            int bestIndex = -1;
            float bestScore = float.MaxValue;
            int bestNeighborCount = int.MaxValue;

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    int index = ToFlatIndex(column, row);
                    if (!pattern[index])
                    {
                        continue;
                    }

                    int neighborCount = CountActiveNeighbors(pattern, column, row);
                    if (neighborCount != 2)
                    {
                        continue;
                    }

                    float distance = Math.Abs(column - centerColumn) + Math.Abs(row - centerRow);
                    float score = distance + (PositiveMod(seed + index, 7) * 0.01f);
                    if (neighborCount < bestNeighborCount || (neighborCount == bestNeighborCount && score < bestScore))
                    {
                        bestNeighborCount = neighborCount;
                        bestScore = score;
                        bestIndex = index;
                    }
                }
            }

            if (bestIndex >= 0)
            {
                pattern[bestIndex] = false;
            }

            return pattern;
        }

        private static void RemoveClosestActiveCell(bool[] pattern, float targetColumn, float targetRow)
        {
            int bestIndex = -1;
            float bestDistance = float.MaxValue;

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    int index = ToFlatIndex(column, row);
                    if (!pattern[index])
                    {
                        continue;
                    }

                    float distance = Math.Abs(column - targetColumn) + Math.Abs(row - targetRow);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestIndex = index;
                    }
                }
            }

            if (bestIndex >= 0)
            {
                pattern[bestIndex] = false;
            }
        }

        private static void RemoveActiveCellsOnColumn(bool[] pattern, int column)
        {
            if (!BoardGridDefinition.IsValidColumn(column))
            {
                return;
            }

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                int index = ToFlatIndex(column, row);
                if (pattern[index])
                {
                    pattern[index] = false;
                }
            }
        }

        private static void RemoveActiveCellsOnRow(bool[] pattern, int row)
        {
            if (!BoardGridDefinition.IsValidRow(row))
            {
                return;
            }

            for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
            {
                int index = ToFlatIndex(column, row);
                if (pattern[index])
                {
                    pattern[index] = false;
                }
            }
        }

        private static bool TryGetBoundingBox(
            bool[] pattern,
            out int minColumn,
            out int maxColumn,
            out int minRow,
            out int maxRow)
        {
            minColumn = int.MaxValue;
            maxColumn = int.MinValue;
            minRow = int.MaxValue;
            maxRow = int.MinValue;

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    if (!pattern[ToFlatIndex(column, row)])
                    {
                        continue;
                    }

                    minColumn = column < minColumn ? column : minColumn;
                    maxColumn = column > maxColumn ? column : maxColumn;
                    minRow = row < minRow ? row : minRow;
                    maxRow = row > maxRow ? row : maxRow;
                }
            }

            return minColumn != int.MaxValue;
        }

        private static bool IsInteriorCell(bool[] pattern, int column, int row)
        {
            return HasActiveNeighbor(pattern, column - 1, row)
                && HasActiveNeighbor(pattern, column + 1, row)
                && HasActiveNeighbor(pattern, column, row - 1)
                && HasActiveNeighbor(pattern, column, row + 1);
        }

        private static int CountActiveNeighbors(bool[] pattern, int column, int row)
        {
            int count = 0;
            if (HasActiveNeighbor(pattern, column - 1, row))
            {
                count++;
            }

            if (HasActiveNeighbor(pattern, column + 1, row))
            {
                count++;
            }

            if (HasActiveNeighbor(pattern, column, row - 1))
            {
                count++;
            }

            if (HasActiveNeighbor(pattern, column, row + 1))
            {
                count++;
            }

            return count;
        }

        private static bool HasActiveNeighbor(bool[] pattern, int column, int row)
        {
            if (!BoardGridDefinition.IsValidCoordinate(column, row))
            {
                return false;
            }

            return pattern[ToFlatIndex(column, row)];
        }

        private static bool[] CopyPattern(bool[] sourcePattern)
        {
            bool[] copy = new bool[BoardGridDefinition.TotalCellCount];
            for (int index = 0; index < copy.Length; index++)
            {
                copy[index] = sourcePattern[index];
            }

            return copy;
        }

        private static bool[] CreateEmptyPattern()
        {
            return new bool[BoardGridDefinition.TotalCellCount];
        }

        private static int ToFlatIndex(int column, int row)
        {
            return (row * BoardGridDefinition.ColumnCount) + column;
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
