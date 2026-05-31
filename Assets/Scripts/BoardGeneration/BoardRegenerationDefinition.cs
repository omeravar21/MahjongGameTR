namespace MahjongGame.BoardGeneration
{
    public static class BoardRegenerationDefinition
    {
        public const int SeedStep = 7919;

        public static int ComputeAttemptSeed(int baseSeed, int attemptIndex)
        {
            if (attemptIndex <= 0)
            {
                return baseSeed;
            }

            return baseSeed + (attemptIndex * SeedStep);
        }
    }
}
