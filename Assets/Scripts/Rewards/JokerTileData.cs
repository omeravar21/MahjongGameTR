using MahjongGame.Tiles;

namespace MahjongGame.Rewards
{
    public sealed class JokerTileData
    {
        public int TileId { get; }

        public int SymbolId { get; }

        public TileBoardPosition BoardPosition { get; }

        public JokerTileState State { get; }

        public JokerTileData(
            int tileId,
            int symbolId,
            TileBoardPosition boardPosition,
            JokerTileState state)
        {
            TileId = tileId;
            SymbolId = symbolId;
            BoardPosition = boardPosition;
            State = state;
        }

        public static bool TryCreate(Tile tile, out JokerTileData jokerTileData)
        {
            jokerTileData = null;

            if (tile == null || tile.Data == null)
            {
                return false;
            }

            if (!tile.IsJoker || tile.TileId < 0 || !tile.OriginalBoardPosition.IsValid)
            {
                return false;
            }

            jokerTileData = new JokerTileData(
                tile.TileId,
                tile.SymbolId,
                tile.OriginalBoardPosition,
                JokerTileState.Registered);

            return true;
        }

        public static bool TryCreate(
            int tileId,
            int symbolId,
            TileBoardPosition boardPosition,
            out JokerTileData jokerTileData)
        {
            jokerTileData = null;

            if (tileId < 0 || !boardPosition.IsValid)
            {
                return false;
            }

            jokerTileData = new JokerTileData(
                tileId,
                symbolId,
                boardPosition,
                JokerTileState.Registered);

            return true;
        }

        public JokerTileData WithState(JokerTileState newState)
        {
            return new JokerTileData(TileId, SymbolId, BoardPosition, newState);
        }
    }
}
