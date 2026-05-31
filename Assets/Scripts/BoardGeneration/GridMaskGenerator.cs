using MahjongGame.Progression;
using UnityEngine;

namespace MahjongGame.BoardGeneration
{
    public sealed class GridMaskGenerator : MonoBehaviour
    {
        private static GridMaskGenerator _instance;

        public static GridMaskGenerator Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[GridMaskGenerator] Instance is not available.");
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

        public GridMask GenerateMask(LevelRecipe recipe)
        {
            return GridMaskDefinition.GenerateFromRecipe(recipe);
        }

        public GridMask GenerateMask(int levelNumber)
        {
            LevelRecipe recipe = ResolveLevelRecipe(levelNumber);
            return GenerateMask(recipe);
        }

        public GridMask GenerateMaskForCurrentLevel()
        {
            int levelNumber = PlayerProgressionDirector.HasInstance
                ? PlayerProgressionDirector.Instance.CurrentLevel
                : LevelProgressData.MinLevel;

            return GenerateMask(levelNumber);
        }

        public bool TryGenerateMask(int levelNumber, out GridMask gridMask)
        {
            gridMask = GenerateMask(levelNumber);
            return gridMask != null;
        }

        private static LevelRecipe ResolveLevelRecipe(int levelNumber)
        {
            if (LevelRecipeGenerator.HasInstance)
            {
                return LevelRecipeGenerator.Instance.GenerateRecipe(levelNumber);
            }

            return LevelRecipeDefinition.GenerateRecipe(levelNumber);
        }
    }
}
