namespace MahjongGame.Rewards
{
    public sealed class JokerTileClearedContext
    {
        public int TileId { get; }

        public JokerTileClearedContext(int tileId)
        {
            TileId = tileId;
        }
    }
}
