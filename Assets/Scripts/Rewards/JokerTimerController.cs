using MahjongGame.Session;
using UnityEngine;

namespace MahjongGame.Rewards
{
    public sealed class JokerTimerController : MonoBehaviour
    {
        private float _elapsedSessionSeconds;
        private bool _isSessionActive;

        public float ElapsedSessionSeconds => _elapsedSessionSeconds;

        public bool IsSessionActive => _isSessionActive;

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
            if (!_isSessionActive)
            {
                return;
            }

            AdvanceElapsedTime(Time.deltaTime);
        }

        internal void AdvanceElapsedTimeForValidation(float deltaSeconds)
        {
            if (!_isSessionActive || deltaSeconds <= 0f)
            {
                return;
            }

            AdvanceElapsedTime(deltaSeconds);
        }

        internal void StartSessionForValidation()
        {
            _elapsedSessionSeconds = 0f;
            _isSessionActive = true;
        }

        private void HandleSessionStarted(SessionStartedContext context)
        {
            _elapsedSessionSeconds = 0f;
            _isSessionActive = true;
        }

        private void HandleSessionEnded(SessionEndedContext context)
        {
            _isSessionActive = false;
        }

        private void AdvanceElapsedTime(float deltaSeconds)
        {
            _elapsedSessionSeconds += deltaSeconds;
        }
    }
}
