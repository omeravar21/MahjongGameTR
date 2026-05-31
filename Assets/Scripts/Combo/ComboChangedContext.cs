namespace MahjongGame.Combo
{
    public sealed class ComboChangedContext
    {
        public int PreviousCombo { get; }

        public int CurrentCombo { get; }

        public ComboChangedContext(int previousCombo, int currentCombo)
        {
            PreviousCombo = previousCombo;
            CurrentCombo = currentCombo;
        }
    }
}
