using MahjongGame.Session;
using UnityEngine;

namespace MahjongGame.Progression
{
    public sealed class LevelCompletionController : MonoBehaviour
    {
        private void OnEnable()
        {
            SessionEvents.SessionEnded += HandleSessionEnded;
        }

        private void OnDisable()
        {
            SessionEvents.SessionEnded -= HandleSessionEnded;
        }

        private void HandleSessionEnded(SessionEndedContext context)
        {
            if (context == null || context.Reason != SessionEndReason.Win)
            {
                return;
            }

            if (context.Session != null && context.Session.Mode == SessionMode.DailyBoard)
            {
                return;
            }

            if (!PlayerProgressionDirector.HasInstance)
            {
                Debug.LogWarning("[LevelCompletionController] PlayerProgressionDirector is not available.");
                return;
            }

            PlayerProgressionDirector progressionDirector = PlayerProgressionDirector.Instance;
            if (context.Session != null
                && context.Session.LevelNumber != progressionDirector.CurrentLevel)
            {
                Debug.LogWarning(
                    "[LevelCompletionController] Session level "
                    + context.Session.LevelNumber
                    + " does not match current progression level "
                    + progressionDirector.CurrentLevel
                    + ".");
            }

            if (progressionDirector.TryCompleteCurrentLevel(out LevelProgressionResult result)
                && result.Success)
            {
                Debug.Log(
                    "[LevelCompletionController] Level "
                    + result.PreviousLevel
                    + " marked complete.");
            }
        }
    }
}
