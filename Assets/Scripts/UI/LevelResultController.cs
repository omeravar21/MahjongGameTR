using MahjongGame.Combo;
using MahjongGame.Progression;
using MahjongGame.Score;
using MahjongGame.Session;
using MahjongGame.Timer;
using UnityEngine;

namespace MahjongGame.UI
{
    public sealed class LevelResultController : MonoBehaviour
    {
        private void OnEnable()
        {
            SessionEvents.SessionEnded += HandleSessionEnded;
        }

        private void OnDisable()
        {
            SessionEvents.SessionEnded -= HandleSessionEnded;
        }

        internal LevelResultSummary BuildSummaryForValidation()
        {
            return BuildSummaryFromCurrentState(ResolveLevelNumber());
        }

        private void HandleSessionEnded(SessionEndedContext context)
        {
            if (context == null || context.Reason != SessionEndReason.Win)
            {
                return;
            }

            int levelNumber = context.Session != null
                ? context.Session.LevelNumber
                : ResolveLevelNumber();

            LevelResultSummary summary = BuildSummaryFromCurrentState(levelNumber);
            LevelResultEvents.RaiseLevelResultReady(summary);
        }

        private LevelResultSummary BuildSummaryFromCurrentState(int levelNumber)
        {
            ScoreController scoreController = GetComponent<ScoreController>();
            ComboController comboController = GetComponent<ComboController>();
            TimerController timerController = ResolveTimerController();

            int score = scoreController != null ? scoreController.CurrentScore : 0;
            int totalComboCount = comboController != null ? comboController.TotalComboCount : 0;
            float completionTimeSeconds = timerController != null ? timerController.LastElapsedTimeSeconds : 0f;

            return new LevelResultSummary(levelNumber, completionTimeSeconds, score, totalComboCount);
        }

        private TimerController ResolveTimerController()
        {
            Transform timerRoot = transform.Find("TimerRoot");
            return timerRoot != null ? timerRoot.GetComponent<TimerController>() : null;
        }

        private static int ResolveLevelNumber()
        {
            if (SessionDirector.HasInstance && SessionDirector.Instance.CurrentSession != null)
            {
                return SessionDirector.Instance.CurrentSession.LevelNumber;
            }

            if (PlayerProgressionDirector.HasInstance)
            {
                return PlayerProgressionDirector.Instance.CurrentLevel;
            }

            return LevelProgressData.MinLevel;
        }
    }
}
