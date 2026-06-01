using MahjongGame.DailyBoard;
using MahjongGame.DailyRewards;
using MahjongGame.Session;
using UnityEngine;

namespace MahjongGame.DailyBoard
{
    public sealed class DailyBoardCompletionController : MonoBehaviour
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
            if (context == null
                || context.Reason != SessionEndReason.Win
                || context.Session == null
                || context.Session.Mode != SessionMode.DailyBoard)
            {
                return;
            }

            if (!DailyBoardDirector.HasInstance)
            {
                Debug.LogWarning("[DailyBoardCompletionController] DailyBoardDirector is not available.");
                return;
            }

            if (DailyRewardDirector.HasInstance)
            {
                DailyRewardDirector.Instance.TryGrantDailyBoardCompletionRewards();
            }
            else
            {
                Debug.LogWarning("[DailyBoardCompletionController] DailyRewardDirector is not available.");
            }

            if (DailyBoardDirector.Instance.TryMarkCompletedToday())
            {
                Debug.Log(
                    "[DailyBoardCompletionController] Daily board completed for day "
                    + context.Session.DailyDayId
                    + ".");
            }
        }
    }
}
