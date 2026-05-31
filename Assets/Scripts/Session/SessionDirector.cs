using MahjongGame.Board;
using MahjongGame.BoardGeneration;
using MahjongGame.ClosedTiles;
using MahjongGame.Rewards;
using MahjongGame.Core;
using MahjongGame.Progression;
using MahjongGame.Tiles;
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

        public int LastBoardSeed { get; private set; }

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

            if (ActiveLevelSaveDirector.HasInstance
                && ActiveLevelSaveDirector.Instance.HasPersistedActiveSession()
                && ActiveLevelSaveDirector.Instance.TryRestoreActiveSession())
            {
                return;
            }

            TryStartSession(out _);
        }

        public bool TryStartSessionFromRestore(int levelNumber, int boardSeed, out SessionData session)
        {
            session = null;

            if (_currentState == LevelSessionState.Starting || _currentState == LevelSessionState.Active)
            {
                return false;
            }

            LastBoardSeed = boardSeed;
            int sessionId = _nextSessionId++;

            SetState(LevelSessionState.Starting);
            SetState(LevelSessionState.Active);

            _currentSession = new SessionData(sessionId, levelNumber, _currentState);
            SessionEvents.RaiseSessionStarted(new SessionStartedContext(_currentSession, isResumeSession: true));
            session = _currentSession;
            return true;
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

            if (DifficultyDirector.HasInstance)
            {
                DifficultyProfile profile = DifficultyDirector.Instance.ResolveProfile(levelNumber);
                Debug.Log(
                    "[SessionDirector] Difficulty profile resolved: tiles="
                    + profile.TileCount
                    + ", closed="
                    + profile.ClosedTileCount
                    + ", jokers="
                    + profile.JokerCount
                    + ", layers="
                    + profile.LayerDepth
                    + ", timer="
                    + profile.RecommendedTimerSeconds
                    + "s, tier="
                    + profile.ComplexityTier
                    + ".");
            }

            if (VisualVarietyDirector.HasInstance)
            {
                VisualVarietyProfile varietyProfile = VisualVarietyDirector.Instance.ResolveProfile(levelNumber);
                Debug.Log(
                    "[SessionDirector] Visual variety profile resolved: archetype="
                    + varietyProfile.ArchetypeId
                    + ", variation="
                    + varietyProfile.VariationIndex
                    + ", holePattern="
                    + varietyProfile.HolePatternId
                    + ", closedPattern="
                    + varietyProfile.ClosedTilePatternId
                    + ", seed="
                    + varietyProfile.DeterministicSeed
                    + ".");
            }

            if (LevelRecipeGenerator.HasInstance)
            {
                LevelRecipe recipe = LevelRecipeGenerator.Instance.GenerateRecipe(levelNumber);
                Debug.Log(
                    "[SessionDirector] Level recipe generated: seed="
                    + recipe.Seed
                    + ", tiles="
                    + recipe.TileCount
                    + ", layers="
                    + recipe.LayerDepth
                    + ", archetype="
                    + recipe.ArchetypeId
                    + ", variation="
                    + recipe.VariationIndex
                    + ", jokers="
                    + recipe.JokerCount
                    + ", rewardPattern="
                    + recipe.RewardJokerPatternId
                    + ", difficultyRating="
                    + recipe.DifficultyRating
                    + ", maxAttempts="
                    + recipe.MaxRegenerationAttempts
                    + ".");
            }

            TrySpawnRuntimeBoard(levelNumber);
        }

        private void TrySpawnRuntimeBoard(int levelNumber)
        {
            BoardData boardData = BoardGenerationPipeline.GenerateBoardData(levelNumber);
            Debug.Log(
                "[SessionDirector] Board generation pipeline completed: level="
                + boardData.LevelNumber
                + ", tiles="
                + boardData.TileCount
                + ", layers="
                + boardData.LayerDepth
                + ", archetype="
                + boardData.ArchetypeId
                + ", variation="
                + boardData.VariationIndex
                + ", holePattern="
                + boardData.HolePatternId
                + ", closed="
                + boardData.ClosedTileCount
                + ", jokers="
                + boardData.JokerCount
                + ", validated="
                + boardData.IsValidated
                + ", seed="
                + boardData.Seed
                + ".");

            Transform boardRoot = transform.Find("BoardRoot");
            if (boardRoot == null)
            {
                Debug.LogWarning("[SessionDirector] BoardRoot was not found under GameplayRoot.");
                return;
            }

            BoardSpawner boardSpawner = boardRoot.GetComponent<BoardSpawner>();
            if (boardSpawner == null)
            {
                Debug.LogWarning("[SessionDirector] BoardSpawner is missing on BoardRoot.");
                return;
            }

            if (!boardSpawner.Spawn(boardData))
            {
                Debug.LogWarning("[SessionDirector] BoardSpawner failed to spawn runtime board for level " + levelNumber + ".");
                return;
            }

            LastBoardSeed = boardData.Seed;
            if (ActiveLevelSaveDirector.HasInstance)
            {
                ActiveLevelSaveDirector.Instance.NotifyBoardSeed(boardData.Seed);
            }

            RegisterSpawnedClosedTiles(boardRoot);
            RegisterSpawnedJokerTiles(boardRoot);
        }

        private void RegisterSpawnedJokerTiles(Transform boardRoot)
        {
            RewardDirector rewardDirector = GetComponent<RewardDirector>();
            if (rewardDirector == null)
            {
                return;
            }

            JokerTileController jokerTileController = rewardDirector.GetJokerTileController();
            if (jokerTileController == null)
            {
                return;
            }

            jokerTileController.ResetRuntimeState();

            System.Collections.Generic.List<Tile> occupyingTiles = BoardTileOccupancyQuery.CollectOccupyingTiles(boardRoot);
            for (int index = 0; index < occupyingTiles.Count; index++)
            {
                Tile tile = occupyingTiles[index];
                if (tile != null && tile.IsJoker)
                {
                    jokerTileController.TryRegisterJokerTile(tile);
                }
            }
        }

        private void RegisterSpawnedClosedTiles(Transform boardRoot)
        {
            ClosedTileController closedTileController = GetComponent<ClosedTileController>();
            if (closedTileController == null)
            {
                return;
            }

            closedTileController.ResetRuntimeState();

            System.Collections.Generic.List<Tile> occupyingTiles = BoardTileOccupancyQuery.CollectOccupyingTiles(boardRoot);
            for (int index = 0; index < occupyingTiles.Count; index++)
            {
                Tile tile = occupyingTiles[index];
                if (tile != null && tile.IsClosed)
                {
                    closedTileController.TryRegisterClosedTile(tile);
                }
            }
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
