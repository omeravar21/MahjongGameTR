using MahjongGame.DailyBoard;
using MahjongGame.Ranking;
using MahjongGame.Score;
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

            int rewardScore = DailyBoardRewardDefinition.GetCompletionGlobalPerformanceScore();
            LevelPerformanceResult performanceResult = new LevelPerformanceResult(
                0,
                0,
                rewardScore,
                0);

            if (RankingDirector.HasInstance)
            {
                RankingDirector.Instance.TryAccumulateLevelPerformance(performanceResult, out _, out _);
            }

            if (DailyBoardDirector.Instance.TryMarkCompletedToday())
            {
                Debug.Log(
                    "[DailyBoardCompletionController] Daily board completed for day "
                    + context.Session.DailyDayId
                    + " with GPS reward "
                    + rewardScore
                    + ".");
            }
        }
    }
}
