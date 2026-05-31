namespace MahjongGame.Tiles
{
    public readonly struct TileInteractionResult
    {
        public bool IsAccepted { get; }

        public TileInteractionBlockReason BlockReason { get; }

        private TileInteractionResult(bool isAccepted, TileInteractionBlockReason blockReason)
        {
            IsAccepted = isAccepted;
            BlockReason = blockReason;
        }

        public static TileInteractionResult Accepted()
        {
            return new TileInteractionResult(true, TileInteractionBlockReason.None);
        }

        public static TileInteractionResult Rejected(TileInteractionBlockReason blockReason)
        {
            return new TileInteractionResult(false, blockReason);
        }
    }
}
