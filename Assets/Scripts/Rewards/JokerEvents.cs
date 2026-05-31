using System;

namespace MahjongGame.Rewards
{
    public static class JokerEvents
    {
        public static event Action<JokerTileRegisteredContext> JokerTileRegistered;

        public static event Action<JokerTileClearedContext> JokerTileCleared;

        public static event Action JokerRuntimeReset;

        internal static void RaiseJokerTileRegistered(JokerTileRegisteredContext context)
        {
            if (context == null)
            {
                return;
            }

            JokerTileRegistered?.Invoke(context);
        }

        internal static void RaiseJokerTileCleared(JokerTileClearedContext context)
        {
            if (context == null)
            {
                return;
            }

            JokerTileCleared?.Invoke(context);
        }

        internal static void RaiseJokerRuntimeReset()
        {
            JokerRuntimeReset?.Invoke();
        }
    }
}
