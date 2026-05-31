namespace MahjongGame.ClosedTiles
{
    public sealed class ClosedTileRegisteredContext
    {
        public ClosedTileData ClosedTileData { get; }

        public ClosedTileRegisteredContext(ClosedTileData closedTileData)
        {
            ClosedTileData = closedTileData;
        }
    }
}
