using MahjongGame.Tiles;

namespace MahjongGame.ClosedTiles
{
    public sealed class ClosedTileData
    {
        public int TileId { get; }

        public int SymbolId { get; }

        public TileBoardPosition BoardPosition { get; }

        public ClosedTileState State { get; }

        public ClosedTileData(
            int tileId,
            int symbolId,
            TileBoardPosition boardPosition,
            ClosedTileState state)
        {
            TileId = tileId;
            SymbolId = symbolId;
            BoardPosition = boardPosition;
            State = state;
        }

        public static bool TryCreate(Tile tile, out ClosedTileData closedTileData)
        {
            closedTileData = null;

            if (tile == null || tile.Data == null)
            {
                return false;
            }

            if (tile.TileId < 0 || !tile.OriginalBoardPosition.IsValid)
            {
                return false;
            }

            closedTileData = new ClosedTileData(
                tile.TileId,
                tile.SymbolId,
                tile.OriginalBoardPosition,
                ClosedTileState.Closed);

            return true;
        }

        public static bool TryCreate(
            int tileId,
            int symbolId,
            TileBoardPosition boardPosition,
            out ClosedTileData closedTileData)
        {
            closedTileData = null;

            if (tileId < 0 || !boardPosition.IsValid)
            {
                return false;
            }

            closedTileData = new ClosedTileData(
                tileId,
                symbolId,
                boardPosition,
                ClosedTileState.Closed);

            return true;
        }

        public ClosedTileData WithState(ClosedTileState newState)
        {
            return new ClosedTileData(TileId, SymbolId, BoardPosition, newState);
        }
    }
}
