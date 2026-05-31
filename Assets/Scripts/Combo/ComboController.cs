using MahjongGame.Matching;
using MahjongGame.Session;
using UnityEngine;

namespace MahjongGame.Combo
{
    public sealed class ComboController : MonoBehaviour
    {
        private int _currentCombo;
        private int _highestCombo;
        private int _totalComboCount;
        private float _comboWindowRemainingSeconds;
        private bool _comboWindowActive;

        public int CurrentCombo => _currentCombo;

        public int HighestCombo => _highestCombo;

        public int TotalComboCount => _totalComboCount;

        public float ComboWindowRemainingSeconds => _comboWindowRemainingSeconds;

        public bool IsComboWindowActive => _comboWindowActive;

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

        private void Update()
        {
            if (!_comboWindowActive)
            {
                return;
            }

            AdvanceComboWindow(Time.deltaTime);
        }

        private void HandleSessionStarted(SessionStartedContext context)
        {
            ResetComboState();
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

            RegisterMatchForCombo();
        }

        internal void RegisterMatchForComboValidation()
        {
            RegisterMatchForCombo();
        }

        internal void AdvanceComboWindowForValidation(float deltaSeconds)
        {
            AdvanceComboWindow(deltaSeconds);
        }

        private void RegisterMatchForCombo()
        {
            if (_comboWindowActive && _comboWindowRemainingSeconds > 0f)
            {
                SetCurrentCombo(_currentCombo + 1);
            }
            else
            {
                SetCurrentCombo(1);
            }

            StartComboWindow();

            if (_currentCombo >= 2)
            {
                _totalComboCount++;
                ComboEvents.RaiseComboIncreased(new ComboIncreasedContext(
                    _currentCombo,
                    _highestCombo));
            }
        }

        private void StartComboWindow()
        {
            _comboWindowActive = true;
            _comboWindowRemainingSeconds = ComboDefinition.ComboWindowSeconds;
        }

        private void AdvanceComboWindow(float deltaSeconds)
        {
            if (deltaSeconds <= 0f)
            {
                return;
            }

            _comboWindowRemainingSeconds -= deltaSeconds;
            if (_comboWindowRemainingSeconds > 0f)
            {
                return;
            }

            _comboWindowRemainingSeconds = 0f;
            _comboWindowActive = false;
            ExpireCombo();
        }

        private void ExpireCombo()
        {
            if (_currentCombo == 0)
            {
                return;
            }

            SetCurrentCombo(0);
            ComboEvents.RaiseComboExpired();
        }

        private void ResetComboState()
        {
            _comboWindowActive = false;
            _comboWindowRemainingSeconds = 0f;
            _totalComboCount = 0;
            SetCurrentCombo(0);
        }

        private void SetCurrentCombo(int newCombo)
        {
            if (newCombo < 0)
            {
                newCombo = 0;
            }

            int previousCombo = _currentCombo;
            if (previousCombo == newCombo)
            {
                return;
            }

            _currentCombo = newCombo;
            if (_currentCombo > _highestCombo)
            {
                _highestCombo = _currentCombo;
            }

            ComboEvents.RaiseComboChanged(new ComboChangedContext(previousCombo, _currentCombo));
        }
    }
}
