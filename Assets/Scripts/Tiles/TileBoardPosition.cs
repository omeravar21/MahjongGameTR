using System;
using MahjongGame.Board;

namespace MahjongGame.Tiles
{
    public readonly struct TileBoardPosition : IEquatable<TileBoardPosition>
    {
        public BoardGridCoordinate GridCoordinate { get; }

        public int LayerIndex { get; }

        public TileBoardPosition(BoardGridCoordinate gridCoordinate, int layerIndex)
        {
            GridCoordinate = gridCoordinate;
            LayerIndex = layerIndex;
        }

        public bool IsValid => GridCoordinate.IsValid && BoardLayerDefinition.IsValidLayerIndex(LayerIndex);

        public bool Equals(TileBoardPosition other)
        {
            return GridCoordinate.Equals(other.GridCoordinate) && LayerIndex == other.LayerIndex;
        }

        public override bool Equals(object obj)
        {
            return obj is TileBoardPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return GridCoordinate.GetHashCode() * 397 ^ LayerIndex;
        }

        public override string ToString()
        {
            return GridCoordinate + " layer " + LayerIndex;
        }
    }
}
