using System;

namespace MahjongGame.Tiles
{
    public static class TileInteractionEvents
    {
        public static event Action<Tile, TileInteractionResult> TileInteractionAccepted;

        public static event Action<Tile, TileInteractionResult> TileInteractionRejected;

        internal static void RaiseTileInteractionAccepted(Tile tile, TileInteractionResult result)
        {
            if (tile == null)
            {
                return;
            }

            TileInteractionAccepted?.Invoke(tile, result);
        }

        internal static void RaiseTileInteractionRejected(Tile tile, TileInteractionResult result)
        {
            if (tile == null)
            {
                return;
            }

            TileInteractionRejected?.Invoke(tile, result);
        }
    }
}
