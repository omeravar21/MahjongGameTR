using MahjongGame.Board;
using MahjongGame.Matching;
using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Session
{
    public sealed class WinConditionController : MonoBehaviour
    {
        [SerializeField] private TrayController trayController;

        private void OnEnable()
        {
            MatchEvents.MatchCleanedUp += HandleMatchCleanedUp;
        }

        private void OnDisable()
        {
            MatchEvents.MatchCleanedUp -= HandleMatchCleanedUp;
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

            TrayController resolvedTrayController = ResolveTrayController();
            Transform boardRoot = BoardTileOccupancyQuery.ResolveBoardRootFromScene();
            if (!LevelCompletionQuery.IsLevelComplete(boardRoot, resolvedTrayController))
            {
                return;
            }

            if (SessionDirector.Instance.TryEndSession(SessionEndReason.Win))
            {
                Debug.Log("[WinConditionController] Level complete — session ended with Win.");
            }
        }

        private TrayController ResolveTrayController()
        {
            if (trayController != null)
            {
                return trayController;
            }

            trayController = GetComponent<TrayController>();
            return trayController;
        }
    }
}
