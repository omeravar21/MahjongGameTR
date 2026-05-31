using MahjongGame.Core;
using MahjongGame.Progression;
using UnityEngine;

namespace MahjongGame.Session
{
    public sealed class SessionDirector : MonoBehaviour
    {
        private static SessionDirector _instance;
        private static int _nextSessionId = 1;

        private LevelSessionState _currentState = LevelSessionState.None;
        private SessionData _currentSession;

        public static SessionDirector Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[SessionDirector] Instance is not available.");
                }

                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        public LevelSessionState CurrentState => _currentState;

        public SessionData CurrentSession => _currentSession;

        public bool IsSessionActive => _currentState == LevelSessionState.Active;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
                return;
            }

            _instance = this;
        }

        private void OnEnable()
        {
            GameEvents.SceneLoadCompleted += HandleSceneLoadCompleted;
        }

        private void OnDisable()
        {
            GameEvents.SceneLoadCompleted -= HandleSceneLoadCompleted;

            if (_currentState == LevelSessionState.Starting || _currentState == LevelSessionState.Active)
            {
                TryEndSession(SessionEndReason.Manual);
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public bool TryStartSession(out SessionData session)
        {
            session = null;

            if (_currentState == LevelSessionState.Starting || _currentState == LevelSessionState.Active)
            {
                return false;
            }

            int levelNumber = ResolveLevelNumber();
            int sessionId = _nextSessionId++;

            SetState(LevelSessionState.Starting);
            PrepareLevelSession(levelNumber);
            SetState(LevelSessionState.Active);

            _currentSession = new SessionData(sessionId, levelNumber, _currentState);
            SessionEvents.RaiseSessionStarted(new SessionStartedContext(_currentSession));
            session = _currentSession;
            return true;
        }

        public bool TryEndSession(SessionEndReason reason)
        {
            if (_currentState == LevelSessionState.None || _currentState == LevelSessionState.Ended)
            {
                return false;
            }

            SessionData endedSession = _currentSession;
            SetState(LevelSessionState.Ending);
            SetState(LevelSessionState.Ended);
            SessionEvents.RaiseSessionEnded(new SessionEndedContext(endedSession, reason));
            _currentSession = null;
            return true;
        }

        private void HandleSceneLoadCompleted(string sceneName)
        {
            if (sceneName != SceneLoadController.GameSceneName)
            {
                return;
            }

            TryStartSession(out _);
        }

        private int ResolveLevelNumber()
        {
            if (PlayerProgressionDirector.HasInstance)
            {
                return PlayerProgressionDirector.Instance.CurrentLevel;
            }

            Debug.LogWarning("[SessionDirector] PlayerProgressionDirector is not available. Using default level.");
            return LevelProgressData.MinLevel;
        }

        private void PrepareLevelSession(int levelNumber)
        {
            Debug.Log("[SessionDirector] Preparing level session for level " + levelNumber + ".");
        }

        private void SetState(LevelSessionState newState)
        {
            if (_currentState == newState)
            {
                return;
            }

            LevelSessionState previousState = _currentState;
            _currentState = newState;

            if (_currentSession != null)
            {
                _currentSession = new SessionData(
                    _currentSession.SessionId,
                    _currentSession.LevelNumber,
                    newState);
            }

            SessionEvents.RaiseSessionStateChanged(previousState, newState);
        }
    }
}
