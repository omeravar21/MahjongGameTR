using MahjongGame.Combo;
using MahjongGame.Matching;
using MahjongGame.Rewards;
using MahjongGame.Session;
using UnityEngine;

namespace MahjongGame.Score
{
    public sealed class ScoreController : MonoBehaviour
    {
        private int _currentScore;
        private int _matchScoreTotal;
        private int _comboScoreTotal;
        private int _jokerBonusTotal;
        private int _matchCount;
        private int _earlyJokerMatchCount;

        public int CurrentScore => _currentScore;

        public int MatchScoreTotal => _matchScoreTotal;

        public int ComboScoreTotal => _comboScoreTotal;

        public int JokerBonusTotal => _jokerBonusTotal;

        public int MatchCount => _matchCount;

        public int EarlyJokerMatchCount => _earlyJokerMatchCount;

        private void OnEnable()
        {
            SessionEvents.SessionStarted += HandleSessionStarted;
            MatchEvents.MatchCleanedUp += HandleMatchCleanedUp;
            ComboEvents.ComboIncreased += HandleComboIncreased;
            JokerEvents.JokerEarlyMatchDetected += HandleJokerEarlyMatchDetected;
        }

        private void OnDisable()
        {
            SessionEvents.SessionStarted -= HandleSessionStarted;
            MatchEvents.MatchCleanedUp -= HandleMatchCleanedUp;
            ComboEvents.ComboIncreased -= HandleComboIncreased;
            JokerEvents.JokerEarlyMatchDetected -= HandleJokerEarlyMatchDetected;
        }

        internal void AwardComboBonusForValidation(int comboLevel)
        {
            AwardComboBonus(comboLevel);
        }

        internal void AwardJokerBonusForValidation(int jokerTileId)
        {
            AwardJokerBonus(jokerTileId);
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

        private void HandleJokerEarlyMatchDetected(JokerEarlyMatchDetectedContext context)
        {
            if (context == null)
            {
                return;
            }

            if (!SessionDirector.HasInstance || !SessionDirector.Instance.IsSessionActive)
            {
                return;
            }

            AwardJokerBonus(context.JokerTileId);
        }

        private void AwardJokerBonus(int jokerTileId)
        {
            int bonusPoints = ScoreDefinition.JokerEarlyMatchBonus;
            if (bonusPoints <= 0)
            {
                return;
            }

            _earlyJokerMatchCount++;
            _jokerBonusTotal += bonusPoints;
            ApplyScoreDelta(bonusPoints);
            ScoreEvents.RaiseJokerBonusAwarded(new JokerBonusAwardedContext(
                bonusPoints,
                jokerTileId,
                _earlyJokerMatchCount,
                _jokerBonusTotal));
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
            _jokerBonusTotal = 0;
            _matchCount = 0;
            _earlyJokerMatchCount = 0;

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
