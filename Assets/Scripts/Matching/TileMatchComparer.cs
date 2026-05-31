using MahjongGame.Tiles;

namespace MahjongGame.Matching
{
    public static class TileMatchComparer
    {
        public static bool AreMatching(Tile a, Tile b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            if (ReferenceEquals(a, b))
            {
                return false;
            }

            if (!a.HasAssignedSymbol || !b.HasAssignedSymbol)
            {
                return false;
            }

            if (a.State != TileState.InTray || b.State != TileState.InTray)
            {
                return false;
            }

            return a.SymbolId == b.SymbolId;
        }
    }
}
