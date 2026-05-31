using MahjongGame.Timer;
using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Session
{
    public sealed class LoseConditionController : MonoBehaviour
    {
        private void OnEnable()
        {
            TrayEvents.TrayCapacityOverflowDetected += HandleTrayCapacityOverflowDetected;
            TimerEvents.TimerExpired += HandleTimerExpired;
        }

        private void OnDisable()
        {
            TrayEvents.TrayCapacityOverflowDetected -= HandleTrayCapacityOverflowDetected;
            TimerEvents.TimerExpired -= HandleTimerExpired;
        }

        private void HandleTrayCapacityOverflowDetected(TrayCapacityOverflowContext context)
        {
            if (context == null)
            {
                return;
            }

            TryEndSessionForLose("[LoseConditionController] Tray overflow — session ended with Lose.");
        }

        private void HandleTimerExpired(TimerExpiredContext context)
        {
            if (context == null)
            {
                return;
            }

            if (!SessionDirector.HasInstance || !SessionDirector.Instance.IsSessionActive)
            {
                return;
            }

            PenaltyEvents.RaiseTimerExpirationPenaltyDetected(
                new TimerExpirationPenaltyContext(context.AllocatedTimeSeconds, context.LevelNumber));

            TryEndSessionForLose("[LoseConditionController] Timer expired — session ended with Lose.");
        }

        private void TryEndSessionForLose(string successLogMessage)
        {
            if (!SessionDirector.HasInstance || !SessionDirector.Instance.IsSessionActive)
            {
                return;
            }

            if (SessionDirector.Instance.TryEndSession(SessionEndReason.Lose))
            {
                Debug.Log(successLogMessage);
            }
        }
    }
}
