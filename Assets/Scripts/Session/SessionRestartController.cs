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

            if (context.Session != null && context.Session.Mode == SessionMode.DailyBoard)
            {
                LevelRuntimeResetter.TryResetRuntimeState(transform);

                if (SessionDirector.Instance.TryStartDailySession(out _))
                {
                    Debug.Log("[SessionRestartController] Daily board restarted after failure.");
                }

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
