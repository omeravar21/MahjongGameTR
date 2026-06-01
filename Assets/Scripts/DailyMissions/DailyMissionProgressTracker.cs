using MahjongGame.Boosters;
using MahjongGame.Combo;
using MahjongGame.Score;
using MahjongGame.Session;
using MahjongGame.Timer;
using UnityEngine;

namespace MahjongGame.DailyMissions
{
    public sealed class DailyMissionProgressTracker : MonoBehaviour
    {
        private int _boostersUsedThisSession;

        private void OnEnable()
        {
            SessionEvents.SessionStarted += HandleSessionStarted;
            SessionEvents.SessionEnded += HandleSessionEnded;
            ComboEvents.ComboIncreased += HandleComboIncreased;
            BoosterEvents.BoosterUsedInSession += HandleBoosterUsedInSession;
            ScoreEvents.JokerBonusAwarded += HandleJokerBonusAwarded;
        }

        private void OnDisable()
        {
            SessionEvents.SessionStarted -= HandleSessionStarted;
            SessionEvents.SessionEnded -= HandleSessionEnded;
            ComboEvents.ComboIncreased -= HandleComboIncreased;
            BoosterEvents.BoosterUsedInSession -= HandleBoosterUsedInSession;
            ScoreEvents.JokerBonusAwarded -= HandleJokerBonusAwarded;
        }

        private void HandleSessionStarted(SessionStartedContext context)
        {
            _boostersUsedThisSession = 0;
        }

        private void HandleSessionEnded(SessionEndedContext context)
        {
            if (!DailyMissionDirector.HasInstance)
            {
                return;
            }

            TimerController timerController = Object.FindAnyObjectByType<TimerController>();
            float completionTimeSeconds = timerController != null ? timerController.LastElapsedTimeSeconds : 0f;
            float allocatedTimeSeconds = timerController != null ? timerController.AllocatedTimeSeconds : 0f;

            DailyMissionDirector.Instance.TryApplySessionWin(
                context,
                _boostersUsedThisSession,
                completionTimeSeconds,
                allocatedTimeSeconds);
        }

        private void HandleComboIncreased(ComboIncreasedContext context)
        {
            if (context == null || !DailyMissionDirector.HasInstance)
            {
                return;
            }

            DailyMissionDirector.Instance.TryApplyComboIncreased(context.HighestCombo);
        }

        private void HandleBoosterUsedInSession(BoosterType boosterType)
        {
            _boostersUsedThisSession++;
        }

        private void HandleJokerBonusAwarded(JokerBonusAwardedContext context)
        {
            if (context == null || !DailyMissionDirector.HasInstance)
            {
                return;
            }

            DailyMissionDirector.Instance.TryApplyProgress(DailyMissionType.MatchRewardJokers, 1, out _);
        }
    }
}
