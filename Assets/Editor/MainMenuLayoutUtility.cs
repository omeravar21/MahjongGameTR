#if UNITY_EDITOR
using MahjongGame.UI;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class MainMenuLayoutUtility
    {
        public static bool HasRequiredLayout()
        {
            if (!MainMenuLayoutController.HasRequiredLayout())
            {
                return false;
            }

            if (!DoorPresentationController.HasRequiredDoorPresentation())
            {
                return false;
            }

            return MainMenuNavigationController.HasRequiredNavigation();
        }
    }
}
#endif
