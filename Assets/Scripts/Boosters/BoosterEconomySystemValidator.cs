using System.Reflection;
using System.Text;
using UnityEngine;

namespace MahjongGame.Boosters
{
    public static class BoosterEconomySystemValidator
    {
        public static bool Validate(Transform gameplayRoot, StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            if (gameplayRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] GameplayRoot is missing for booster economy validation.");
                return false;
            }

            BoosterEconomyDirector economyDirector = gameplayRoot.GetComponent<BoosterEconomyDirector>();

            passed &= ValidateComponents(economyDirector, reportBuilder);
            passed &= ValidateTypesAndEvents(reportBuilder);
            passed &= ValidateDefinition(reportBuilder);
            passed &= ValidateGrantAndConsumeBehavior(economyDirector, reportBuilder);
            passed &= ValidateProgressionRewardBehavior(economyDirector, reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Booster economy validation completed successfully."
                : "[FAIL] Booster economy validation found issues.");

            return passed;
        }

        private static bool ValidateComponents(
            BoosterEconomyDirector economyDirector,
            StringBuilder reportBuilder)
        {
            if (economyDirector == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoosterEconomyDirector is missing on GameplayRoot.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] BoosterEconomyDirector is present on GameplayRoot.");
            return true;
        }

        private static bool ValidateTypesAndEvents(StringBuilder reportBuilder)
        {
            bool passed = true;

            AppendLine(reportBuilder, typeof(BoosterType) != null
                ? "[PASS] BoosterType type is present."
                : "[FAIL] BoosterType type is missing.");

            AppendLine(reportBuilder, typeof(BoosterDefinition) != null
                ? "[PASS] BoosterDefinition type is present."
                : "[FAIL] BoosterDefinition type is missing.");

            AppendLine(reportBuilder, typeof(BoosterCounts) != null
                ? "[PASS] BoosterCounts type is present."
                : "[FAIL] BoosterCounts type is missing.");

            passed &= ValidateEventExists(
                typeof(BoosterEvents),
                nameof(BoosterEvents.BoosterCountsChanged),
                reportBuilder);
            passed &= ValidateEventExists(
                typeof(BoosterEvents),
                nameof(BoosterEvents.BoosterProgressionRewardGranted),
                reportBuilder);
            passed &= ValidateEventExists(
                typeof(BoosterEvents),
                nameof(BoosterEvents.BoosterRuntimeReset),
                reportBuilder);

            return passed;
        }

        private static bool ValidateDefinition(StringBuilder reportBuilder)
        {
            bool passed = true;

            if (BoosterDefinition.ProgressionIntervalLevels != 10)
            {
                AppendLine(reportBuilder, "[FAIL] BoosterDefinition progression interval is not 10 levels.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] BoosterDefinition progression interval is 10 levels.");
            }

            if (BoosterDefinition.RewardsPerMilestone != 1)
            {
                AppendLine(reportBuilder, "[FAIL] BoosterDefinition rewards per milestone is not 1.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] BoosterDefinition rewards per milestone is 1.");
            }

            if (BoosterDefinition.StartingShuffleCount != 0
                || BoosterDefinition.StartingUndoCount != 0
                || BoosterDefinition.StartingHintCount != 0)
            {
                AppendLine(reportBuilder, "[FAIL] BoosterDefinition starting counts are not zero.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] BoosterDefinition starting counts are zero.");
            }

            return passed;
        }

        private static bool ValidateGrantAndConsumeBehavior(
            BoosterEconomyDirector economyDirector,
            StringBuilder reportBuilder)
        {
            if (economyDirector == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoosterEconomyDirector is unavailable for grant/consume validation.");
                return false;
            }

            economyDirector.ResetCountsForValidation();

            if (economyDirector.GetCount(BoosterType.Shuffle) != 0
                || economyDirector.GetCount(BoosterType.Undo) != 0
                || economyDirector.GetCount(BoosterType.Hint) != 0)
            {
                AppendLine(reportBuilder, "[FAIL] Booster counts are not zero after reset.");
                return false;
            }

            if (!economyDirector.TryGrant(BoosterType.Shuffle, 1)
                || economyDirector.GetCount(BoosterType.Shuffle) != 1)
            {
                AppendLine(reportBuilder, "[FAIL] Booster grant validation failed for shuffle.");
                return false;
            }

            if (!economyDirector.TryConsume(BoosterType.Shuffle)
                || economyDirector.GetCount(BoosterType.Shuffle) != 0)
            {
                AppendLine(reportBuilder, "[FAIL] Booster consume validation failed for shuffle.");
                return false;
            }

            if (economyDirector.TryConsume(BoosterType.Undo))
            {
                AppendLine(reportBuilder, "[FAIL] Booster consume succeeded with zero undo count.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Booster grant and consume behave correctly.");
            return true;
        }

        private static bool ValidateProgressionRewardBehavior(
            BoosterEconomyDirector economyDirector,
            StringBuilder reportBuilder)
        {
            if (economyDirector == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoosterEconomyDirector is unavailable for progression validation.");
                return false;
            }

            bool progressionRewardGranted = false;
            BoosterEvents.BoosterProgressionRewardGranted += HandleProgressionRewardGranted;

            try
            {
                economyDirector.ResetCountsForValidation();

                if (economyDirector.TryApplyProgressionRewardForValidation(8))
                {
                    AppendLine(reportBuilder, "[FAIL] Progression reward granted before milestone.");
                    return false;
                }

                if (!economyDirector.TryApplyProgressionRewardForValidation(9))
                {
                    AppendLine(reportBuilder, "[FAIL] Progression reward was not granted at milestone.");
                    return false;
                }

                if (!progressionRewardGranted)
                {
                    AppendLine(reportBuilder, "[FAIL] BoosterProgressionRewardGranted event was not raised.");
                    return false;
                }

                if (economyDirector.GetCount(BoosterType.Shuffle) != 1
                    || economyDirector.GetCount(BoosterType.Undo) != 1
                    || economyDirector.GetCount(BoosterType.Hint) != 1)
                {
                    AppendLine(reportBuilder, "[FAIL] Milestone booster rewards are incorrect.");
                    return false;
                }

                if (economyDirector.TotalLevelsCompleted != 10)
                {
                    AppendLine(reportBuilder, "[FAIL] Total levels completed is incorrect after milestone win.");
                    return false;
                }
            }
            finally
            {
                BoosterEvents.BoosterProgressionRewardGranted -= HandleProgressionRewardGranted;
                economyDirector.ResetCountsForValidation();
            }

            AppendLine(reportBuilder, "[PASS] Booster progression rewards behave correctly.");
            return true;

            void HandleProgressionRewardGranted(BoosterProgressionRewardGrantedContext context)
            {
                progressionRewardGranted = context != null && context.TotalLevelsCompleted == 10;
            }
        }

        private static bool ValidateEventExists(
            System.Type eventsType,
            string eventName,
            StringBuilder reportBuilder)
        {
            EventInfo eventInfo = eventsType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Static);
            if (eventInfo == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoosterEvents." + eventName + " event is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] BoosterEvents." + eventName + " event is present.");
            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string line)
        {
            reportBuilder.AppendLine(line);
        }
    }
}
