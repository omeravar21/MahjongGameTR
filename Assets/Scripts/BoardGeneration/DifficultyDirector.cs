using MahjongGame.Progression;
using UnityEngine;

namespace MahjongGame.BoardGeneration
{
    public sealed class DifficultyDirector : MonoBehaviour
    {
        private static DifficultyDirector _instance;

        public static DifficultyDirector Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[DifficultyDirector] Instance is not available.");
                }

                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public DifficultyProfile ResolveProfile(int levelNumber)
        {
            return DifficultyDefinition.ResolveProfile(levelNumber);
        }

        public DifficultyProfile ResolveProfileForCurrentLevel()
        {
            int levelNumber = PlayerProgressionDirector.HasInstance
                ? PlayerProgressionDirector.Instance.CurrentLevel
                : LevelProgressData.MinLevel;

            return ResolveProfile(levelNumber);
        }

        public bool TryResolveProfile(int levelNumber, out DifficultyProfile profile)
        {
            profile = DifficultyDefinition.ResolveProfile(levelNumber);
            return profile != null;
        }

        public bool TryResolveProfileForCurrentLevel(out DifficultyProfile profile)
        {
            if (!PlayerProgressionDirector.HasInstance)
            {
                Debug.LogWarning("[DifficultyDirector] PlayerProgressionDirector is not available.");
                profile = null;
                return false;
            }

            profile = ResolveProfile(PlayerProgressionDirector.Instance.CurrentLevel);
            return profile != null;
        }

        public static bool HasDifficultyScaled(DifficultyProfile previous, DifficultyProfile next)
        {
            if (previous == null || next == null)
            {
                return false;
            }

            if (next.LevelNumber <= previous.LevelNumber)
            {
                return false;
            }

            if (next.TileCount < previous.TileCount
                || next.ClosedTileCount < previous.ClosedTileCount
                || next.JokerCount < previous.JokerCount
                || next.LayerDepth < previous.LayerDepth
                || next.RecommendedTimerSeconds < previous.RecommendedTimerSeconds - 0.001f)
            {
                return false;
            }

            return true;
        }
    }
}
