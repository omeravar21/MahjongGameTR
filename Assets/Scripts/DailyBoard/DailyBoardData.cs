using MahjongGame.Core.Save;

namespace MahjongGame.DailyBoard
{
    public sealed class DailyBoardData
    {
        public int LastCompletedDayId { get; private set; }

        public static DailyBoardData CreateDefault()
        {
            return new DailyBoardData();
        }

        public static DailyBoardData FromSave(PlayerSaveData saveData)
        {
            DailyBoardData dailyBoardData = new DailyBoardData();

            if (saveData == null)
            {
                return dailyBoardData;
            }

            dailyBoardData.ApplyFromSave(saveData);
            return dailyBoardData;
        }

        public void ApplyFromSave(PlayerSaveData saveData)
        {
            if (saveData?.dailyBoard == null)
            {
                LastCompletedDayId = 0;
                return;
            }

            saveData.dailyBoard.EnsureDefaults();
            LastCompletedDayId = saveData.dailyBoard.lastCompletedDayId < 0
                ? 0
                : saveData.dailyBoard.lastCompletedDayId;
        }

        public void WriteToSave(PlayerSaveData saveData)
        {
            if (saveData == null)
            {
                return;
            }

            saveData.dailyBoard ??= new DailyBoardSaveData();
            saveData.dailyBoard.EnsureDefaults();
            saveData.dailyBoard.lastCompletedDayId = LastCompletedDayId < 0 ? 0 : LastCompletedDayId;
        }

        public void SetLastCompletedDayId(int dayId)
        {
            LastCompletedDayId = dayId < 0 ? 0 : dayId;
        }
    }
}
