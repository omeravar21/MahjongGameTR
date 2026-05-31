using UnityEngine;

namespace MahjongGame.Tray
{
    public static class TrayMovementLayout
    {
        public const float MovementDurationSeconds = 0.30f;

        public static int SlotCount => TrayRootDefinition.SlotCount;

        public static string TrayContainerName => TrayRootDefinition.TrayContainerName;

        public static float TrayLocalY => TrayRootLayout.TrayLocalY;

        public static float SlotSpacing => TrayRootLayout.SlotSpacing;

        public static string GetSlotName(int slotIndex)
        {
            return TrayRootDefinition.GetSlotName(slotIndex);
        }

        public static Vector3 GetSlotLocalPosition(int slotIndex)
        {
            return TrayRootLayout.GetSlotLocalPosition(slotIndex);
        }
    }
}
