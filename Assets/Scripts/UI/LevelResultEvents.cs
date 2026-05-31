using System;

namespace MahjongGame.UI
{
    public static class LevelResultEvents
    {
        public static event Action<LevelResultSummary> LevelResultReady;

        internal static void RaiseLevelResultReady(LevelResultSummary summary)
        {
            if (summary == null)
            {
                return;
            }

            LevelResultReady?.Invoke(summary);
        }
    }
}
