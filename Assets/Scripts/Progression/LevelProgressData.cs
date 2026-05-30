using System;

namespace MahjongGame.Progression
{
    [Serializable]
    public sealed class LevelProgressData
    {
        public const int MinLevel = 1;
        public const int MaxLevel = 9999;

        public int levelNumber = MinLevel;
        public bool isCompleted;
        public int completionCount;

        public LevelProgressData()
        {
        }

        public LevelProgressData(int levelNumber)
        {
            this.levelNumber = ClampLevel(levelNumber);
        }

        public static int ClampLevel(int levelNumber)
        {
            if (levelNumber < MinLevel)
            {
                return MinLevel;
            }

            if (levelNumber > MaxLevel)
            {
                return MaxLevel;
            }

            return levelNumber;
        }
    }
}