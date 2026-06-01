using System.Reflection;
using System.Text;
using MahjongGame.Board;
using MahjongGame.BoardGeneration;
using MahjongGame.Matching;
using MahjongGame.Progression;
using MahjongGame.Tiles;
using MahjongGame.Timer;
using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Session
{
    public static class SessionSystemValidator
    {
        public static bool Validate(Transform gameplayRoot, StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            if (gameplayRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] GameplayRoot is missing for session system validation.");
                return false;
            }

            SessionDirector sessionDirector = gameplayRoot.GetComponent<SessionDirector>();
            WinConditionController winConditionController = gameplayRoot.GetComponent<WinConditionController>();
            LoseConditionController loseConditionController = gameplayRoot.GetComponent<LoseConditionController>();
            SessionRestartController restartController = gameplayRoot.GetComponent<SessionRestartController>();
            MatchController matchController = gameplayRoot.GetComponent<MatchController>();
            TileMovementController movementController = gameplayRoot.GetComponent<TileMovementController>();
            TrayController trayController = gameplayRoot.GetComponent<TrayController>();

            Transform boardRoot = gameplayRoot.Find("BoardRoot");
            BoardPreviewSpawner previewSpawner = boardRoot != null
                ? boardRoot.GetComponent<BoardPreviewSpawner>()
                : null;
            BoardSpawner boardSpawner = boardRoot != null
                ? boardRoot.GetComponent<BoardSpawner>()
                : null;

            passed &= ValidateComponents(
                sessionDirector,
                winConditionController,
                loseConditionController,
                restartController,
                previewSpawner,
                boardSpawner,
                boardRoot,
                reportBuilder);
            passed &= ValidateTypes(reportBuilder);
            passed &= ValidateSessionEvents(reportBuilder);
            passed &= ValidatePenaltyEvents(reportBuilder);
            passed &= ValidateResetApis(matchController, movementController, trayController, reportBuilder);

            if (Application.isPlaying && sessionDirector != null && sessionDirector.enabled)
            {
                passed &= ValidateEventWiring(
                    winConditionController,
                    loseConditionController,
                    restartController,
                    reportBuilder);
                passed &= ValidatePlayModeLoop(
                    sessionDirector,
                    gameplayRoot,
                    trayController,
                    boardRoot,
                    reportBuilder);
            }
            else if (!Application.isPlaying)
            {
                AppendLine(reportBuilder, "[SKIP] Session loop checks require Play Mode on GameScene.");
            }
            else
            {
                AppendLine(reportBuilder, "[SKIP] Session loop checks require an enabled SessionDirector in Play Mode.");
            }

            AppendLine(reportBuilder, passed
                ? "[PASS] Session system validation completed successfully."
                : "[FAIL] Session system validation found issues.");

            return passed;
        }

        private static bool ValidateComponents(
            SessionDirector sessionDirector,
            WinConditionController winConditionController,
            LoseConditionController loseConditionController,
            SessionRestartController restartController,
            BoardPreviewSpawner previewSpawner,
            BoardSpawner boardSpawner,
            Transform boardRoot,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            if (sessionDirector == null)
            {
                AppendLine(reportBuilder, "[FAIL] SessionDirector is missing on GameplayRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] SessionDirector is present on GameplayRoot.");
            }

            if (winConditionController == null)
            {
                AppendLine(reportBuilder, "[FAIL] WinConditionController is missing on GameplayRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] WinConditionController is present on GameplayRoot.");
            }

            if (loseConditionController == null)
            {
                AppendLine(reportBuilder, "[FAIL] LoseConditionController is missing on GameplayRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] LoseConditionController is present on GameplayRoot.");
            }

            if (restartController == null)
            {
                AppendLine(reportBuilder, "[FAIL] SessionRestartController is missing on GameplayRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] SessionRestartController is present on GameplayRoot.");
            }

            if (boardRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoardRoot is missing under GameplayRoot.");
                passed = false;
            }
            else if (previewSpawner == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoardPreviewSpawner is missing on BoardRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] BoardPreviewSpawner is present on BoardRoot.");
            }

            if (boardSpawner == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoardSpawner is missing on BoardRoot.");
                passed = false;
            }
            else if (!boardSpawner.HasTilePrefab)
            {
                AppendLine(reportBuilder, "[FAIL] BoardSpawner tile prefab is not configured.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] BoardSpawner is present on BoardRoot.");
            }

            return passed;
        }

        private static bool ValidateTypes(StringBuilder reportBuilder)
        {
            bool passed = true;

            if (typeof(LevelCompletionQuery) == null)
            {
                AppendLine(reportBuilder, "[FAIL] LevelCompletionQuery type is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] LevelCompletionQuery type is present.");
            }

            if (typeof(LevelRuntimeResetter) == null)
            {
                AppendLine(reportBuilder, "[FAIL] LevelRuntimeResetter type is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] LevelRuntimeResetter type is present.");
            }

            if (typeof(SessionEvents) == null)
            {
                AppendLine(reportBuilder, "[FAIL] SessionEvents type is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] SessionEvents type is present.");
            }

            if (typeof(SessionEndReason) == null)
            {
                AppendLine(reportBuilder, "[FAIL] SessionEndReason type is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] SessionEndReason type is present.");
            }

            return passed;
        }

        private static bool ValidateSessionEvents(StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= ValidateEventExists(typeof(SessionEvents), nameof(SessionEvents.SessionStarted), reportBuilder);
            passed &= ValidateEventExists(typeof(SessionEvents), nameof(SessionEvents.SessionStateChanged), reportBuilder);
            passed &= ValidateEventExists(typeof(SessionEvents), nameof(SessionEvents.SessionEnded), reportBuilder);

            return passed;
        }

        private static bool ValidatePenaltyEvents(StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= ValidateEventExists(typeof(PenaltyEvents), nameof(PenaltyEvents.TimerExpirationPenaltyDetected), reportBuilder);

            return passed;
        }

        private static bool ValidateEventExists(System.Type eventType, string eventName, StringBuilder reportBuilder)
        {
            EventInfo eventInfo = eventType.GetEvent(eventName, BindingFlags.Static | BindingFlags.Public);
            if (eventInfo == null)
            {
                AppendLine(reportBuilder, "[FAIL] " + eventType.Name + "." + eventName + " event is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] " + eventType.Name + "." + eventName + " event is present.");
            return true;
        }

        private static bool ValidateResetApis(
            MatchController matchController,
            TileMovementController movementController,
            TrayController trayController,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= ValidatePublicMethod(
                matchController,
                nameof(MatchController.ResetProcessingState),
                reportBuilder);
            passed &= ValidatePublicMethod(
                movementController,
                nameof(TileMovementController.ResetMovementState),
                reportBuilder);
            passed &= ValidatePublicMethod(
                trayController,
                nameof(TrayController.ResetRuntimeState),
                reportBuilder);

            return passed;
        }

        private static bool ValidatePublicMethod(
            Component component,
            string methodName,
            StringBuilder reportBuilder)
        {
            if (component == null)
            {
                AppendLine(reportBuilder, "[FAIL] Cannot validate " + methodName + " because component is missing.");
                return false;
            }

            MethodInfo method = component.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);

            if (method == null)
            {
                AppendLine(reportBuilder, "[FAIL] " + component.GetType().Name + "." + methodName + " is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] " + component.GetType().Name + "." + methodName + " is present.");
            return true;
        }

        private static bool ValidateEventWiring(
            WinConditionController winConditionController,
            LoseConditionController loseConditionController,
            SessionRestartController restartController,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= ValidateSubscription(
                winConditionController,
                typeof(MatchEvents),
                nameof(MatchEvents.MatchCleanedUp),
                reportBuilder);
            passed &= ValidateSubscription(
                loseConditionController,
                typeof(TrayEvents),
                nameof(TrayEvents.TrayCapacityOverflowDetected),
                reportBuilder);
            passed &= ValidateSubscription(
                loseConditionController,
                typeof(TimerEvents),
                nameof(TimerEvents.TimerExpired),
                reportBuilder);
            passed &= ValidateSubscription(
                restartController,
                typeof(SessionEvents),
                nameof(SessionEvents.SessionEnded),
                reportBuilder);

            return passed;
        }

        private static bool ValidateSubscription(
            Component subscriber,
            System.Type eventType,
            string eventName,
            StringBuilder reportBuilder)
        {
            if (subscriber == null)
            {
                AppendLine(reportBuilder, "[FAIL] Cannot validate " + eventName + " subscription because subscriber is missing.");
                return false;
            }

            if (!IsSubscribed(subscriber, eventType, eventName))
            {
                AppendLine(reportBuilder, "[FAIL] " + subscriber.GetType().Name + " is not subscribed to " + eventName + ".");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] " + subscriber.GetType().Name + " is subscribed to " + eventName + ".");
            return true;
        }

        private static bool IsSubscribed(Component subscriber, System.Type eventType, string eventName)
        {
            FieldInfo eventBackingField = eventType.GetField(
                eventName,
                BindingFlags.Static | BindingFlags.NonPublic);

            if (eventBackingField == null)
            {
                return false;
            }

            if (!(eventBackingField.GetValue(null) is System.Delegate subscribers))
            {
                return false;
            }

            foreach (System.Delegate handler in subscribers.GetInvocationList())
            {
                if (ReferenceEquals(handler.Target, subscriber))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ValidatePlayModeLoop(
            SessionDirector sessionDirector,
            Transform gameplayRoot,
            TrayController trayController,
            Transform boardRoot,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            if (!SessionDirector.HasInstance)
            {
                AppendLine(reportBuilder, "[FAIL] SessionDirector instance is not available in Play Mode.");
                return false;
            }

            passed &= ValidateSessionStart(sessionDirector, boardRoot, reportBuilder);
            passed &= ValidateSessionOwnership(sessionDirector, reportBuilder);
            passed &= ValidateFailAndRestart(sessionDirector, trayController, boardRoot, reportBuilder);
            passed &= ValidateTimerExpirationFail(sessionDirector, gameplayRoot, reportBuilder);
            passed &= ValidateWinWithoutRestart(sessionDirector, reportBuilder);

            return passed;
        }

        private static bool ValidateSessionStart(
            SessionDirector sessionDirector,
            Transform boardRoot,
            StringBuilder reportBuilder)
        {
            if (!sessionDirector.IsSessionActive)
            {
                AppendLine(reportBuilder, "[FAIL] Session is not active after GameScene start.");
                return false;
            }

            if (sessionDirector.CurrentState != LevelSessionState.Active)
            {
                AppendLine(reportBuilder, "[FAIL] SessionDirector.CurrentState is not Active after GameScene start.");
                return false;
            }

            if (sessionDirector.CurrentSession == null)
            {
                AppendLine(reportBuilder, "[FAIL] SessionDirector.CurrentSession is null after GameScene start.");
                return false;
            }

            int sessionLevel = sessionDirector.CurrentSession.LevelNumber;
            int currentLevel = ResolveCurrentLevel();
            if (sessionLevel != currentLevel)
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] Session level "
                    + sessionLevel
                    + " does not match current progression level "
                    + currentLevel
                    + ".");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Session level matches current progression level.");

            DifficultyProfile profile = DifficultyDirector.HasInstance
                ? DifficultyDirector.Instance.ResolveProfile(sessionLevel)
                : DifficultyDefinition.ResolveProfile(sessionLevel);

            int boardTileCount = boardRoot != null
                ? BoardTileOccupancyQuery.CollectOccupyingTiles(boardRoot).Count
                : 0;
            if (boardTileCount != profile.TileCount)
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] Spawned board tile count "
                    + boardTileCount
                    + " does not match difficulty profile "
                    + profile.TileCount
                    + ".");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Spawned board tile count matches difficulty profile.");
            AppendLine(reportBuilder, "[PASS] Session started and is active.");
            return true;
        }

        private static bool ValidateSessionOwnership(SessionDirector sessionDirector, StringBuilder reportBuilder)
        {
            if (sessionDirector.TryStartSession(out _))
            {
                AppendLine(reportBuilder, "[FAIL] TryStartSession succeeded while session was already active.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] TryStartSession is rejected while session is active.");
            return true;
        }

        private static bool ValidateFailAndRestart(
            SessionDirector sessionDirector,
            TrayController trayController,
            Transform boardRoot,
            StringBuilder reportBuilder)
        {
            int levelBefore = ResolveCurrentLevel();
            int sessionIdBefore = sessionDirector.CurrentSession.SessionId;

            if (!sessionDirector.TryEndSession(SessionEndReason.Lose))
            {
                AppendLine(reportBuilder, "[FAIL] TryEndSession(Lose) failed during fail/restart validation.");
                return false;
            }

            if (!sessionDirector.IsSessionActive)
            {
                AppendLine(reportBuilder, "[FAIL] Session is not active after lose-triggered restart.");
                return false;
            }

            if (sessionDirector.CurrentSession == null)
            {
                AppendLine(reportBuilder, "[FAIL] CurrentSession is null after lose-triggered restart.");
                return false;
            }

            if (sessionDirector.CurrentSession.SessionId <= sessionIdBefore)
            {
                AppendLine(reportBuilder, "[FAIL] Session id did not increase after lose-triggered restart.");
                return false;
            }

            if (ResolveCurrentLevel() != levelBefore)
            {
                AppendLine(reportBuilder, "[FAIL] Player level changed after lose-triggered restart.");
                return false;
            }

            if (trayController != null && trayController.ReservedTileCount != 0)
            {
                AppendLine(reportBuilder, "[FAIL] Tray is not empty after lose-triggered restart.");
                return false;
            }

            BoardData expectedBoardData = BoardGenerationPipeline.GenerateBoardData(levelBefore);
            int expectedBoardTileCount = expectedBoardData != null ? expectedBoardData.TileCount : 0;
            int boardTileCount = boardRoot != null
                ? BoardTileOccupancyQuery.CollectOccupyingTiles(boardRoot).Count
                : 0;

            if (expectedBoardTileCount <= 0)
            {
                AppendLine(reportBuilder, "[FAIL] BoardGenerationPipeline produced empty board data for restart validation.");
                return false;
            }

            if (boardTileCount != expectedBoardTileCount)
            {
                AppendLine(reportBuilder, "[FAIL] Board tile count after restart is "
                    + boardTileCount
                    + ", expected "
                    + expectedBoardTileCount
                    + ".");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Lose failure triggers level restart with clean tray and repopulated board.");
            return true;
        }

        private static bool ValidateTimerExpirationFail(
            SessionDirector sessionDirector,
            Transform gameplayRoot,
            StringBuilder reportBuilder)
        {
            if (!sessionDirector.IsSessionActive)
            {
                AppendLine(reportBuilder, "[FAIL] Session must be active before timer expiration validation.");
                return false;
            }

            Transform timerRoot = gameplayRoot != null ? gameplayRoot.Find("TimerRoot") : null;
            TimerController timerController = timerRoot != null
                ? timerRoot.GetComponent<TimerController>()
                : null;

            if (timerController == null)
            {
                AppendLine(reportBuilder, "[FAIL] TimerController is missing for timer expiration validation.");
                return false;
            }

            int levelBefore = ResolveCurrentLevel();
            int sessionIdBefore = sessionDirector.CurrentSession.SessionId;
            bool penaltyDetected = false;

            void HandlePenaltyDetected(TimerExpirationPenaltyContext context)
            {
                if (context != null)
                {
                    penaltyDetected = true;
                }
            }

            PenaltyEvents.TimerExpirationPenaltyDetected += HandlePenaltyDetected;
            try
            {
                timerController.StopTimer();
                if (!timerController.TryStartTimer(0.05f))
                {
                    AppendLine(reportBuilder, "[FAIL] TryStartTimer failed for timer expiration validation.");
                    return false;
                }

                timerController.AdvanceTimerForValidation(0.06f);

                if (timerController.CurrentState != TimerState.Expired)
                {
                    AppendLine(reportBuilder, "[FAIL] Timer did not reach Expired state during validation.");
                    return false;
                }

                if (!penaltyDetected)
                {
                    AppendLine(reportBuilder, "[FAIL] TimerExpirationPenaltyDetected was not raised on timer expiration.");
                    return false;
                }

                if (!sessionDirector.IsSessionActive)
                {
                    AppendLine(reportBuilder, "[FAIL] Session is not active after timer expiration restart.");
                    return false;
                }

                if (sessionDirector.CurrentSession == null)
                {
                    AppendLine(reportBuilder, "[FAIL] CurrentSession is null after timer expiration restart.");
                    return false;
                }

                if (sessionDirector.CurrentSession.SessionId <= sessionIdBefore)
                {
                    AppendLine(reportBuilder, "[FAIL] Session id did not increase after timer expiration restart.");
                    return false;
                }

                if (ResolveCurrentLevel() != levelBefore)
                {
                    AppendLine(reportBuilder, "[FAIL] Player level changed after timer expiration restart.");
                    return false;
                }

                AppendLine(reportBuilder, "[PASS] Timer expiration raises penalty event and triggers level restart.");
                return true;
            }
            finally
            {
                PenaltyEvents.TimerExpirationPenaltyDetected -= HandlePenaltyDetected;
            }
        }

        private static bool ValidateWinWithoutRestart(SessionDirector sessionDirector, StringBuilder reportBuilder)
        {
            if (!sessionDirector.IsSessionActive)
            {
                AppendLine(reportBuilder, "[FAIL] Session must be active before win validation.");
                return false;
            }

            if (!sessionDirector.TryEndSession(SessionEndReason.Win))
            {
                AppendLine(reportBuilder, "[FAIL] TryEndSession(Win) failed during win validation.");
                return false;
            }

            if (sessionDirector.IsSessionActive)
            {
                AppendLine(reportBuilder, "[FAIL] Session restarted automatically after win.");
                return false;
            }

            if (sessionDirector.CurrentState != LevelSessionState.Ended)
            {
                AppendLine(reportBuilder, "[FAIL] SessionDirector.CurrentState is not Ended after win.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Win ends session without auto-restart.");
            return true;
        }

        private static int ResolveCurrentLevel()
        {
            if (PlayerProgressionDirector.HasInstance)
            {
                return PlayerProgressionDirector.Instance.CurrentLevel;
            }

            return LevelProgressData.MinLevel;
        }

        private static void AppendLine(StringBuilder reportBuilder, string line)
        {
            reportBuilder.AppendLine(line);
        }
    }
}
