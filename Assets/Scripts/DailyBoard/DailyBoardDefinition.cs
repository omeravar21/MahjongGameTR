using System;

namespace MahjongGame.DailyBoard
{
    /// <summary>
    /// Static daily board rules. UTC calendar day drives global identity so all players share the same daily seed.
    /// </summary>
    public static class DailyBoardDefinition
    {
        public static int GetUtcDayId(DateTime utcNow)
        {
            DateTime utcDate = utcNow.Kind == DateTimeKind.Utc
                ? utcNow.Date
                : utcNow.ToUniversalTime().Date;

            return (utcDate.Year * 10000) + (utcDate.Month * 100) + utcDate.Day;
        }

        public static int GetCurrentUtcDayId()
        {
            return GetUtcDayId(DateTime.UtcNow);
        }

        public static int ComputeSeed(int dayId)
        {
            if (dayId <= 0)
            {
                return 0;
            }

            unchecked
            {
                int hash = (dayId * 92837111) ^ 689287499;
                hash = (hash * 486187739) ^ 19349663;
                return hash == int.MinValue ? int.MaxValue : Math.Abs(hash);
            }
        }

        public static bool IsAvailable(int dayId, int lastCompletedDayId)
        {
            if (dayId <= 0)
            {
                return false;
            }

            return lastCompletedDayId != dayId;
        }

        public static bool IsCompletedToday(int dayId, int lastCompletedDayId)
        {
            return dayId > 0 && lastCompletedDayId == dayId;
        }

        public static DailyBoardIdentity BuildIdentity(int dayId, int lastCompletedDayId)
        {
            int dailySeed = ComputeSeed(dayId);
            bool isCompletedToday = IsCompletedToday(dayId, lastCompletedDayId);
            bool isAvailable = IsAvailable(dayId, lastCompletedDayId);

            return new DailyBoardIdentity(dayId, dailySeed, isAvailable, isCompletedToday);
        }
    }
}
