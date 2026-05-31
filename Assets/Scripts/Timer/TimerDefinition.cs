using MahjongGame.Progression;
using UnityEngine;

namespace MahjongGame.Timer
{
    public static class TimerDefinition
    {
        public const float BaseDurationSeconds = 120f;
        public const float PerLevelBonusSeconds = 5f;

        private static bool _loggedStubWarning;

        public static float ResolveDurationSeconds(int levelNumber)
        {
            int clampedLevel = LevelProgressData.ClampLevel(levelNumber);
            float duration = BaseDurationSeconds + (clampedLevel - LevelProgressData.MinLevel) * PerLevelBonusSeconds;

            if (!_loggedStubWarning)
            {
                Debug.LogWarning("[TimerDefinition] Using stub duration until DifficultyDirector is available.");
                _loggedStubWarning = true;
            }

            return duration;
        }
    }
}
