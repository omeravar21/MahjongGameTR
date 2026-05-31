using MahjongGame.Board;

namespace MahjongGame.BoardGeneration
{
    public static class VariationPatternDefinition
    {
        public static bool[] ApplyVariation(bool[] archetypePattern, int variationIndex, int seed)
        {
            if (archetypePattern == null || archetypePattern.Length != BoardGridDefinition.TotalCellCount)
            {
                return CreateEmptyPattern();
            }

            int clampedIndex = ClampVariationIndex(variationIndex);
            if (clampedIndex == 0)
            {
                return CopyPattern(archetypePattern);
            }

            bool[] variedPattern = ApplyVariationRule(archetypePattern, clampedIndex, seed);
            if (ArchetypePatternDefinition.CountActiveCells(variedPattern)
                < ArchetypePatternDefinition.MinimumActiveCellCount)
            {
                return CopyPattern(archetypePattern);
            }

            return variedPattern;
        }

        public static int ClampVariationIndex(int variationIndex)
        {
            if (variationIndex < 0)
            {
                return 0;
            }

            if (variationIndex >= VisualVarietyDefinition.VariationsPerArchetype)
            {
                return VisualVarietyDefinition.VariationsPerArchetype - 1;
            }

            return variationIndex;
        }

        private static bool[] ApplyVariationRule(bool[] archetypePattern, int variationIndex, int seed)
        {
            switch (variationIndex)
            {
                case 1:
                    return ApplyEdgeTrim(archetypePattern);
                case 2:
                    return ApplySparseInterior(archetypePattern);
                case 3:
                    return ApplyAsymmetricTrim(archetypePattern, seed);
                default:
                    return CopyPattern(archetypePattern);
            }
        }

        private static bool[] ApplyEdgeTrim(bool[] archetypePattern)
        {
            bool[] result = CopyPattern(archetypePattern);

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    int index = ToFlatIndex(column, row);
                    if (!archetypePattern[index] || !IsBoundaryCell(archetypePattern, column, row))
                    {
                        continue;
                    }

                    result[index] = false;
                }
            }

            return result;
        }

        private static bool[] ApplySparseInterior(bool[] archetypePattern)
        {
            bool[] result = CopyPattern(archetypePattern);

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    int index = ToFlatIndex(column, row);
                    if (!archetypePattern[index] || !IsInteriorCell(archetypePattern, column, row))
                    {
                        continue;
                    }

                    result[index] = false;
                }
            }

            return result;
        }

        private static bool[] ApplyAsymmetricTrim(bool[] archetypePattern, int seed)
        {
            bool[] result = CopyPattern(archetypePattern);
            int side = PositiveMod(seed, 4);

            switch (side)
            {
                case 0:
                    TrimLeftmostActiveColumn(archetypePattern, result);
                    break;
                case 1:
                    TrimRightmostActiveColumn(archetypePattern, result);
                    break;
                case 2:
                    TrimTopmostActiveRow(archetypePattern, result);
                    break;
                default:
                    TrimBottommostActiveRow(archetypePattern, result);
                    break;
            }

            return result;
        }

        private static void TrimLeftmostActiveColumn(bool[] archetypePattern, bool[] result)
        {
            int targetColumn = int.MaxValue;
            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    if (archetypePattern[ToFlatIndex(column, row)])
                    {
                        targetColumn = column < targetColumn ? column : targetColumn;
                    }
                }
            }

            if (targetColumn == int.MaxValue)
            {
                return;
            }

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                result[ToFlatIndex(targetColumn, row)] = false;
            }
        }

        private static void TrimRightmostActiveColumn(bool[] archetypePattern, bool[] result)
        {
            int targetColumn = int.MinValue;
            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    if (archetypePattern[ToFlatIndex(column, row)])
                    {
                        targetColumn = column > targetColumn ? column : targetColumn;
                    }
                }
            }

            if (targetColumn == int.MinValue)
            {
                return;
            }

            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                result[ToFlatIndex(targetColumn, row)] = false;
            }
        }

        private static void TrimTopmostActiveRow(bool[] archetypePattern, bool[] result)
        {
            int targetRow = int.MaxValue;
            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    if (archetypePattern[ToFlatIndex(column, row)])
                    {
                        targetRow = row < targetRow ? row : targetRow;
                    }
                }
            }

            if (targetRow == int.MaxValue)
            {
                return;
            }

            for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
            {
                result[ToFlatIndex(column, targetRow)] = false;
            }
        }

        private static void TrimBottommostActiveRow(bool[] archetypePattern, bool[] result)
        {
            int targetRow = int.MinValue;
            for (int row = 0; row < BoardGridDefinition.RowCount; row++)
            {
                for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
                {
                    if (archetypePattern[ToFlatIndex(column, row)])
                    {
                        targetRow = row > targetRow ? row : targetRow;
                    }
                }
            }

            if (targetRow == int.MinValue)
            {
                return;
            }

            for (int column = 0; column < BoardGridDefinition.ColumnCount; column++)
            {
                result[ToFlatIndex(column, targetRow)] = false;
            }
        }

        private static bool IsBoundaryCell(bool[] pattern, int column, int row)
        {
            if (!HasActiveNeighbor(pattern, column - 1, row)
                || !HasActiveNeighbor(pattern, column + 1, row)
                || !HasActiveNeighbor(pattern, column, row - 1)
                || !HasActiveNeighbor(pattern, column, row + 1))
            {
                return true;
            }

            return false;
        }

        private static bool IsInteriorCell(bool[] pattern, int column, int row)
        {
            return HasActiveNeighbor(pattern, column - 1, row)
                && HasActiveNeighbor(pattern, column + 1, row)
                && HasActiveNeighbor(pattern, column, row - 1)
                && HasActiveNeighbor(pattern, column, row + 1);
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
