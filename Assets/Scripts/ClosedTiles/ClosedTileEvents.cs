using System;

namespace MahjongGame.ClosedTiles
{
    public static class ClosedTileEvents
    {
        public static event Action<ClosedTileRegisteredContext> ClosedTileRegistered;

        public static event Action<ClosedTileStateChangedContext> ClosedTileStateChanged;

        public static event Action ClosedTileRuntimeReset;

        internal static void RaiseClosedTileRegistered(ClosedTileRegisteredContext context)
        {
            if (context == null)
            {
                return;
            }

            ClosedTileRegistered?.Invoke(context);
        }

        internal static void RaiseClosedTileStateChanged(ClosedTileStateChangedContext context)
        {
            if (context == null)
            {
                return;
            }

            ClosedTileStateChanged?.Invoke(context);
        }

        internal static void RaiseClosedTileRuntimeReset()
        {
            ClosedTileRuntimeReset?.Invoke();
        }
    }
}
