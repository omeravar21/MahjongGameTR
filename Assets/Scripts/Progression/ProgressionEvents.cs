using System;

namespace MahjongGame.Progression
{
    public sealed class LevelCompletedContext
    {
        public int CompletedLevel { get; }

        public LevelCompletedContext(int completedLevel)
        {
            CompletedLevel = LevelProgressData.ClampLevel(completedLevel);
        }
    }

    public sealed class LevelAdvancedContext
    {
        public int PreviousLevel { get; }

        public int NewLevel { get; }

        public LevelAdvancedContext(int previousLevel, int newLevel)
        {
            PreviousLevel = LevelProgressData.ClampLevel(previousLevel);
            NewLevel = LevelProgressData.ClampLevel(newLevel);
        }
    }

    public static class ProgressionEvents
    {
        public static event Action<LevelCompletedContext> LevelCompleted;

        public static event Action<LevelAdvancedContext> LevelAdvanced;

        internal static void RaiseLevelCompleted(LevelCompletedContext context)
        {
            if (context == null)
            {
                return;
            }

            LevelCompleted?.Invoke(context);
        }

        internal static void RaiseLevelAdvanced(LevelAdvancedContext context)
        {
            if (context == null)
            {
                return;
            }

            LevelAdvanced?.Invoke(context);
        }
    }
}
