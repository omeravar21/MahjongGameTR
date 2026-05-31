using MahjongGame.Tiles;

namespace MahjongGame.BoardGeneration
{
    public readonly struct TileSymbolAssignment
    {
        public int TileId { get; }

        public TileBoardPosition Position { get; }

        public int SymbolId { get; }

        public TileSymbolAssignment(int tileId, TileBoardPosition position, int symbolId)
        {
            TileId = tileId;
            Position = position;
            SymbolId = symbolId;
        }
    }
}
