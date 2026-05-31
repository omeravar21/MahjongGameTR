namespace MahjongGame.Score
{
    public static class ScoreDefinition
    {
        public const int BaseMatchScore = 1000;

        public static int ResolveComboBonus(int comboLevel)
        {
            switch (comboLevel)
            {
                case 2:
                    return 200;
                case 3:
                    return 400;
                case 4:
                    return 600;
                case 5:
                    return 800;
                default:
                    return comboLevel >= 6 ? 1200 : 0;
            }
        }
    }
}
