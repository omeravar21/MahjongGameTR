using UnityEngine;

namespace MahjongGame.Session
{
    public sealed class SessionRestartController : MonoBehaviour
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
            if (context == null || context.Reason != SessionEndReason.Lose)
            {
                return;
            }

            if (!SessionDirector.HasInstance)
            {
                return;
            }

            LevelRuntimeResetter.TryResetLevel(transform);

            if (SessionDirector.Instance.TryStartSession(out _))
            {
                Debug.Log("[SessionRestartController] Level restarted after failure.");
            }
        }
    }
}
