namespace MahjongGame.Tiles
{
    public enum TileSelectabilityBlockReason
    {
        None = 0,
        InvalidState = 1,
        BlockedByUpperTile = 2,
        BlockedByBothSides = 3
    }
}
