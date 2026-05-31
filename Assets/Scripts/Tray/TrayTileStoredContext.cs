using MahjongGame.Tiles;

namespace MahjongGame.Tray
{
    public sealed class TrayTileStoredContext
    {
        public Tile Tile { get; }

        public int SlotIndex { get; }

        public int StoredTileCount { get; }

        public TrayTileStoredContext(Tile tile, int slotIndex, int storedTileCount)
        {
            Tile = tile;
            SlotIndex = slotIndex;
            StoredTileCount = storedTileCount;
        }
    }
}
