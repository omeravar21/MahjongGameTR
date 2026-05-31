using System;
using MahjongGame.Board;

namespace MahjongGame.Tiles
{
    [Serializable]
    public sealed class TileData
    {
        public const int UnassignedSymbolId = -1;

        public int TileId;
        public BoardGridCoordinate GridCoordinate;
        public int LayerIndex;
        public TileType Type;
        public bool IsClosed;
        public bool IsJoker;
        public int SymbolId;
        public TileBoardPosition OriginalBoardPosition;

        public TileData(
            int tileId,
            BoardGridCoordinate gridCoordinate,
            int layerIndex,
            TileType type,
            bool isClosed = false,
            bool isJoker = false,
            int symbolId = UnassignedSymbolId)
        {
            TileId = tileId;
            GridCoordinate = gridCoordinate;
            LayerIndex = layerIndex;
            Type = type;
            IsClosed = isClosed;
            IsJoker = isJoker;
            SymbolId = symbolId;
            OriginalBoardPosition = new TileBoardPosition(gridCoordinate, layerIndex);
        }

        public TileIdentity Identity => new TileIdentity(TileId);

        public bool HasValidIdentity => Identity.IsValid;

        public bool HasAssignedSymbol => SymbolId >= 0;

        public bool IsRewardJoker => IsJoker;

        public bool IsClosedTile => IsClosed || Type == TileType.Closed;
    }
}
