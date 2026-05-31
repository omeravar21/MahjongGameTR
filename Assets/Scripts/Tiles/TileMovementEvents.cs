using System;

namespace MahjongGame.Tiles
{
    public static class TileMovementEvents
    {
        public static event Action<TileMovementRequest> TileMovementStarted;

        public static event Action<TileMovementRequest> TileMovementCompleted;

        internal static void RaiseTileMovementStarted(TileMovementRequest request)
        {
            if (request == null)
            {
                return;
            }

            TileMovementStarted?.Invoke(request);
        }

        internal static void RaiseTileMovementCompleted(TileMovementRequest request)
        {
            if (request == null)
            {
                return;
            }

            TileMovementCompleted?.Invoke(request);
        }
    }
}
