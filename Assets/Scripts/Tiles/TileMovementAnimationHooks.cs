using System;

namespace MahjongGame.Tiles
{
    public static class TileMovementAnimationHooks
    {
        public static event Func<TileMovementRequest, Action, bool> TryPlayCustomMovement;

        internal static bool TryInvokeCustomMovement(TileMovementRequest request, Action onComplete)
        {
            if (request == null || onComplete == null)
            {
                return false;
            }

            Func<TileMovementRequest, Action, bool> handler = TryPlayCustomMovement;
            if (handler == null)
            {
                return false;
            }

            return handler.Invoke(request, onComplete);
        }
    }
}
