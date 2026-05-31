using MahjongGame.Board;
using UnityEngine;

namespace MahjongGame.Tray
{
    public static class TraySlotDefinition
    {
        public const string SlotBackgroundName = "SlotBackground";
        public const string SlotTrimName = "SlotTrim";

        public const float SlotWidth = BoardGridDefinition.DefaultCellWidth;
        public const float SlotHeight = BoardGridDefinition.DefaultCellHeight;
        public const float TrimInset = 0.06f;

        public static readonly Color SlotBackgroundColor = new Color(0.28f, 0.22f, 0.18f, 0.85f);
        public static readonly Color SlotTrimColor = new Color(0.62f, 0.52f, 0.38f, 0.75f);

        public const int SlotBackgroundSortingBase = 152;
        public const int SortingOrdersPerSlot = 2;

        public static int GetSlotBackgroundSortingOrder(int slotIndex)
        {
            return SlotBackgroundSortingBase + slotIndex * SortingOrdersPerSlot;
        }

        public static int GetSlotTrimSortingOrder(int slotIndex)
        {
            return GetSlotBackgroundSortingOrder(slotIndex) + 1;
        }
    }
}
