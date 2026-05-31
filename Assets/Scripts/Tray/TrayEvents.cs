using System;

namespace MahjongGame.Tray
{
    public static class TrayEvents
    {
        public static event Action<TrayTileAdmissionContext> TrayTileAdmissionStarted;

        public static event Action<TrayTileStoredContext> TrayTileStored;

        public static event Action<TrayCapacityOverflowContext> TrayCapacityOverflowDetected;

        internal static void RaiseTrayTileAdmissionStarted(TrayTileAdmissionContext context)
        {
            if (context == null)
            {
                return;
            }

            TrayTileAdmissionStarted?.Invoke(context);
        }

        internal static void RaiseTrayTileStored(TrayTileStoredContext context)
        {
            if (context == null)
            {
                return;
            }

            TrayTileStored?.Invoke(context);
        }

        internal static void RaiseTrayCapacityOverflowDetected(TrayCapacityOverflowContext context)
        {
            if (context == null)
            {
                return;
            }

            TrayCapacityOverflowDetected?.Invoke(context);
        }
    }
}
