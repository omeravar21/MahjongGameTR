using MahjongGame.Tiles;

namespace MahjongGame.BoardGeneration
{
    public readonly struct TileSymbolAssignment
    {
        public int TileId { get; }

        public TileBoardPosition Position { get; }

        public int SymbolId { get; }

        public bool IsClosed { get; }

        public TileSymbolAssignment(int tileId, TileBoardPosition position, int symbolId, bool isClosed = false)
        {
            TileId = tileId;
            Position = position;
            SymbolId = symbolId;
            IsClosed = isClosed;
        }

        public TileSymbolAssignment WithClosed(bool isClosed)
        {
            return new TileSymbolAssignment(TileId, Position, SymbolId, isClosed);
        }
    }
}
