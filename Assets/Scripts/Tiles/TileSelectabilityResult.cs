namespace MahjongGame.Tiles
{
    public readonly struct TileSelectabilityResult
    {
        public bool IsSelectable { get; }

        public TileSelectabilityBlockReason BlockReason { get; }

        private TileSelectabilityResult(bool isSelectable, TileSelectabilityBlockReason blockReason)
        {
            IsSelectable = isSelectable;
            BlockReason = blockReason;
        }

        public static TileSelectabilityResult Selectable()
        {
            return new TileSelectabilityResult(true, TileSelectabilityBlockReason.None);
        }

        public static TileSelectabilityResult Blocked(TileSelectabilityBlockReason blockReason)
        {
            return new TileSelectabilityResult(false, blockReason);
        }
    }
}
