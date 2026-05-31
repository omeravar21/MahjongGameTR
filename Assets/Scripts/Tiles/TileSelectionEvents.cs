using System;

namespace MahjongGame.Tiles
{
    public static class TileSelectionEvents
    {
        public static event Action<TileSelectionRequest> TileSelectionRequested;

        internal static void RaiseTileSelectionRequested(TileSelectionRequest request)
        {
            if (request == null)
            {
                return;
            }

            TileSelectionRequested?.Invoke(request);
        }
    }
}
