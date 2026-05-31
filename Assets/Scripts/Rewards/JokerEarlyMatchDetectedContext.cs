namespace MahjongGame.Rewards
{
    public sealed class JokerEarlyMatchDetectedContext
    {
        public int JokerTileId { get; }

        public float ElapsedSessionSeconds { get; }

        public JokerEarlyMatchDetectedContext(int jokerTileId, float elapsedSessionSeconds)
        {
            JokerTileId = jokerTileId;
            ElapsedSessionSeconds = elapsedSessionSeconds < 0f ? 0f : elapsedSessionSeconds;
        }
    }
}
