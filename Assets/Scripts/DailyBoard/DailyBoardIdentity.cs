namespace MahjongGame.DailyBoard
{
    public sealed class DailyBoardIdentity
    {
        public static readonly DailyBoardIdentity Empty = new DailyBoardIdentity(0, 0, false, false);

        public int DayId { get; }

        public int DailySeed { get; }

        public bool IsAvailable { get; }

        public bool IsCompletedToday { get; }

        public DailyBoardIdentity(int dayId, int dailySeed, bool isAvailable, bool isCompletedToday)
        {
            DayId = dayId < 0 ? 0 : dayId;
            DailySeed = dailySeed;
            IsAvailable = isAvailable;
            IsCompletedToday = isCompletedToday;
        }

        public bool IsValid()
        {
            return DayId > 0 && DailySeed > 0;
        }
    }
}
