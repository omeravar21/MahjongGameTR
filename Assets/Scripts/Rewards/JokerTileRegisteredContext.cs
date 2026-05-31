namespace MahjongGame.Rewards
{
    public sealed class JokerTileRegisteredContext
    {
        public JokerTileData JokerTileData { get; }

        public JokerTileRegisteredContext(JokerTileData jokerTileData)
        {
            JokerTileData = jokerTileData;
        }
    }
}
