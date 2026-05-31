namespace MahjongGame.Tiles
{
    public enum TileInteractionBlockReason
    {
        None = 0,
        InvalidTile = 1,
        InvalidState = 2,
        BlockedByUpperTile = 3,
        BlockedByBothSides = 4,
        NoTraySlotAvailable = 5,
        AlreadyMoving = 6,
        MissingSceneWiring = 7
    }
}
