namespace MahjongGame.Ranking
{
    public static class GlobalLeaderboardDefinition
    {
        public const int MaxDisplayedEntries = 50;

        public const string LocalPlayerDisplayName = "Player";

        private static readonly (string DisplayName, long Score)[] ReferenceEntries =
        {
            ("Ayla", 5_000_000),
            ("Mert", 4_750_000),
            ("Elif", 4_500_000),
            ("Deniz", 4_250_000),
            ("Can", 4_000_000),
            ("Selin", 3_750_000),
            ("Burak", 3_500_000),
            ("Zeynep", 3_250_000),
            ("Emre", 3_000_000),
            ("Aylin", 2_750_000),
            ("Kerem", 2_500_000),
            ("Derya", 2_250_000),
            ("Ozan", 2_000_000),
            ("Melis", 1_850_000),
            ("Arda", 1_700_000),
            ("Irem", 1_550_000),
            ("Kaan", 1_400_000),
            ("Seda", 1_250_000),
            ("Tolga", 1_100_000),
            ("Nisa", 950_000),
            ("Baris", 850_000),
            ("Ece", 750_000),
            ("Yusuf", 650_000),
            ("Gizem", 550_000),
            ("Hakan", 450_000),
            ("Pinar", 350_000),
            ("Serkan", 275_000),
            ("Asli", 200_000),
            ("Volkan", 150_000),
            ("Lale", 100_000),
        };

        public static int ReferenceEntryCount => ReferenceEntries.Length;

        public static (string DisplayName, long Score) GetReferenceEntry(int index)
        {
            if (index < 0 || index >= ReferenceEntries.Length)
            {
                return (string.Empty, 0);
            }

            return ReferenceEntries[index];
        }

        public static bool AreReferenceEntriesSortedDescending()
        {
            for (int index = 1; index < ReferenceEntries.Length; index++)
            {
                if (ReferenceEntries[index].Score > ReferenceEntries[index - 1].Score)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
