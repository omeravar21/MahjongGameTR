using System;
using MahjongGame.Board;

namespace MahjongGame.Tiles
{
    [Serializable]
    public sealed class TileData
    {
        public int TileId;
        public BoardGridCoordinate GridCoordinate;
        public int LayerIndex;
        public TileType Type;
        public bool IsClosed;
        public bool IsJoker;

        public TileData(
            int tileId,
            BoardGridCoordinate gridCoordinate,
            int layerIndex,
            TileType type,
            bool isClosed = false,
            bool isJoker = false)
        {
            TileId = tileId;
            GridCoordinate = gridCoordinate;
            LayerIndex = layerIndex;
            Type = type;
            IsClosed = isClosed;
            IsJoker = isJoker;
        }
    }
}