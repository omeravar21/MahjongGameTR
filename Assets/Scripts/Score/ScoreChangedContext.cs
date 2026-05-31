namespace MahjongGame.Score
{
    public sealed class ScoreChangedContext
    {
        public int PreviousScore { get; }

        public int NewScore { get; }

        public int Delta { get; }

        public ScoreChangedContext(int previousScore, int newScore)
        {
            PreviousScore = previousScore;
            NewScore = newScore;
            Delta = newScore - previousScore;
        }
    }
}
