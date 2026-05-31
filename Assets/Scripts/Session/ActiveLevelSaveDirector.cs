using MahjongGame.Board;
using MahjongGame.Boosters;
using MahjongGame.ClosedTiles;
using MahjongGame.Combo;
using MahjongGame.Core;
using MahjongGame.Core.Save;
using MahjongGame.Matching;
using MahjongGame.Rewards;
using MahjongGame.Score;
using MahjongGame.Session;
using MahjongGame.Timer;
using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Session
{
    [DefaultExecutionOrder(-50)]
    public sealed class ActiveLevelSaveDirector : MonoBehaviour
    {
        private const float SaveDebounceSeconds = 2f;

        private static ActiveLevelSaveDirector _instance;

        private float _saveDebounceRemainingSeconds;
        private int _shuffleUsedInSession;
        private int _undoUsedInSession;
        private int _hintUsedInSession;

        public static ActiveLevelSaveDirector Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[ActiveLevelSaveDirector] Instance is not available.");
                }

                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
                return;
            }

            _instance = this;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void OnEnable()
        {
            SessionEvents.SessionStarted += HandleSessionStarted;
            SessionEvents.SessionEnded += HandleSessionEnded;
            MatchEvents.MatchCleanedUp += HandleMatchCleanedUp;
            ScoreEvents.ScoreChanged += HandleScoreChanged;
            ComboEvents.ComboChanged += HandleComboChanged;
            TimerEvents.TimerRemainingTimeChanged += HandleTimerRemainingTimeChanged;
            BoosterEvents.BoosterUsedInSession += HandleBoosterUsedInSession;
        }

        private void OnDisable()
        {
            SessionEvents.SessionStarted -= HandleSessionStarted;
            SessionEvents.SessionEnded -= HandleSessionEnded;
            MatchEvents.MatchCleanedUp -= HandleMatchCleanedUp;
            ScoreEvents.ScoreChanged -= HandleScoreChanged;
            ComboEvents.ComboChanged -= HandleComboChanged;
            TimerEvents.TimerRemainingTimeChanged -= HandleTimerRemainingTimeChanged;
            BoosterEvents.BoosterUsedInSession -= HandleBoosterUsedInSession;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                TryPersistActiveSession();
            }
        }

        private void OnApplicationQuit()
        {
            TryPersistActiveSession();
        }

        private void Update()
        {
            if (!SessionDirector.HasInstance || !SessionDirector.Instance.IsSessionActive)
            {
                return;
            }

            _saveDebounceRemainingSeconds -= Time.deltaTime;
            if (_saveDebounceRemainingSeconds <= 0f)
            {
                _saveDebounceRemainingSeconds = SaveDebounceSeconds;
                TryPersistActiveSession();
            }
        }

        public bool HasPersistedActiveSession()
        {
            if (!SaveSystem.HasInstance || SaveSystem.Instance.Data == null)
            {
                return false;
            }

            SaveSystem.Instance.Data.EnsureDefaults();
            return SaveSystem.Instance.Data.activeLevelState.hasActiveSession;
        }

        public bool TryRestoreActiveSession()
        {
            if (!SaveSystem.HasInstance || SaveSystem.Instance.Data == null)
            {
                return false;
            }

            PlayerSaveData playerSaveData = SaveSystem.Instance.Data;
            playerSaveData.EnsureDefaults();
            ActiveLevelStateSaveData savedState = playerSaveData.activeLevelState;
            if (!savedState.hasActiveSession)
            {
                return false;
            }

            if (!SessionDirector.HasInstance)
            {
                return false;
            }

            Transform gameplayRoot = transform;
            Transform boardRoot = gameplayRoot.Find("BoardRoot");
            BoardSpawner boardSpawner = boardRoot != null ? boardRoot.GetComponent<BoardSpawner>() : null;
            TrayController trayController = gameplayRoot.GetComponent<TrayController>();
            ClosedTileController closedTileController = gameplayRoot.GetComponent<ClosedTileController>();
            RewardDirector rewardDirector = gameplayRoot.GetComponent<RewardDirector>();

            if (boardRoot == null || boardSpawner == null)
            {
                Debug.LogWarning("[ActiveLevelSaveDirector] Board restore wiring is incomplete.");
                return false;
            }

            if (!ActiveBoardStateRestorer.TryRestore(
                    gameplayRoot,
                    boardRoot,
                    boardSpawner,
                    trayController,
                    closedTileController,
                    rewardDirector,
                    savedState))
            {
                Debug.LogWarning("[ActiveLevelSaveDirector] Failed to restore saved board state.");
                ClearActiveSessionSave();
                return false;
            }

            if (!SessionDirector.Instance.TryStartSessionFromRestore(savedState.currentLevel, savedState.currentSeed, out _))
            {
                Debug.LogWarning("[ActiveLevelSaveDirector] Failed to start restored session.");
                return false;
            }

            RestoreSessionMetrics(gameplayRoot, savedState);
            _shuffleUsedInSession = savedState.shuffleUsed;
            _undoUsedInSession = savedState.undoUsed;
            _hintUsedInSession = savedState.hintUsed;
            TryPersistActiveSession();
            return true;
        }

        internal void NotifyBoardSeed(int seed)
        {
            if (!SaveSystem.HasInstance || SaveSystem.Instance.Data == null)
            {
                return;
            }

            SaveSystem.Instance.Data.EnsureDefaults();
            SaveSystem.Instance.Data.activeLevelState.currentSeed = seed;
        }

        internal void ResetBoosterUsageForValidation()
        {
            _shuffleUsedInSession = 0;
            _undoUsedInSession = 0;
            _hintUsedInSession = 0;
        }

        internal bool TryPersistActiveSessionForValidation()
        {
            return TryPersistActiveSession(forcePersist: true);
        }

        internal void ClearActiveSessionSaveForValidation()
        {
            ClearActiveSessionSave();
        }

        private void HandleSessionStarted(SessionStartedContext context)
        {
            if (context == null || context.IsResumeSession)
            {
                return;
            }

            ResetSessionUsageCounters();
            MarkActiveSessionStarted(context.LevelNumber, SessionDirector.Instance.LastBoardSeed);
            TryPersistActiveSession(forcePersist: true);
        }

        private void HandleSessionEnded(SessionEndedContext context)
        {
            if (context == null)
            {
                return;
            }

            if (context.Reason == SessionEndReason.Win || context.Reason == SessionEndReason.Lose)
            {
                ClearActiveSessionSave();
                return;
            }

            TryPersistActiveSession(forcePersist: true);
        }

        private void HandleMatchCleanedUp(MatchCleanupContext context)
        {
            RequestDebouncedSave();
        }

        private void HandleScoreChanged(ScoreChangedContext context)
        {
            RequestDebouncedSave();
        }

        private void HandleComboChanged(ComboChangedContext context)
        {
            RequestDebouncedSave();
        }

        private void HandleTimerRemainingTimeChanged(float remainingTimeSeconds)
        {
            RequestDebouncedSave();
        }

        private void HandleBoosterUsedInSession(BoosterType boosterType)
        {
            switch (boosterType)
            {
                case BoosterType.Shuffle:
                    _shuffleUsedInSession++;
                    break;
                case BoosterType.Undo:
                    _undoUsedInSession++;
                    break;
                case BoosterType.Hint:
                    _hintUsedInSession++;
                    break;
            }

            RequestDebouncedSave();
        }

        private void RequestDebouncedSave()
        {
            if (!SessionDirector.HasInstance || !SessionDirector.Instance.IsSessionActive)
            {
                return;
            }

            _saveDebounceRemainingSeconds = 0f;
        }

        private bool TryPersistActiveSession(bool forcePersist = false)
        {
            if (!SaveSystem.HasInstance || SaveSystem.Instance.Data == null)
            {
                return false;
            }

            if (!SessionDirector.HasInstance || !SessionDirector.Instance.IsSessionActive)
            {
                return false;
            }

            if (!forcePersist && _saveDebounceRemainingSeconds > 0f)
            {
                return false;
            }

            PlayerSaveData playerSaveData = SaveSystem.Instance.Data;
            playerSaveData.EnsureDefaults();
            ActiveLevelStateSaveData activeState = playerSaveData.activeLevelState;
            activeState.hasActiveSession = true;
            activeState.currentLevel = SessionDirector.Instance.CurrentSession.LevelNumber;
            activeState.currentSeed = SessionDirector.Instance.LastBoardSeed;

            TimerController timerController = GetComponent<TimerController>();
            ScoreController scoreController = GetComponent<ScoreController>();
            ComboController comboController = GetComponent<ComboController>();
            TrayController trayController = GetComponent<TrayController>();
            ClosedTileController closedTileController = GetComponent<ClosedTileController>();
            Transform boardRoot = transform.Find("BoardRoot");

            activeState.remainingTimer = timerController != null ? timerController.RemainingTimeSeconds : 0f;
            activeState.score = scoreController != null ? scoreController.CurrentScore : 0;
            activeState.currentCombo = comboController != null ? comboController.CurrentCombo : 0;
            activeState.highestComboInSession = comboController != null ? comboController.HighestCombo : 0;
            activeState.shuffleUsed = _shuffleUsedInSession;
            activeState.undoUsed = _undoUsedInSession;
            activeState.hintUsed = _hintUsedInSession;

            ActiveBoardStateSerializer.WriteBoardStateJson(
                activeState,
                boardRoot,
                trayController,
                closedTileController);

            SaveSystem.Instance.Save();
            _saveDebounceRemainingSeconds = SaveDebounceSeconds;
            return true;
        }

        private void RestoreSessionMetrics(Transform gameplayRoot, ActiveLevelStateSaveData savedState)
        {
            TimerController timerController = gameplayRoot.GetComponent<TimerController>();
            ScoreController scoreController = gameplayRoot.GetComponent<ScoreController>();
            ComboController comboController = gameplayRoot.GetComponent<ComboController>();

            float allocatedSeconds = TimerDefinition.ResolveDurationSeconds(savedState.currentLevel);
            timerController?.TryRestoreTimerForResume(
                allocatedSeconds,
                savedState.remainingTimer,
                savedState.currentLevel);
            scoreController?.RestoreScoreStateForResume(savedState.score);
            comboController?.RestoreComboStateForResume(
                savedState.currentCombo,
                savedState.highestComboInSession);
        }

        private void MarkActiveSessionStarted(int levelNumber, int seed)
        {
            if (!SaveSystem.HasInstance || SaveSystem.Instance.Data == null)
            {
                return;
            }

            PlayerSaveData playerSaveData = SaveSystem.Instance.Data;
            playerSaveData.EnsureDefaults();
            ActiveLevelStateSaveData activeState = playerSaveData.activeLevelState;
            activeState.hasActiveSession = true;
            activeState.currentLevel = levelNumber;
            activeState.currentSeed = seed;
            activeState.remainingTimer = TimerDefinition.ResolveDurationSeconds(levelNumber);
            activeState.score = 0;
            activeState.currentCombo = 0;
            activeState.highestComboInSession = 0;
            activeState.shuffleUsed = 0;
            activeState.undoUsed = 0;
            activeState.hintUsed = 0;
            activeState.boardStateJson = string.Empty;
            activeState.trayStateJson = string.Empty;
            activeState.closedTileStateJson = string.Empty;
            activeState.matchedTilesJson = string.Empty;
            activeState.remainingTilesJson = string.Empty;
        }

        private void ClearActiveSessionSave()
        {
            if (!SaveSystem.HasInstance || SaveSystem.Instance.Data == null)
            {
                return;
            }

            PlayerSaveData playerSaveData = SaveSystem.Instance.Data;
            playerSaveData.EnsureDefaults();
            playerSaveData.activeLevelState = new ActiveLevelStateSaveData();
            SaveSystem.Instance.Save();
            ResetSessionUsageCounters();
        }

        private void ResetSessionUsageCounters()
        {
            _shuffleUsedInSession = 0;
            _undoUsedInSession = 0;
            _hintUsedInSession = 0;
            _saveDebounceRemainingSeconds = SaveDebounceSeconds;
        }
    }
}
