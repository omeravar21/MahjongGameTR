using MahjongGame.Progression;
using UnityEngine;

namespace MahjongGame.BoardGeneration
{
    public sealed class VisualVarietyDirector : MonoBehaviour
    {
        private static VisualVarietyDirector _instance;

        public static VisualVarietyDirector Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[VisualVarietyDirector] Instance is not available.");
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

        public VisualVarietyProfile ResolveProfile(int levelNumber)
        {
            return VisualVarietyDefinition.ResolveProfile(levelNumber);
        }

        public VisualVarietyProfile ResolveProfileForCurrentLevel()
        {
            int levelNumber = PlayerProgressionDirector.HasInstance
                ? PlayerProgressionDirector.Instance.CurrentLevel
                : LevelProgressData.MinLevel;

            return ResolveProfile(levelNumber);
        }

        public bool TryResolveProfile(int levelNumber, out VisualVarietyProfile profile)
        {
            profile = VisualVarietyDefinition.ResolveProfile(levelNumber);
            return profile != null;
        }
    }
}
