using System;

namespace MahjongGame.Core
{
    public static class GameEvents
    {
        public static event Action<AppGameState> GameStateChanged;
        public static event Action<string> SceneLoadStarted;
        public static event Action<string> SceneLoadCompleted;

        internal static void RaiseGameStateChanged(AppGameState state)
        {
            GameStateChanged?.Invoke(state);
        }

        internal static void RaiseSceneLoadStarted(string sceneName)
        {
            SceneLoadStarted?.Invoke(sceneName);
        }

        internal static void RaiseSceneLoadCompleted(string sceneName)
        {
            SceneLoadCompleted?.Invoke(sceneName);
        }
    }
}