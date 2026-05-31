using MahjongGame.Progression;
using UnityEngine;

namespace MahjongGame.BoardGeneration
{
    public sealed class LevelRecipeGenerator : MonoBehaviour
    {
        private static LevelRecipeGenerator _instance;

        public static LevelRecipeGenerator Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[LevelRecipeGenerator] Instance is not available.");
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

        public LevelRecipe GenerateRecipe(int levelNumber)
        {
            DifficultyProfile difficultyProfile = ResolveDifficultyProfile(levelNumber);
            VisualVarietyProfile visualVarietyProfile = ResolveVisualVarietyProfile(levelNumber);
            return LevelRecipeDefinition.GenerateRecipe(levelNumber, difficultyProfile, visualVarietyProfile);
        }

        public LevelRecipe GenerateRecipeForCurrentLevel()
        {
            int levelNumber = PlayerProgressionDirector.HasInstance
                ? PlayerProgressionDirector.Instance.CurrentLevel
                : LevelProgressData.MinLevel;

            return GenerateRecipe(levelNumber);
        }

        public bool TryGenerateRecipe(int levelNumber, out LevelRecipe recipe)
        {
            recipe = GenerateRecipe(levelNumber);
            return recipe != null;
        }

        private static DifficultyProfile ResolveDifficultyProfile(int levelNumber)
        {
            if (DifficultyDirector.HasInstance)
            {
                return DifficultyDirector.Instance.ResolveProfile(levelNumber);
            }

            return DifficultyDefinition.ResolveProfile(levelNumber);
        }

        private static VisualVarietyProfile ResolveVisualVarietyProfile(int levelNumber)
        {
            if (VisualVarietyDirector.HasInstance)
            {
                return VisualVarietyDirector.Instance.ResolveProfile(levelNumber);
            }

            return VisualVarietyDefinition.ResolveProfile(levelNumber);
        }
    }
}
