using MahjongGame.BoardGeneration;

namespace MahjongGame.DailyBoard
{
    public static class DailyBoardRecipeDefinition
    {
        public static LevelRecipe GenerateRecipe(int dayId, int dailySeed)
        {
            DifficultyProfile difficultyProfile = DifficultyDefinition.ResolveProfile(
                DailyBoardRulesDefinition.ReferenceDifficultyLevel);
            VisualVarietyProfile visualVarietyProfile = ResolveVisualVarietyFromDailySeed(dailySeed);

            LevelRecipe recipe = LevelRecipeDefinition.GenerateRecipe(
                DailyBoardDefinition.DailySessionLevelNumber,
                difficultyProfile,
                visualVarietyProfile);

            return LevelRecipeDefinition.CreateWithSeed(recipe, dailySeed);
        }

        internal static VisualVarietyProfile ResolveVisualVarietyFromDailySeed(int dailySeed)
        {
            int seed = dailySeed == 0 ? 1 : dailySeed;

            int archetypeIndex = PositiveMod(seed, VisualVarietyDefinition.LaunchArchetypeCount);
            BoardArchetypeId archetypeId = ResolveArchetypeId(archetypeIndex);

            int variationIndex = PositiveMod(
                (seed / VisualVarietyDefinition.LaunchArchetypeCount) + archetypeIndex,
                VisualVarietyDefinition.VariationsPerArchetype);

            int holePatternIndex = PositiveMod(
                (seed * 7) + (archetypeIndex * 3) + variationIndex,
                VisualVarietyDefinition.LaunchHolePatternCount);
            HolePatternId holePatternId = (HolePatternId)holePatternIndex;

            int closedPatternIndex = PositiveMod(
                (seed * 11) + (variationIndex * 5) + holePatternIndex,
                VisualVarietyDefinition.LaunchClosedTilePatternCount);
            ClosedTilePatternId closedTilePatternId = (ClosedTilePatternId)closedPatternIndex;

            return new VisualVarietyProfile(
                DailyBoardDefinition.DailySessionLevelNumber,
                seed,
                archetypeId,
                variationIndex,
                holePatternId,
                closedTilePatternId);
        }

        private static BoardArchetypeId ResolveArchetypeId(int archetypeIndex)
        {
            switch (archetypeIndex)
            {
                case 0: return BoardArchetypeId.Diamond;
                case 1: return BoardArchetypeId.Oval;
                case 2: return BoardArchetypeId.Pyramid;
                case 3: return BoardArchetypeId.Tower;
                case 4: return BoardArchetypeId.Cross;
                case 5: return BoardArchetypeId.Bridge;
                case 6: return BoardArchetypeId.Island;
                default: return BoardArchetypeId.Maze;
            }
        }

        private static int PositiveMod(int value, int modulus)
        {
            if (modulus <= 0)
            {
                return 0;
            }

            int result = value % modulus;
            return result < 0 ? result + modulus : result;
        }
    }
}
