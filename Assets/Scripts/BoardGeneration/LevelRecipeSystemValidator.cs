using System.Text;
using MahjongGame.Progression;

namespace MahjongGame.BoardGeneration
{
    public static class LevelRecipeSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateDeterministicRecipe(reportBuilder);
            passed &= ValidateSampleLevelRecipe(1, reportBuilder);
            passed &= ValidateSampleLevelRecipe(10, reportBuilder);
            passed &= ValidateSampleLevelRecipe(50, reportBuilder);
            passed &= ValidateSampleLevelRecipe(300, reportBuilder);
            passed &= ValidateSampleLevelRecipe(1000, reportBuilder);
            passed &= ValidateDifficultyAndVisualInputs(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Level recipe system validation completed successfully."
                : "[FAIL] Level recipe system validation found issues.");

            return passed;
        }

        private static bool ValidateDeterministicRecipe(StringBuilder reportBuilder)
        {
            LevelRecipe first = LevelRecipeDefinition.GenerateRecipe(42);
            LevelRecipe second = LevelRecipeDefinition.GenerateRecipe(42);

            if (!RecipesEqual(first, second))
            {
                AppendLine(reportBuilder, "[FAIL] Level 42 does not resolve a stable level recipe.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Level recipes are deterministic per level.");
            return true;
        }

        private static bool ValidateSampleLevelRecipe(int levelNumber, StringBuilder reportBuilder)
        {
            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(levelNumber);
            DifficultyProfile difficultyProfile = DifficultyDefinition.ResolveProfile(levelNumber);
            VisualVarietyProfile visualVarietyProfile = VisualVarietyDefinition.ResolveProfile(levelNumber);

            if (recipe == null)
            {
                AppendLine(reportBuilder, "[FAIL] Level " + levelNumber + " did not resolve a recipe.");
                return false;
            }

            if (recipe.LevelNumber != levelNumber
                || recipe.Seed != visualVarietyProfile.DeterministicSeed
                || recipe.TileCount != difficultyProfile.TileCount
                || recipe.LayerDepth != difficultyProfile.LayerDepth
                || recipe.ClosedTileCount != difficultyProfile.ClosedTileCount
                || recipe.JokerCount != difficultyProfile.JokerCount
                || recipe.ArchetypeId != visualVarietyProfile.ArchetypeId
                || recipe.VariationIndex != visualVarietyProfile.VariationIndex
                || recipe.HolePatternId != visualVarietyProfile.HolePatternId
                || recipe.ClosedTilePatternId != visualVarietyProfile.ClosedTilePatternId
                || recipe.RecommendedTimerSeconds != difficultyProfile.RecommendedTimerSeconds
                || recipe.MaxRegenerationAttempts != LevelRecipeDefinition.MaxRegenerationAttempts
                || recipe.DifficultyRating <= 0f)
            {
                AppendLine(reportBuilder, "[FAIL] Level " + levelNumber + " recipe failed field checks.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Level " + levelNumber + " recipe is valid.");
            return true;
        }

        private static bool ValidateDifficultyAndVisualInputs(StringBuilder reportBuilder)
        {
            LevelRecipe levelOne = LevelRecipeDefinition.GenerateRecipe(1);
            LevelRecipe levelTwo = LevelRecipeDefinition.GenerateRecipe(2);

            if (levelOne.ArchetypeId == levelTwo.ArchetypeId && levelOne.Seed == levelTwo.Seed)
            {
                AppendLine(reportBuilder, "[FAIL] Early level recipes did not vary across difficulty and visual inputs.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Recipes combine difficulty and visual variety inputs correctly.");
            return true;
        }

        private static bool RecipesEqual(LevelRecipe left, LevelRecipe right)
        {
            return left.LevelNumber == right.LevelNumber
                && left.Seed == right.Seed
                && left.TileCount == right.TileCount
                && left.LayerDepth == right.LayerDepth
                && left.ArchetypeId == right.ArchetypeId
                && left.VariationIndex == right.VariationIndex
                && left.HolePatternId == right.HolePatternId
                && left.ClosedTileCount == right.ClosedTileCount
                && left.ClosedTilePatternId == right.ClosedTilePatternId
                && left.JokerCount == right.JokerCount
                && left.RewardJokerPatternId == right.RewardJokerPatternId
                && left.RecommendedTimerSeconds == right.RecommendedTimerSeconds
                && left.DifficultyRating == right.DifficultyRating
                && left.MaxRegenerationAttempts == right.MaxRegenerationAttempts;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
