using MahjongGame.Progression;
using MahjongGame.Session;
using UnityEngine;

namespace MahjongGame.Timer
{
    public sealed class TimerController : MonoBehaviour
    {
        private TimerState _currentState = TimerState.Stopped;
        private float _remainingTimeSeconds;
        private float _allocatedTimeSeconds;
        private int _levelNumber = LevelProgressData.MinLevel;

        public TimerState CurrentState => _currentState;

        public float RemainingTimeSeconds => _remainingTimeSeconds;

        public float AllocatedTimeSeconds => _allocatedTimeSeconds;

        public bool IsRunning => _currentState == TimerState.Running;

        private void OnEnable()
        {
            SessionEvents.SessionStarted += HandleSessionStarted;
            SessionEvents.SessionEnded += HandleSessionEnded;
        }

        private void OnDisable()
        {
            SessionEvents.SessionStarted -= HandleSessionStarted;
            SessionEvents.SessionEnded -= HandleSessionEnded;
        }

        private void Update()
        {
            if (_currentState != TimerState.Running)
            {
                return;
            }

            AdvanceTimer(Time.deltaTime);
        }

        public bool TryStartTimer(float allocatedSeconds)
        {
            if (_currentState == TimerState.Running)
            {
                return false;
            }

            if (allocatedSeconds <= 0f)
            {
                return false;
            }

            _allocatedTimeSeconds = allocatedSeconds;
            _remainingTimeSeconds = allocatedSeconds;
            SetState(TimerState.Running);
            TimerEvents.RaiseTimerStarted(new TimerStartedContext(
                _allocatedTimeSeconds,
                _remainingTimeSeconds,
                _levelNumber));
            return true;
        }

        public bool TryPauseTimer()
        {
            if (_currentState != TimerState.Running)
            {
                return false;
            }

            SetState(TimerState.Paused);
            TimerEvents.RaiseTimerPaused();
            return true;
        }

        public bool TryResumeTimer()
        {
            if (_currentState != TimerState.Paused)
            {
                return false;
            }

            SetState(TimerState.Running);
            TimerEvents.RaiseTimerResumed();
            return true;
        }

        public void StopTimer()
        {
            _remainingTimeSeconds = 0f;
            _allocatedTimeSeconds = 0f;
            SetState(TimerState.Stopped);
        }

        internal void AdvanceTimerForValidation(float deltaSeconds)
        {
            if (_currentState != TimerState.Running)
            {
                return;
            }

            AdvanceTimer(deltaSeconds);
        }

        private void HandleSessionStarted(SessionStartedContext context)
        {
            if (context == null)
            {
                return;
            }

            _levelNumber = context.LevelNumber;
            StopTimer();
            TryStartTimer(TimerDefinition.ResolveDurationSeconds(context.LevelNumber));
        }

        private void HandleSessionEnded(SessionEndedContext context)
        {
            StopTimer();
        }

        private void AdvanceTimer(float deltaSeconds)
        {
            if (deltaSeconds <= 0f)
            {
                return;
            }

            _remainingTimeSeconds -= deltaSeconds;
            if (_remainingTimeSeconds < 0f)
            {
                _remainingTimeSeconds = 0f;
            }

            TimerEvents.RaiseTimerRemainingTimeChanged(_remainingTimeSeconds);

            if (_remainingTimeSeconds <= 0f)
            {
                SetState(TimerState.Expired);
                TimerEvents.RaiseTimerExpired(new TimerExpiredContext(_allocatedTimeSeconds, _levelNumber));
            }
        }

        private void SetState(TimerState newState)
        {
            _currentState = newState;
        }
    }
}
