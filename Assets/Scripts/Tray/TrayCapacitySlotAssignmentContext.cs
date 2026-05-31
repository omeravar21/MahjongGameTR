using MahjongGame.Tiles;

namespace MahjongGame.Tray
{
    public sealed class TrayCapacitySlotAssignmentContext
    {
        public Tile Tile { get; }

        public int SlotIndex { get; }

        public int ReservedCountAfterAssignment { get; }

        public TrayCapacitySlotAssignmentContext(Tile tile, int slotIndex, int reservedCountAfterAssignment)
        {
            Tile = tile;
            SlotIndex = slotIndex;
            ReservedCountAfterAssignment = reservedCountAfterAssignment;
        }
    }
}
