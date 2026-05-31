using MahjongGame.Combo;
using MahjongGame.Matching;
using MahjongGame.Session;
using UnityEngine;

namespace MahjongGame.Score
{
    public sealed class ScoreController : MonoBehaviour
    {
        private int _currentScore;
        private int _matchScoreTotal;
        private int _comboScoreTotal;
        private int _matchCount;

        public int CurrentScore => _currentScore;

        public int MatchScoreTotal => _matchScoreTotal;

        public int ComboScoreTotal => _comboScoreTotal;

        public int MatchCount => _matchCount;

        private void OnEnable()
        {
            SessionEvents.SessionStarted += HandleSessionStarted;
            MatchEvents.MatchCleanedUp += HandleMatchCleanedUp;
            ComboEvents.ComboIncreased += HandleComboIncreased;
        }

        private void OnDisable()
        {
            SessionEvents.SessionStarted -= HandleSessionStarted;
            MatchEvents.MatchCleanedUp -= HandleMatchCleanedUp;
            ComboEvents.ComboIncreased -= HandleComboIncreased;
        }

        internal void AwardComboBonusForValidation(int comboLevel)
        {
            AwardComboBonus(comboLevel);
        }

        private void HandleComboIncreased(ComboIncreasedContext context)
        {
            if (context == null)
            {
                return;
            }

            if (!SessionDirector.HasInstance || !SessionDirector.Instance.IsSessionActive)
            {
                return;
            }

            AwardComboBonus(context.ComboLevel);
        }

        private void AwardComboBonus(int comboLevel)
        {
            int comboBonus = ScoreDefinition.ResolveComboBonus(comboLevel);
            if (comboBonus <= 0)
            {
                return;
            }

            _comboScoreTotal += comboBonus;
            ApplyScoreDelta(comboBonus);
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
            _comboScoreTotal = 0;
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
