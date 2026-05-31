namespace MahjongGame.Combo
{
    public sealed class ComboIncreasedContext
    {
        public int ComboLevel { get; }

        public int HighestCombo { get; }

        public ComboIncreasedContext(int comboLevel, int highestCombo)
        {
            ComboLevel = comboLevel;
            HighestCombo = highestCombo;
        }
    }
}
