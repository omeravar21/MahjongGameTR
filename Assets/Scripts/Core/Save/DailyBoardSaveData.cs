using System;

namespace MahjongGame.Core.Save
{
    [Serializable]
    public sealed class DailyBoardSaveData
    {
        public int lastCompletedDayId;

        public void EnsureDefaults()
        {
            if (lastCompletedDayId < 0)
            {
                lastCompletedDayId = 0;
            }
        }
    }
}
