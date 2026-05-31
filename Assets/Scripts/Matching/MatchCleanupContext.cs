namespace MahjongGame.Matching
{
    public sealed class MatchCleanupContext
    {
        public MatchRequest Request { get; }

        public MatchCleanupContext(MatchRequest request)
        {
            Request = request;
        }
    }
}
