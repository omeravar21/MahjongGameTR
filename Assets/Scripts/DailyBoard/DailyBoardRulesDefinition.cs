using MahjongGame.BoardGeneration;

namespace MahjongGame.DailyBoard
{
    /// <summary>
    /// Daily board uses a fixed global difficulty tier and UTC day boundaries (see DailyBoardDefinition).
    /// </summary>
    public static class DailyBoardRulesDefinition
    {
        public const int ReferenceDifficultyLevel = 100;

        public const bool AllowReplayAfterCompletion = false;

        public static float GetRecommendedTimerSeconds()
        {
            return DifficultyDefinition.ResolveProfile(ReferenceDifficultyLevel).RecommendedTimerSeconds;
        }
    }
}
