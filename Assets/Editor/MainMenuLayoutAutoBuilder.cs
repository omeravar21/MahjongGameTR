#if UNITY_EDITOR
using UnityEditor;

namespace MahjongGame.Editor
{
    [InitializeOnLoad]
    public static class MainMenuLayoutAutoBuilder
    {
        private const string BuildSessionKey = "MahjongGame.Phase24LayoutBuilt";

        static MainMenuLayoutAutoBuilder()
        {
            EditorApplication.delayCall += TryBuildOnce;
        }

        private static void TryBuildOnce()
        {
            if (SessionState.GetBool(BuildSessionKey, false))
            {
                return;
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if (MainMenuLayoutUtility.HasRequiredLayout())
            {
                SessionState.SetBool(BuildSessionKey, true);
                return;
            }

            MainMenuLayoutBuilder.BuildMainMenuLayout();
            SessionState.SetBool(BuildSessionKey, true);
        }
    }
}
#endif