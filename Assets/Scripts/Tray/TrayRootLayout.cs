using MahjongGame.Board;
using UnityEngine;

namespace MahjongGame.Tray
{
    public static class TrayRootLayout
    {
        public const float TrayLocalY = 4.85f;
        public const float SlotSpacing = 1.25f;
        public const float FrameHorizontalPadding = 0.55f;
        public const float FrameVerticalPadding = 0.25f;
        public const float FrameHeight = BoardGridDefinition.DefaultCellHeight + FrameVerticalPadding * 2f;

        public static Vector3 GetTrayContainerLocalPosition()
        {
            return Vector3.zero;
        }

        public static Vector3 GetSlotLocalPosition(int slotIndex)
        {
            float totalWidth = SlotSpacing * (TrayRootDefinition.SlotCount - 1);
            float startX = -totalWidth * 0.5f;
            return new Vector3(startX + slotIndex * SlotSpacing, TrayLocalY, 0f);
        }

        public static Vector3 GetFrameLocalPosition()
        {
            return new Vector3(0f, TrayLocalY, 0f);
        }

        public static Vector2 GetFrameSize()
        {
            float slotSpan = SlotSpacing * (TrayRootDefinition.SlotCount - 1);
            float width = slotSpan + BoardGridDefinition.DefaultCellWidth + FrameHorizontalPadding * 2f;
            return new Vector2(width, FrameHeight);
        }
    }
}
