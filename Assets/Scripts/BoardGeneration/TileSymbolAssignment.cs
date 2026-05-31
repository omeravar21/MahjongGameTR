using MahjongGame.Tiles;

namespace MahjongGame.BoardGeneration
{
    public readonly struct TileSymbolAssignment
    {
        public int TileId { get; }

        public TileBoardPosition Position { get; }

        public int SymbolId { get; }

        public bool IsClosed { get; }

        public bool IsJoker { get; }

        public TileSymbolAssignment(int tileId, TileBoardPosition position, int symbolId, bool isClosed = false, bool isJoker = false)
        {
            TileId = tileId;
            Position = position;
            SymbolId = symbolId;
            IsClosed = isClosed;
            IsJoker = isJoker;
        }

        public TileSymbolAssignment WithClosed(bool isClosed)
        {
            return new TileSymbolAssignment(TileId, Position, SymbolId, isClosed, IsJoker);
        }

        public TileSymbolAssignment WithJoker(bool isJoker)
        {
            return new TileSymbolAssignment(TileId, Position, SymbolId, IsClosed, isJoker);
        }
    }
}
