namespace MahjongGame.Matching
{
    public sealed class MatchExecutionContext
    {
        public MatchRequest Request { get; }

        public MatchExecutionContext(MatchRequest request)
        {
            Request = request;
        }
    }
}
