using System;

namespace MahjongGame.Board
{
    [Serializable]
    public readonly struct BoardGridCoordinate : IEquatable<BoardGridCoordinate>
    {
        public int Column { get; }
        public int Row { get; }

        public BoardGridCoordinate(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public bool IsValid => BoardGridDefinition.IsValidCoordinate(Column, Row);

        public int ToFlatIndex()
        {
            return Row * BoardGridDefinition.ColumnCount + Column;
        }

        public static BoardGridCoordinate FromFlatIndex(int flatIndex)
        {
            int column = flatIndex % BoardGridDefinition.ColumnCount;
            int row = flatIndex / BoardGridDefinition.ColumnCount;
            return new BoardGridCoordinate(column, row);
        }

        public static bool TryCreate(int column, int row, out BoardGridCoordinate coordinate)
        {
            coordinate = new BoardGridCoordinate(column, row);
            return coordinate.IsValid;
        }

        public bool Equals(BoardGridCoordinate other)
        {
            return Column == other.Column && Row == other.Row;
        }

        public override bool Equals(object obj)
        {
            return obj is BoardGridCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Column * 397 ^ Row;
        }

        public override string ToString()
        {
            return "(" + Column + ", " + Row + ")";
        }
    }
}
