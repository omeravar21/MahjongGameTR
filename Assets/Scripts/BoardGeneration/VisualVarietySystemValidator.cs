using System.Collections.Generic;
using System.Text;
using MahjongGame.Progression;

namespace MahjongGame.BoardGeneration
{
    public static class VisualVarietySystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateDeterministicResolution(reportBuilder);
            passed &= ValidateArchetypeRotation(reportBuilder);
            passed &= ValidatePatternRotation(reportBuilder);
            passed &= ValidateLaunchCatalogCoverage(reportBuilder);
            passed &= ValidateRepetitionReduction(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Visual variety system validation completed successfully."
                : "[FAIL] Visual variety system validation found issues.");

            return passed;
        }

        private static bool ValidateDeterministicResolution(StringBuilder reportBuilder)
        {
            VisualVarietyProfile first = VisualVarietyDefinition.ResolveProfile(25);
            VisualVarietyProfile second = VisualVarietyDefinition.ResolveProfile(25);

            if (!ProfilesEqual(first, second))
            {
                AppendLine(reportBuilder, "[FAIL] Level 25 does not resolve a stable visual variety profile.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Visual variety profiles are deterministic per level.");
            return true;
        }

        private static bool ValidateArchetypeRotation(StringBuilder reportBuilder)
        {
            VisualVarietyProfile levelOne = VisualVarietyDefinition.ResolveProfile(1);
            VisualVarietyProfile levelTwo = VisualVarietyDefinition.ResolveProfile(2);

            if (levelOne.ArchetypeId == levelTwo.ArchetypeId)
            {
                AppendLine(reportBuilder, "[FAIL] Levels 1 and 2 resolved the same archetype.");
                return false;
            }

            HashSet<BoardArchetypeId> archetypes = new HashSet<BoardArchetypeId>();
            for (int level = 1; level <= VisualVarietyDefinition.LaunchArchetypeCount; level++)
            {
                archetypes.Add(VisualVarietyDefinition.ResolveProfile(level).ArchetypeId);
            }

            if (archetypes.Count != VisualVarietyDefinition.LaunchArchetypeCount)
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] Launch archetype rotation did not cover all "
                    + VisualVarietyDefinition.LaunchArchetypeCount
                    + " launch archetypes in the first band.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Archetype rotation covers launch archetypes without immediate repetition.");
            return true;
        }

        private static bool ValidatePatternRotation(StringBuilder reportBuilder)
        {
            VisualVarietyProfile levelOne = VisualVarietyDefinition.ResolveProfile(1);
            VisualVarietyProfile levelTwo = VisualVarietyDefinition.ResolveProfile(2);

            if (levelOne.HolePatternId == levelTwo.HolePatternId
                && levelOne.ClosedTilePatternId == levelTwo.ClosedTilePatternId
                && levelOne.VariationIndex == levelTwo.VariationIndex)
            {
                AppendLine(reportBuilder, "[FAIL] Levels 1 and 2 resolved identical pattern selections.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Pattern and variation rotation produces distinct early-level selections.");
            return true;
        }

        private static bool ValidateLaunchCatalogCoverage(StringBuilder reportBuilder)
        {
            if (VisualVarietyDefinition.LaunchArchetypeCount != 8
                || VisualVarietyDefinition.LaunchHolePatternCount != 6
                || VisualVarietyDefinition.LaunchClosedTilePatternCount != 14)
            {
                AppendLine(reportBuilder, "[FAIL] Launch catalog counts do not match documented launch scope.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Launch catalog counts match documented launch scope.");
            return true;
        }

        private static bool ValidateRepetitionReduction(StringBuilder reportBuilder)
        {
            HashSet<string> signatures = new HashSet<string>();
            for (int level = LevelProgressData.MinLevel; level <= 24; level++)
            {
                VisualVarietyProfile profile = VisualVarietyDefinition.ResolveProfile(level);
                string signature = profile.ArchetypeId
                    + ":"
                    + profile.VariationIndex
                    + ":"
                    + profile.HolePatternId
                    + ":"
                    + profile.ClosedTilePatternId;

                if (!signatures.Add(signature))
                {
                    AppendLine(reportBuilder, "[FAIL] Duplicate visual variety signature detected before level 25 at level " + level + ".");
                    return false;
                }
            }

            AppendLine(reportBuilder, "[PASS] Visual variety signatures remain distinct across levels 1-24.");
            return true;
        }

        private static bool ProfilesEqual(VisualVarietyProfile left, VisualVarietyProfile right)
        {
            return left.LevelNumber == right.LevelNumber
                && left.DeterministicSeed == right.DeterministicSeed
                && left.ArchetypeId == right.ArchetypeId
                && left.VariationIndex == right.VariationIndex
                && left.HolePatternId == right.HolePatternId
                && left.ClosedTilePatternId == right.ClosedTilePatternId;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
