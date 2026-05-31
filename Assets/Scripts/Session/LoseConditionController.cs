using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Session
{
    public sealed class LoseConditionController : MonoBehaviour
    {
        private void OnEnable()
        {
            TrayEvents.TrayCapacityOverflowDetected += HandleTrayCapacityOverflowDetected;
        }

        private void OnDisable()
        {
            TrayEvents.TrayCapacityOverflowDetected -= HandleTrayCapacityOverflowDetected;
        }

        private void HandleTrayCapacityOverflowDetected(TrayCapacityOverflowContext context)
        {
            if (context == null)
            {
                return;
            }

            if (!SessionDirector.HasInstance || !SessionDirector.Instance.IsSessionActive)
            {
                return;
            }

            if (SessionDirector.Instance.TryEndSession(SessionEndReason.Lose))
            {
                Debug.Log("[LoseConditionController] Tray overflow — session ended with Lose.");
            }
        }
    }
}
