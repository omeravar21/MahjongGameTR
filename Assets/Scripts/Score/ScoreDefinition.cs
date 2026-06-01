namespace MahjongGame.Score
{
    public static class ScoreDefinition
    {
        public const int BaseMatchScore = 1000;

        public const int JokerEarlyMatchBonus = 2500;

        public const int TimePerformanceBonusWithin40Percent = 25000;

        public const int TimePerformanceBonusWithin60Percent = 15000;

        public const int TimePerformanceBonusWithin80Percent = 8000;

        public const int TimePerformanceBonusWithin100Percent = 3000;

        public const int PerfectClearBonus = 10000;

        public const int NoBoosterBonus = 5000;

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

        public static int ResolveTimePerformanceBonus(float elapsedSeconds, float allocatedSeconds)
        {
            if (allocatedSeconds <= 0f || elapsedSeconds < 0f)
            {
                return 0;
            }

            float completionRatio = elapsedSeconds / allocatedSeconds;

            if (completionRatio <= 0.4f)
            {
                return TimePerformanceBonusWithin40Percent;
            }

            if (completionRatio <= 0.6f)
            {
                return TimePerformanceBonusWithin60Percent;
            }

            if (completionRatio <= 0.8f)
            {
                return TimePerformanceBonusWithin80Percent;
            }

            if (completionRatio <= 1f)
            {
                return TimePerformanceBonusWithin100Percent;
            }

            return 0;
        }

        public static int ResolvePerfectClearBonus(bool isPerfectClear)
        {
            return isPerfectClear ? PerfectClearBonus : 0;
        }

        public static int ResolveNoBoosterBonus(bool usedBoosterInSession)
        {
            return usedBoosterInSession ? 0 : NoBoosterBonus;
        }
    }
}
