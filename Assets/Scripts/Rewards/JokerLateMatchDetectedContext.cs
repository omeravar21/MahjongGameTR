namespace MahjongGame.Rewards
{
    public sealed class JokerLateMatchDetectedContext
    {
        public int JokerTileId { get; }

        public float ElapsedSessionSeconds { get; }

        public JokerLateMatchDetectedContext(int jokerTileId, float elapsedSessionSeconds)
        {
            JokerTileId = jokerTileId;
            ElapsedSessionSeconds = elapsedSessionSeconds < 0f ? 0f : elapsedSessionSeconds;
        }
    }
}
