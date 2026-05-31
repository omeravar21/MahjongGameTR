using System;
using MahjongGame.Board;

namespace MahjongGame.BoardGeneration
{
    public static class ArchetypePatternDefinition
    {
        public const int MinimumActiveCellCount = 18;

        public static bool[] GetPattern(BoardArchetypeId archetypeId)
        {
            switch (archetypeId)
            {
                case BoardArchetypeId.Diamond:
                    return CreateDiamondPattern();
                case BoardArchetypeId.Oval:
                    return CreateOvalPattern();
                case BoardArchetypeId.Pyramid:
                    return CreatePyramidPattern();
                case BoardArchetypeId.Tower:
                    return CreateTowerPattern();
                case BoardArchetypeId.Cross:
                    return CreateCrossPattern();
                case BoardArchetypeId.Bridge:
                    return CreateBridgePattern();
                case BoardArchetypeId.Island:
                    return CreateIslandPattern();
                case BoardArchetypeId.Maze:
                    return CreateMazePattern();
                default:
                    throw new ArgumentOutOfRangeException(nameof(archetypeId), archetypeId, null);
            }
        }

        public static int CountActiveCells(bool[] pattern)
        {
            if (pattern == null || pattern.Length != BoardGridDefinition.TotalCellCount)
            {
                return 0;
            }

            int count = 0;
            for (int index = 0; index < pattern.Length; index++)
            {
                if (pattern[index])
                {
                    count++;
                }
            }

            return count;
        }

        private static bool[] CreateDiamondPattern()
        {
            return FromRows(
                "..X...",
                ".XXX..",
                "XXXXX.",
                ".XXXXX",
                "..XXX.",
                "...X..",
                "......");
        }

        private static bool[] CreateOvalPattern()
        {
            return FromRows(
                "......",
                ".XXXX.",
                "XXXXXX",
                "XXXXXX",
                "XXXXXX",
                ".XXXX.",
                "......");
        }

        private static bool[] CreatePyramidPattern()
        {
            return FromRows(
                "..X...",
                ".XXX..",
                "XXXXX.",
                "XXXXXX",
                "XXXXXX",
                "XXXXXX",
                "XXXXXX");
        }

        private static bool[] CreateTowerPattern()
        {
            return FromRows(
                "..XX..",
                "..XX..",
                ".XXXX.",
                ".XXXX.",
                "..XX..",
                "..XX..",
                ".XXXX.");
        }

        private static bool[] CreateCrossPattern()
        {
            return FromRows(
                "..X...",
                ".XXX..",
                "XXXXX.",
                "XXXXX.",
                "XXXXX.",
                ".XXX..",
                "..X...");
        }

        private static bool[] CreateBridgePattern()
        {
            return FromRows(
                "X....X",
                "X....X",
                "XXXXXX",
                "XXXXXX",
                "X....X",
                "X....X",
                "......");
        }

        private static bool[] CreateIslandPattern()
        {
            return FromRows(
                "......",
                ".XXXX.",
                "XXXXXX",
                "XXXXXX",
                "XXXXXX",
                ".XXXX.",
                "......");
        }

        private static bool[] CreateMazePattern()
        {
            return FromRows(
                "XX..XX",
                "X....X",
                "XXXXXX",
                "..XX..",
                "XXXXXX",
                "X....X",
                "XX..XX");
        }

        private static bool[] FromRows(params string[] rows)
        {
            if (rows.Length != BoardGridDefinition.RowCount)
            {
                throw new InvalidOperationException("Archetype pattern rows must match base grid row count.");
            }

            bool[] pattern = new bool[BoardGridDefinition.TotalCellCount];

            for (int row = 0; row < rows.Length; row++)
            {
                string rowPattern = rows[row];
                if (rowPattern.Length != BoardGridDefinition.ColumnCount)
                {
                    throw new InvalidOperationException("Archetype pattern columns must match base grid column count.");
                }

                for (int column = 0; column < rowPattern.Length; column++)
                {
                    pattern[(row * BoardGridDefinition.ColumnCount) + column] = rowPattern[column] == 'X';
                }
            }

            return pattern;
        }
    }
}
