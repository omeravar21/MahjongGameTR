namespace MahjongGame.Board
{
    public static class BoardGridDefinition
    {
        public const int ColumnCount = 6;
        public const int RowCount = 7;
        public const int TotalCellCount = ColumnCount * RowCount;

        public const float DefaultCellWidth = 1.2f;
        public const float DefaultCellHeight = 1.2f;

        public static bool IsValidColumn(int column)
        {
            return column >= 0 && column < ColumnCount;
        }

        public static bool IsValidRow(int row)
        {
            return row >= 0 && row < RowCount;
        }

        public static bool IsValidCoordinate(int column, int row)
        {
            return IsValidColumn(column) && IsValidRow(row);
        }

        public static string GetCellName(int column, int row)
        {
            return "GridCell_" + column + "_" + row;
        }

        public static string GridRootName => "BoardGridRoot";
    }
}
