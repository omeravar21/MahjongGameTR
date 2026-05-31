using UnityEngine;

namespace MahjongGame.Tray
{
    public static class TrayMovementLayout
    {
        public const int SlotCount = 4;
        public const string TrayContainerName = "TrayContainer";
        public const float TrayLocalY = 4.85f;
        public const float SlotSpacing = 1.25f;
        public const float MovementDurationSeconds = 0.30f;

        public static string GetSlotName(int slotIndex)
        {
            return "TraySlot_" + slotIndex;
        }

        public static Vector3 GetSlotLocalPosition(int slotIndex)
        {
            float totalWidth = SlotSpacing * (SlotCount - 1);
            float startX = -totalWidth * 0.5f;
            return new Vector3(startX + slotIndex * SlotSpacing, TrayLocalY, 0f);
        }
    }
}
