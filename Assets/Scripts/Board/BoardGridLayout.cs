using UnityEngine;

namespace MahjongGame.Board
{
    public static class BoardGridLayout
    {
        public static Vector3 GetCellLocalPosition(BoardGridCoordinate coordinate, float cellWidth, float cellHeight)
        {
            return GetCellLocalPosition(coordinate.Column, coordinate.Row, cellWidth, cellHeight);
        }

        public static Vector3 GetCellLocalPosition(int column, int row, float cellWidth, float cellHeight)
        {
            float x = (column - (BoardGridDefinition.ColumnCount - 1) * 0.5f) * cellWidth;
            float y = ((BoardGridDefinition.RowCount - 1) * 0.5f - row) * cellHeight;
            return new Vector3(x, y, 0f);
        }
    }
}
