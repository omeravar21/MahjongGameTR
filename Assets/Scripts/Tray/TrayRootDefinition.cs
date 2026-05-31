namespace MahjongGame.Tray
{
    public static class TrayRootDefinition
    {
        public const string TrayRootName = "TrayRoot";
        public const string TrayContainerName = "TrayContainer";
        public const string FrameRootName = "TrayFrame";
        public const string FrameBackgroundName = "TrayFrameBackground";
        public const string FrameTrimName = "TrayFrameTrim";

        public const int SlotCount = 4;
        public const int Capacity = 4;

        public static string GetSlotName(int slotIndex)
        {
            return "TraySlot_" + slotIndex;
        }

        public static bool IsValidSlotIndex(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < SlotCount;
        }
    }
}
