using MahjongGame.Progression;

namespace MahjongGame.BoardGeneration
{
    public static class VisualVarietyDefinition
    {
        public const int LaunchArchetypeCount = 8;
        public const int VariationsPerArchetype = 4;
        public const int LaunchHolePatternCount = 6;
        public const int LaunchClosedTilePatternCount = 14;

        private static readonly BoardArchetypeId[] LaunchArchetypeOrder =
        {
            BoardArchetypeId.Diamond,
            BoardArchetypeId.Oval,
            BoardArchetypeId.Pyramid,
            BoardArchetypeId.Tower,
            BoardArchetypeId.Cross,
            BoardArchetypeId.Bridge,
            BoardArchetypeId.Island,
            BoardArchetypeId.Maze
        };

        public static VisualVarietyProfile ResolveProfile(int levelNumber)
        {
            int clampedLevel = LevelProgressData.ClampLevel(levelNumber);
            int deterministicSeed = ComputeDeterministicSeed(clampedLevel);

            int archetypeIndex = PositiveMod(clampedLevel - 1, LaunchArchetypeCount);
            BoardArchetypeId archetypeId = LaunchArchetypeOrder[archetypeIndex];

            int variationIndex = PositiveMod(
                (clampedLevel - 1) / LaunchArchetypeCount + archetypeIndex,
                VariationsPerArchetype);

            int holePatternIndex = PositiveMod(
                (clampedLevel * 7) + (archetypeIndex * 3) + variationIndex,
                LaunchHolePatternCount);
            HolePatternId holePatternId = (HolePatternId)holePatternIndex;

            int closedPatternIndex = PositiveMod(
                (clampedLevel * 11) + (variationIndex * 5) + holePatternIndex,
                LaunchClosedTilePatternCount);
            ClosedTilePatternId closedTilePatternId = (ClosedTilePatternId)closedPatternIndex;

            return new VisualVarietyProfile(
                clampedLevel,
                deterministicSeed,
                archetypeId,
                variationIndex,
                holePatternId,
                closedTilePatternId);
        }

        public static int ComputeDeterministicSeed(int levelNumber)
        {
            unchecked
            {
                return (levelNumber * 73856093) ^ 19349663;
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
