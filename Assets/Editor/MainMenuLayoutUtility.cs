#if UNITY_EDITOR
using MahjongGame.UI;
using UnityEngine;
using UnityEngine.UI;

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

            return DoorPresentationController.HasRequiredDoorPresentation();
        }
    }
}
#endif
