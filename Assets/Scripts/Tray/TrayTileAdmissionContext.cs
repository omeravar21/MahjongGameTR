using MahjongGame.Tiles;

namespace MahjongGame.Tray
{
    public sealed class TrayTileAdmissionContext
    {
        public Tile Tile { get; }

        public int SlotIndex { get; }

        public int ReservedCountAfterAdmission { get; }

        public TrayTileAdmissionContext(Tile tile, int slotIndex, int reservedCountAfterAdmission)
        {
            Tile = tile;
            SlotIndex = slotIndex;
            ReservedCountAfterAdmission = reservedCountAfterAdmission;
        }
    }
}
