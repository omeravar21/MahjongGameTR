using System;
using System.Text;
using MahjongGame.Core;
using MahjongGame.Core.Save;
using MahjongGame.DailyBoard;
using MahjongGame.DailyMissions;
using MahjongGame.Ranking;
using UnityEngine;

namespace MahjongGame.DailyRewards
{
    public static class DailyRewardSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateTypes(reportBuilder);
            passed &= ValidateMissionRewardDefinitions(reportBuilder);
            passed &= ValidateDailyBoardRewardDefinitions(reportBuilder);
            passed &= ValidateMissionRewardGrant(reportBuilder);
            passed &= ValidateDailyBoardRewardGrant(reportBuilder);
            passed &= ValidateProgressionIsolation(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Daily reward validation completed successfully."
                : "[FAIL] Daily reward validation found issues.");

            return passed;
        }

        private static bool ValidateTypes(StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= ValidateTypeExists(typeof(DailyRewardDirector), reportBuilder);
            passed &= ValidateTypeExists(typeof(DailyMissionRewardController), reportBuilder);
            passed &= ValidateTypeExists(typeof(DailyMissionRewardDefinition), reportBuilder);

            return passed;
        }

        private static bool ValidateMissionRewardDefinitions(StringBuilder reportBuilder)
        {
            bool passed = true;

            if (DailyMissionRewardDefinition.GetGlobalPerformanceScore(DailyMissionTier.Easy) != 500
                || DailyMissionRewardDefinition.GetGlobalPerformanceScore(DailyMissionTier.Medium) != 1000
                || DailyMissionRewardDefinition.GetGlobalPerformanceScore(DailyMissionTier.Hard) != 1500)
            {
                AppendLine(reportBuilder, "[FAIL] Mission GPS reward tiers are incorrect.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Mission GPS reward tiers are defined.");
            }

            if (DailyMissionRewardDefinition.GetHintReward(DailyMissionTier.Easy) != 1
                || DailyMissionRewardDefinition.GetShuffleReward(DailyMissionTier.Medium) != 1
                || DailyMissionRewardDefinition.GetHintReward(DailyMissionTier.Hard) != 2)
            {
                AppendLine(reportBuilder, "[FAIL] Mission booster reward tiers are incorrect.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Mission booster reward tiers are defined.");
            }

            return passed;
        }

        private static bool ValidateDailyBoardRewardDefinitions(StringBuilder reportBuilder)
        {
            if (DailyBoardRewardDefinition.GetCompletionShuffleBoosterReward() <= 0
                || DailyBoardRewardDefinition.GetCompletionUndoBoosterReward() <= 0
                || DailyBoardRewardDefinition.GetCompletionHintBoosterReward() <= 0)
            {
                AppendLine(reportBuilder, "[FAIL] Daily board booster rewards are not configured.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Daily board booster rewards are configured.");
            return true;
        }

        private static bool ValidateMissionRewardGrant(StringBuilder reportBuilder)
        {
            if (SaveSystem.HasInstance || RankingDirector.HasInstance)
            {
                AppendLine(
                    reportBuilder,
                    "[SKIP] Mission reward grant requires an isolated editor session.");
                return true;
            }

            GameObject validationObject = CreateValidationRoot();
            if (validationObject == null)
            {
                AppendLine(reportBuilder, "[FAIL] Mission reward validation root could not be created.");
                return false;
            }

            bool passed = true;

            try
            {
                RankingDirector rankingDirector = validationObject.GetComponent<RankingDirector>();
                DailyRewardDirector rewardDirector = validationObject.GetComponent<DailyRewardDirector>();
                DailyMissionDirector missionDirector = validationObject.GetComponent<DailyMissionDirector>();

                long gpsBefore = rankingDirector.GlobalPerformanceScore;
                int hintBefore = SaveSystem.Instance.Data.boosterCounts.hint;

                if (!rewardDirector.TryGrantMissionCompletionRewards(DailyMissionTier.Easy))
                {
                    AppendLine(reportBuilder, "[FAIL] Mission completion rewards could not be granted.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] Mission completion rewards grant GPS and boosters.");
                }

                long gpsAfterFirst = rankingDirector.GlobalPerformanceScore;
                int hintAfterFirst = SaveSystem.Instance.Data.boosterCounts.hint;

                if (gpsAfterFirst <= gpsBefore || hintAfterFirst <= hintBefore)
                {
                    AppendLine(reportBuilder, "[FAIL] Mission completion rewards did not update save values.");
                    passed = false;
                }

                if (!missionDirector.TryMarkSlotRewardClaimed(0))
                {
                    AppendLine(reportBuilder, "[FAIL] Mission slot reward claim could not be marked.");
                    passed = false;
                }
                else if (!missionDirector.IsSlotRewardClaimed(0))
                {
                    AppendLine(reportBuilder, "[FAIL] Mission slot reward claim state was not persisted.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] Mission slot reward claim state persists.");
                }

                if (missionDirector.TryMarkSlotRewardClaimed(0))
                {
                    AppendLine(reportBuilder, "[FAIL] Mission slot reward claim allowed duplicate marking.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] Mission slot reward claim rejects duplicates.");
                }
            }
            finally
            {
                DestroyValidationRoot(validationObject);
            }

            return passed;
        }

        private static bool ValidateDailyBoardRewardGrant(StringBuilder reportBuilder)
        {
            if (SaveSystem.HasInstance || RankingDirector.HasInstance)
            {
                AppendLine(
                    reportBuilder,
                    "[SKIP] Daily board reward grant requires an isolated editor session.");
                return true;
            }

            GameObject validationObject = CreateValidationRoot();
            if (validationObject == null)
            {
                AppendLine(reportBuilder, "[FAIL] Daily board reward validation root could not be created.");
                return false;
            }

            bool passed = true;

            try
            {
                RankingDirector rankingDirector = validationObject.GetComponent<RankingDirector>();
                DailyRewardDirector rewardDirector = validationObject.GetComponent<DailyRewardDirector>();

                long gpsBefore = rankingDirector.GlobalPerformanceScore;
                int shuffleBefore = SaveSystem.Instance.Data.boosterCounts.shuffle;

                if (!rewardDirector.TryGrantDailyBoardCompletionRewards())
                {
                    AppendLine(reportBuilder, "[FAIL] Daily board completion rewards could not be granted.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] Daily board completion rewards grant GPS and boosters.");
                }

                if (rankingDirector.GlobalPerformanceScore <= gpsBefore
                    || SaveSystem.Instance.Data.boosterCounts.shuffle <= shuffleBefore)
                {
                    AppendLine(reportBuilder, "[FAIL] Daily board completion rewards did not update save values.");
                    passed = false;
                }
            }
            finally
            {
                DestroyValidationRoot(validationObject);
            }

            return passed;
        }

        private static bool ValidateProgressionIsolation(StringBuilder reportBuilder)
        {
            if (SaveSystem.HasInstance || RankingDirector.HasInstance)
            {
                AppendLine(
                    reportBuilder,
                    "[SKIP] Progression isolation requires an isolated editor session.");
                return true;
            }

            GameObject validationObject = CreateValidationRoot();
            if (validationObject == null)
            {
                AppendLine(reportBuilder, "[FAIL] Progression isolation validation root could not be created.");
                return false;
            }

            bool passed = true;

            try
            {
                SaveSystem.Instance.Data.currentLevel = 42;
                SaveSystem.Instance.Data.totalLevelsCompleted = 17;

                DailyRewardDirector rewardDirector = validationObject.GetComponent<DailyRewardDirector>();
                rewardDirector.TryGrantDailyBoardCompletionRewards();
                rewardDirector.TryGrantMissionCompletionRewards(DailyMissionTier.Medium);

                if (SaveSystem.Instance.Data.currentLevel != 42
                    || SaveSystem.Instance.Data.totalLevelsCompleted != 17)
                {
                    AppendLine(reportBuilder, "[FAIL] Daily rewards modified progression fields.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] Daily rewards do not modify progression fields.");
                }
            }
            finally
            {
                DestroyValidationRoot(validationObject);
            }

            return passed;
        }

        private static GameObject CreateValidationRoot()
        {
            if (SaveSystem.HasInstance || RankingDirector.HasInstance)
            {
                return null;
            }

            GameObject validationObject = new GameObject("DailyRewardSystemValidator_Temp");
            SaveSystem saveSystem = validationObject.AddComponent<SaveSystem>();
            saveSystem.EnsureValidationInstance();
            saveSystem.ResetToDefaults();

            RankingDirector rankingDirector = validationObject.AddComponent<RankingDirector>();
            rankingDirector.EnsureValidationInstance();

            validationObject.AddComponent<DailyMissionDirector>();
            DailyRewardDirector rewardDirector = validationObject.AddComponent<DailyRewardDirector>();
            rewardDirector.EnsureValidationInstance();
            return validationObject;
        }

        private static void DestroyValidationRoot(GameObject validationObject)
        {
            if (validationObject == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(validationObject);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(validationObject);
            }
        }

        private static bool ValidateTypeExists(Type type, StringBuilder reportBuilder)
        {
            if (type == null)
            {
                AppendLine(reportBuilder, "[FAIL] Required daily reward type is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] " + type.Name + " type is present.");
            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
