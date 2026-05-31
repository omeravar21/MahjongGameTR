using MahjongGame.Matching;
using MahjongGame.Session;
using UnityEngine;

namespace MahjongGame.Score
{
    public sealed class ScoreController : MonoBehaviour
    {
        private int _currentScore;
        private int _matchScoreTotal;
        private int _matchCount;

        public int CurrentScore => _currentScore;

        public int MatchScoreTotal => _matchScoreTotal;

        public int MatchCount => _matchCount;

        private void OnEnable()
        {
            SessionEvents.SessionStarted += HandleSessionStarted;
            MatchEvents.MatchCleanedUp += HandleMatchCleanedUp;
        }

        private void OnDisable()
        {
            SessionEvents.SessionStarted -= HandleSessionStarted;
            MatchEvents.MatchCleanedUp -= HandleMatchCleanedUp;
        }

        private void HandleSessionStarted(SessionStartedContext context)
        {
            ResetScoreState();
        }

        private void HandleMatchCleanedUp(MatchCleanupContext context)
        {
            if (context == null)
            {
                return;
            }

            if (!SessionDirector.HasInstance || !SessionDirector.Instance.IsSessionActive)
            {
                return;
            }

            AwardBaseMatchScore();
        }

        internal void AwardBaseMatchScoreForValidation()
        {
            AwardBaseMatchScore();
        }

        private void AwardBaseMatchScore()
        {
            int pointsAwarded = ScoreDefinition.BaseMatchScore;
            _matchCount++;
            _matchScoreTotal += pointsAwarded;
            ApplyScoreDelta(pointsAwarded);
            ScoreEvents.RaiseMatchScoreAwarded(new MatchScoreAwardedContext(
                pointsAwarded,
                _matchScoreTotal,
                _matchCount));
        }

        private void ResetScoreState()
        {
            int previousScore = _currentScore;
            _currentScore = 0;
            _matchScoreTotal = 0;
            _matchCount = 0;

            if (previousScore != 0)
            {
                ScoreEvents.RaiseScoreChanged(new ScoreChangedContext(previousScore, _currentScore));
            }
        }

        private void ApplyScoreDelta(int delta)
        {
            if (delta == 0)
            {
                return;
            }

            int previousScore = _currentScore;
            _currentScore += delta;
            ScoreEvents.RaiseScoreChanged(new ScoreChangedContext(previousScore, _currentScore));
        }
    }
}
