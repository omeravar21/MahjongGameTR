#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MahjongGame.Editor
{
    public static class MainMenuLayoutUtility
    {
        private static readonly string[] RequiredObjects =
        {
            "ProfileButton",
            "ThemeButton",
            "SettingsButton",
            "LevelButton"
        };

        public static bool HasRequiredLayout()
        {
            GameObject canvas = GameObject.Find("Canvas_MainMenu");
            if (canvas == null || canvas.GetComponent<Canvas>() == null)
            {
                return false;
            }

            foreach (string objectName in RequiredObjects)
            {
                Transform child = canvas.transform.Find(objectName);
                if (child == null || child.GetComponent<Button>() == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
#endif