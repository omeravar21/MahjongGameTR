using System.Collections.Generic;
using MahjongGame.Board;

namespace MahjongGame.BoardGeneration
{
    public sealed class GridMask
    {
        private readonly GridCellOccupancy[] _cellOccupancy;

        public int LevelNumber { get; }

        public int Seed { get; }

        public int ColumnCount => BoardGridDefinition.ColumnCount;

        public int RowCount => BoardGridDefinition.RowCount;

        public int TotalCellCount => BoardGridDefinition.TotalCellCount;

        public int ActiveCellCount { get; }

        public GridMask(int levelNumber, int seed, GridCellOccupancy[] cellOccupancy)
        {
            LevelNumber = levelNumber;
            Seed = seed;
            _cellOccupancy = cellOccupancy ?? new GridCellOccupancy[BoardGridDefinition.TotalCellCount];

            int activeCount = 0;
            for (int index = 0; index < _cellOccupancy.Length; index++)
            {
                if (_cellOccupancy[index] == GridCellOccupancy.Eligible)
                {
                    activeCount++;
                }
            }

            ActiveCellCount = activeCount;
        }

        public GridCellOccupancy GetCellOccupancy(int column, int row)
        {
            if (!BoardGridDefinition.IsValidCoordinate(column, row))
            {
                return GridCellOccupancy.Disabled;
            }

            return _cellOccupancy[ToFlatIndex(column, row)];
        }

        public GridCellOccupancy GetCellOccupancy(BoardGridCoordinate coordinate)
        {
            return GetCellOccupancy(coordinate.Column, coordinate.Row);
        }

        public bool IsCellEligible(int column, int row)
        {
            return GetCellOccupancy(column, row) == GridCellOccupancy.Eligible;
        }

        public bool IsCellEligible(BoardGridCoordinate coordinate)
        {
            return IsCellEligible(coordinate.Column, coordinate.Row);
        }

        public IReadOnlyList<BoardGridCoordinate> GetEligibleCoordinates()
        {
            List<BoardGridCoordinate> coordinates = new List<BoardGridCoordinate>(ActiveCellCount);

            for (int row = 0; row < RowCount; row++)
            {
                for (int column = 0; column < ColumnCount; column++)
                {
                    if (IsCellEligible(column, row))
                    {
                        coordinates.Add(new BoardGridCoordinate(column, row));
                    }
                }
            }

            return coordinates;
        }

        private static int ToFlatIndex(int column, int row)
        {
            return row * BoardGridDefinition.ColumnCount + column;
        }
    }
}
