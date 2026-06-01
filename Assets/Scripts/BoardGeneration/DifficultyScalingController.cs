using MahjongGame.Progression;
using MahjongGame.Session;
using UnityEngine;

namespace MahjongGame.BoardGeneration
{
    public sealed class DifficultyScalingController : MonoBehaviour
    {
        private void OnEnable()
        {
            SessionEvents.SessionStarted += HandleSessionStarted;
        }

        private void OnDisable()
        {
            SessionEvents.SessionStarted -= HandleSessionStarted;
        }

        private void HandleSessionStarted(SessionStartedContext context)
        {
            if (context == null || context.IsResumeSession)
            {
                return;
            }

            int levelNumber = context.LevelNumber;
            if (levelNumber <= 0)
            {
                Debug.LogWarning("[DifficultyScalingController] Session started without a valid level number.");
                return;
            }

            if (PlayerProgressionDirector.HasInstance)
            {
                int currentLevel = PlayerProgressionDirector.Instance.CurrentLevel;
                if (levelNumber != currentLevel)
                {
                    Debug.LogWarning(
                        "[DifficultyScalingController] Session level "
                        + levelNumber
                        + " does not match current progression level "
                        + currentLevel
                        + ".");
                }
            }
            else
            {
                Debug.LogWarning("[DifficultyScalingController] PlayerProgressionDirector is not available.");
            }

            DifficultyProfile profile = ResolveProfile(levelNumber);
            if (profile == null)
            {
                Debug.LogWarning(
                    "[DifficultyScalingController] Could not resolve difficulty profile for level "
                    + levelNumber
                    + ".");
                return;
            }

            Debug.Log(
                "[DifficultyScalingController] Difficulty profile for level "
                + profile.LevelNumber
                + ": tiles="
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

        private static DifficultyProfile ResolveProfile(int levelNumber)
        {
            if (DifficultyDirector.HasInstance)
            {
                return DifficultyDirector.Instance.ResolveProfile(levelNumber);
            }

            return DifficultyDefinition.ResolveProfile(levelNumber);
        }
    }
}
