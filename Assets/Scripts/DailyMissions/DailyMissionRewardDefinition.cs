namespace MahjongGame.DailyMissions
{
    public static class DailyMissionRewardDefinition
    {
        public const int EasyGlobalPerformanceScore = 500;
        public const int MediumGlobalPerformanceScore = 1000;
        public const int HardGlobalPerformanceScore = 1500;

        public const int EasyShuffleBoosterReward = 0;
        public const int EasyUndoBoosterReward = 0;
        public const int EasyHintBoosterReward = 1;

        public const int MediumShuffleBoosterReward = 1;
        public const int MediumUndoBoosterReward = 1;
        public const int MediumHintBoosterReward = 1;

        public const int HardShuffleBoosterReward = 2;
        public const int HardUndoBoosterReward = 2;
        public const int HardHintBoosterReward = 2;

        public static int GetGlobalPerformanceScore(DailyMissionTier tier)
        {
            return tier switch
            {
                DailyMissionTier.Easy => EasyGlobalPerformanceScore,
                DailyMissionTier.Medium => MediumGlobalPerformanceScore,
                DailyMissionTier.Hard => HardGlobalPerformanceScore,
                _ => 0
            };
        }

        public static int GetShuffleReward(DailyMissionTier tier)
        {
            return tier switch
            {
                DailyMissionTier.Easy => EasyShuffleBoosterReward,
                DailyMissionTier.Medium => MediumShuffleBoosterReward,
                DailyMissionTier.Hard => HardShuffleBoosterReward,
                _ => 0
            };
        }

        public static int GetUndoReward(DailyMissionTier tier)
        {
            return tier switch
            {
                DailyMissionTier.Easy => EasyUndoBoosterReward,
                DailyMissionTier.Medium => MediumUndoBoosterReward,
                DailyMissionTier.Hard => HardUndoBoosterReward,
                _ => 0
            };
        }

        public static int GetHintReward(DailyMissionTier tier)
        {
            return tier switch
            {
                DailyMissionTier.Easy => EasyHintBoosterReward,
                DailyMissionTier.Medium => MediumHintBoosterReward,
                DailyMissionTier.Hard => HardHintBoosterReward,
                _ => 0
            };
        }
    }
}
