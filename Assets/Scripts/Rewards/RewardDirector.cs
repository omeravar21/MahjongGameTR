using MahjongGame.Matching;
using MahjongGame.Session;
using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.Rewards
{
    public sealed class RewardDirector : MonoBehaviour
    {
        [SerializeField] private JokerTileController jokerTileController;
        [SerializeField] private JokerTimerController jokerTimerController;

        private int _earlyJokerMatchCount;
        private int _lateJokerMatchCount;

        public int RegisteredJokerTileCount =>
            jokerTileController != null ? jokerTileController.GetRegisteredJokerTileCount() : 0;

        public int EarlyJokerMatchCount => _earlyJokerMatchCount;

        public int LateJokerMatchCount => _lateJokerMatchCount;

        private void Awake()
        {
            ResolveComponents();
        }

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

        public JokerTileController GetJokerTileController()
        {
            ResolveComponents();
            return jokerTileController;
        }

        public JokerTimerController GetJokerTimerController()
        {
            ResolveComponents();
            return jokerTimerController;
        }

        public void ResetJokerRuntimeState()
        {
            _earlyJokerMatchCount = 0;
            _lateJokerMatchCount = 0;

            if (jokerTileController != null)
            {
                jokerTileController.ResetRuntimeState();
            }
        }

        internal bool TryEvaluateJokerMatchForValidation(int jokerTileId, float elapsedSessionSeconds)
        {
            if (jokerTileId < 0)
            {
                return false;
            }

            if (elapsedSessionSeconds <= JokerDefinition.EarlyMatchWindowSeconds)
            {
                _earlyJokerMatchCount++;
                JokerEvents.RaiseJokerEarlyMatchDetected(
                    new JokerEarlyMatchDetectedContext(jokerTileId, elapsedSessionSeconds));
                return true;
            }

            _lateJokerMatchCount++;
            JokerEvents.RaiseJokerLateMatchDetected(
                new JokerLateMatchDetectedContext(jokerTileId, elapsedSessionSeconds));
            return true;
        }

        private void ResolveComponents()
        {
            if (jokerTileController == null)
            {
                jokerTileController = GetComponent<JokerTileController>();
            }

            if (jokerTimerController == null)
            {
                jokerTimerController = GetComponent<JokerTimerController>();
            }
        }

        private void HandleSessionStarted(SessionStartedContext context)
        {
            if (context == null || context.IsResumeSession)
            {
                return;
            }

            ResetJokerRuntimeState();
        }

        private void HandleMatchCleanedUp(MatchCleanupContext context)
        {
            if (context == null || context.Request == null)
            {
                return;
            }

            if (!SessionDirector.HasInstance || !SessionDirector.Instance.IsSessionActive)
            {
                return;
            }

            int jokerTileId = ResolveMatchedJokerTileId(context.Request.FirstTile, context.Request.SecondTile);
            if (jokerTileId < 0)
            {
                return;
            }

            float elapsedSessionSeconds = jokerTimerController != null
                ? jokerTimerController.ElapsedSessionSeconds
                : float.MaxValue;

            jokerTileController.TryClearJokerTile(jokerTileId);

            if (elapsedSessionSeconds <= JokerDefinition.EarlyMatchWindowSeconds)
            {
                _earlyJokerMatchCount++;
                JokerEvents.RaiseJokerEarlyMatchDetected(
                    new JokerEarlyMatchDetectedContext(jokerTileId, elapsedSessionSeconds));
                return;
            }

            _lateJokerMatchCount++;
            JokerEvents.RaiseJokerLateMatchDetected(
                new JokerLateMatchDetectedContext(jokerTileId, elapsedSessionSeconds));
        }

        private int ResolveMatchedJokerTileId(Tile firstTile, Tile secondTile)
        {
            if (jokerTileController == null)
            {
                return -1;
            }

            if (firstTile != null && jokerTileController.IsJokerTile(firstTile.TileId))
            {
                return firstTile.TileId;
            }

            if (secondTile != null && jokerTileController.IsJokerTile(secondTile.TileId))
            {
                return secondTile.TileId;
            }

            return -1;
        }
    }
}
