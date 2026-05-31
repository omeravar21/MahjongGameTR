using System;

namespace MahjongGame.Tray
{
    public static class TrayCapacityEvents
    {
        public static event Action<TrayCapacityOverflowContext> TrayCapacityOverflowDetected;

        public static event Action<TrayCapacitySlotAssignmentContext> TraySlotAssigned;

        internal static void RaiseTrayCapacityOverflowDetected(TrayCapacityOverflowContext context)
        {
            if (context == null)
            {
                return;
            }

            TrayCapacityOverflowDetected?.Invoke(context);
        }

        internal static void RaiseTraySlotAssigned(TrayCapacitySlotAssignmentContext context)
        {
            if (context == null)
            {
                return;
            }

            TraySlotAssigned?.Invoke(context);
        }
    }
}
