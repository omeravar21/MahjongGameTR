using MahjongGame.Progression;
using UnityEngine;
using UnityEngine.UI;

namespace MahjongGame.UI
{
    [DefaultExecutionOrder(20)]
    public sealed class CurrentLevelButtonController : MonoBehaviour
    {
        private void Start()
        {
            ApplyCurrentLevelLabel();
        }

        public static string FormatLevelLabel(int level)
        {
            return $"LEVEL {LevelProgressData.ClampLevel(level)}";
        }

        private void ApplyCurrentLevelLabel()
        {
            Text label = FindLevelButtonLabel();
            if (label == null)
            {
                Debug.LogWarning("[CurrentLevelButtonController] LevelButton/Label was not found.");
                return;
            }

            int level = ResolveCurrentLevel();
            label.text = FormatLevelLabel(level);
        }

        private static int ResolveCurrentLevel()
        {
            if (!PlayerProgressionDirector.HasInstance)
            {
                return LevelProgressData.MinLevel;
            }

            return PlayerProgressionDirector.Instance.CurrentLevel;
        }

        private static Text FindLevelButtonLabel()
        {
            Transform canvasTransform = MainMenuLayoutController.GetCanvasTransform();
            if (canvasTransform == null)
            {
                return null;
            }

            Transform labelTransform = canvasTransform.Find("LevelButton/Label");
            return labelTransform != null ? labelTransform.GetComponent<Text>() : null;
        }
    }
}
