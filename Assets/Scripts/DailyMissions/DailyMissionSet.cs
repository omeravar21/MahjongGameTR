namespace MahjongGame.DailyMissions
{
    public sealed class DailyMissionSet
    {
        public const int SlotCount = 5;

        public static readonly DailyMissionSet Empty = new DailyMissionSet(0, 0, new DailyMissionEntry[0]);

        public int DayId { get; }

        public int MissionSeed { get; }

        public DailyMissionEntry[] Entries { get; }

        public DailyMissionSet(int dayId, int missionSeed, DailyMissionEntry[] entries)
        {
            DayId = dayId < 0 ? 0 : dayId;
            MissionSeed = missionSeed;
            Entries = entries ?? new DailyMissionEntry[0];
        }

        public bool IsValid()
        {
            return DayId > 0 && MissionSeed > 0 && Entries.Length == SlotCount;
        }
    }
}
