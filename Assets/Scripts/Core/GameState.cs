namespace MahjongGame.Core
{
    public enum AppGameState
    {
        None = 0,
        Booting = 1,
        MainMenu = 2,
        Loading = 3,
        Gameplay = 4
    }

    public static class GameState
    {
        public static AppGameState Current { get; private set; } = AppGameState.None;

        public static bool IsInitialized => Current != AppGameState.None;

        public static void SetState(AppGameState newState)
        {
            if (Current == newState)
            {
                return;
            }

            Current = newState;
            GameEvents.RaiseGameStateChanged(newState);
        }
    }
}