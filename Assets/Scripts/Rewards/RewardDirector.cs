using MahjongGame.Session;
using UnityEngine;

namespace MahjongGame.Rewards
{
    public sealed class RewardDirector : MonoBehaviour
    {
        [SerializeField] private JokerTileController jokerTileController;

        public int RegisteredJokerTileCount =>
            jokerTileController != null ? jokerTileController.GetRegisteredJokerTileCount() : 0;

        private void Awake()
        {
            ResolveJokerTileController();
        }

        private void OnEnable()
        {
            SessionEvents.SessionStarted += HandleSessionStarted;
        }

        private void OnDisable()
        {
            SessionEvents.SessionStarted -= HandleSessionStarted;
        }

        public JokerTileController GetJokerTileController()
        {
            ResolveJokerTileController();
            return jokerTileController;
        }

        public void ResetJokerRuntimeState()
        {
            if (jokerTileController != null)
            {
                jokerTileController.ResetRuntimeState();
            }
        }

        private void ResolveJokerTileController()
        {
            if (jokerTileController == null)
            {
                jokerTileController = GetComponent<JokerTileController>();
            }
        }

        private void HandleSessionStarted(SessionStartedContext context)
        {
            ResetJokerRuntimeState();
        }
    }
}
