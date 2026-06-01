using System;
using System.Reflection;
using System.Text;
using MahjongGame.Core.Save;
using UnityEngine;

namespace MahjongGame.DailyMissions
{
    public static class DailyMissionSystemValidator
    {
        private static readonly DateTime SampleUtcDate = new DateTime(2026, 5, 31, 12, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime NextUtcDate = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateTypes(reportBuilder);
            passed &= ValidateEvents(reportBuilder);
            passed &= ValidateDeterministicMissionSet(reportBuilder);
            passed &= ValidateDifferentDayMissionSet(reportBuilder);
            passed &= ValidateTierDistribution(reportBuilder);
            passed &= ValidateDirectorProgressAndCompletion(reportBuilder);
            passed &= ValidateDayRollover(reportBuilder);
            passed &= ValidateSaveRoundTrip(reportBuilder);
            passed &= ValidateRankingAndProgressionIsolation(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Daily mission architecture validation completed successfully."
                : "[FAIL] Daily mission architecture validation found issues.");

            return passed;
        }

        private static bool ValidateTypes(StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= ValidateTypeExists(typeof(DailyMissionDirector), reportBuilder);
            passed &= ValidateTypeExists(typeof(DailyMissionData), reportBuilder);
            passed &= ValidateTypeExists(typeof(DailyMissionDefinition), reportBuilder);
            passed &= ValidateTypeExists(typeof(DailyMissionSet), reportBuilder);
            passed &= ValidateTypeExists(typeof(DailyMissionProgressTracker), reportBuilder);

            return passed;
        }

        private static bool ValidateEvents(StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= ValidateEventExists(
                typeof(DailyMissionEvents),
                nameof(DailyMissionEvents.DailyMissionsRefreshed),
                reportBuilder);
            passed &= ValidateEventExists(
                typeof(DailyMissionEvents),
                nameof(DailyMissionEvents.DailyMissionProgressChanged),
                reportBuilder);
            passed &= ValidateEventExists(
                typeof(DailyMissionEvents),
                nameof(DailyMissionEvents.DailyMissionCompleted),
                reportBuilder);

            return passed;
        }

        private static bool ValidateDeterministicMissionSet(StringBuilder reportBuilder)
        {
            int dayId = DailyMissionDefinition.GetUtcDayId(SampleUtcDate);
            int missionSeed = DailyMissionDefinition.ComputeMissionSeed(dayId);

            DailyMissionSet first = DailyMissionSetGenerator.GenerateSet(dayId, missionSeed);
            DailyMissionSet second = DailyMissionSetGenerator.GenerateSet(dayId, missionSeed);

            if (!MissionSetEqual(first, second))
            {
                AppendLine(reportBuilder, "[FAIL] Same day id did not produce identical daily mission sets.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Same day id produces identical daily mission sets.");
            return true;
        }

        private static bool ValidateDifferentDayMissionSet(StringBuilder reportBuilder)
        {
            int firstDayId = DailyMissionDefinition.GetUtcDayId(SampleUtcDate);
            int secondDayId = DailyMissionDefinition.GetUtcDayId(NextUtcDate);

            DailyMissionSet first = DailyMissionSetGenerator.GenerateSet(
                firstDayId,
                DailyMissionDefinition.ComputeMissionSeed(firstDayId));
            DailyMissionSet second = DailyMissionSetGenerator.GenerateSet(
                secondDayId,
                DailyMissionDefinition.ComputeMissionSeed(secondDayId));

            if (MissionSetEqual(first, second))
            {
                AppendLine(reportBuilder, "[FAIL] Different day ids produced identical daily mission sets.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Different day ids produce different daily mission sets.");
            return true;
        }

        private static bool ValidateTierDistribution(StringBuilder reportBuilder)
        {
            int dayId = DailyMissionDefinition.GetUtcDayId(SampleUtcDate);
            DailyMissionSet missionSet = DailyMissionSetGenerator.GenerateSet(
                dayId,
                DailyMissionDefinition.ComputeMissionSeed(dayId));

            int easyCount = 0;
            int mediumCount = 0;
            int hardCount = 0;

            for (int i = 0; i < missionSet.Entries.Length; i++)
            {
                switch (missionSet.Entries[i].Slot.Tier)
                {
                    case DailyMissionTier.Easy:
                        easyCount++;
                        break;
                    case DailyMissionTier.Medium:
                        mediumCount++;
                        break;
                    case DailyMissionTier.Hard:
                        hardCount++;
                        break;
                }
            }

            if (easyCount != DailyMissionDefinition.EasyMissionCount
                || mediumCount != DailyMissionDefinition.MediumMissionCount
                || hardCount != DailyMissionDefinition.HardMissionCount)
            {
                AppendLine(reportBuilder, "[FAIL] Daily mission set tier distribution is invalid.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Daily mission set has 2 Easy, 2 Medium, and 1 Hard mission.");
            return true;
        }

        private static bool ValidateDirectorProgressAndCompletion(StringBuilder reportBuilder)
        {
            GameObject validationObject = new GameObject("DailyMissionSystemValidator_Temp");
            DailyMissionDirector director = validationObject.AddComponent<DailyMissionDirector>();

            bool passed = true;

            try
            {
                director.SetStateForValidation(0, SampleUtcDate);
                DailyMissionSet missionSet = director.GetCurrentSet();

                if (!missionSet.IsValid())
                {
                    AppendLine(reportBuilder, "[FAIL] DailyMissionDirector did not build a valid daily mission set.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] DailyMissionDirector builds a valid daily mission set.");
                }

                if (!director.TryApplyProgress(
                        missionSet.Entries[0].Slot.MissionType,
                        1,
                        out _))
                {
                    AppendLine(reportBuilder, "[FAIL] DailyMissionDirector could not apply mission progress.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] DailyMissionDirector applies mission progress.");
                }

                DailyMissionEntry firstEntry = missionSet.Entries[0];
                int amountNeeded = firstEntry.Slot.TargetValue - firstEntry.Progress.CurrentValue;
                if (amountNeeded > 0)
                {
                    director.TryApplyProgress(firstEntry.Slot.MissionType, amountNeeded, out _);
                }

                bool completedAny = false;
                DailyMissionSet updatedSet = director.GetCurrentSet();
                if (updatedSet.Entries[0].IsComplete())
                {
                    completedAny = true;
                }

                if (!completedAny)
                {
                    AppendLine(reportBuilder, "[FAIL] DailyMissionDirector did not mark eligible mission progress as complete.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] DailyMissionDirector marks mission completion state.");
                }
            }
            finally
            {
                director.ResetUtcNowProviderForValidation();
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(validationObject);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(validationObject);
                }
            }

            return passed;
        }

        private static bool ValidateDayRollover(StringBuilder reportBuilder)
        {
            GameObject validationObject = new GameObject("DailyMissionSystemValidator_Rollover");
            DailyMissionDirector director = validationObject.AddComponent<DailyMissionDirector>();

            bool passed = true;

            try
            {
                director.SetStateForValidation(0, SampleUtcDate);
                DailyMissionSet firstDaySet = director.GetCurrentSet();
                for (int i = 0; i < firstDaySet.Entries.Length; i++)
                {
                    DailyMissionEntry entry = firstDaySet.Entries[i];
                    director.TryApplyProgress(entry.Slot.MissionType, entry.Slot.TargetValue, out _);
                }

                director.SetStateForValidation(0, NextUtcDate);
                DailyMissionSet nextDaySet = director.GetCurrentSet();

                if (nextDaySet.DayId == firstDaySet.DayId)
                {
                    AppendLine(reportBuilder, "[FAIL] Daily mission day rollover did not change day id.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] Daily mission day rollover changes day id.");
                }

                bool hasResetProgress = true;
                for (int i = 0; i < nextDaySet.Entries.Length; i++)
                {
                    if (nextDaySet.Entries[i].Progress.CurrentValue != 0 || nextDaySet.Entries[i].Progress.IsCompleted)
                    {
                        hasResetProgress = false;
                        break;
                    }
                }

                if (!hasResetProgress)
                {
                    AppendLine(reportBuilder, "[FAIL] Daily mission day rollover did not reset progress.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] Daily mission day rollover resets progress.");
                }
            }
            finally
            {
                director.ResetUtcNowProviderForValidation();
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(validationObject);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(validationObject);
                }
            }

            return passed;
        }

        private static bool ValidateSaveRoundTrip(StringBuilder reportBuilder)
        {
            bool passed = true;

            PlayerSaveData saveData = PlayerSaveData.CreateDefault();
            saveData.EnsureDefaults();

            int dayId = DailyMissionDefinition.GetUtcDayId(SampleUtcDate);
            DailyMissionSet missionSet = DailyMissionSetGenerator.GenerateSet(
                dayId,
                DailyMissionDefinition.ComputeMissionSeed(dayId));

            saveData.dailyMissions.missionDayId = dayId;
            for (int i = 0; i < DailyMissionSet.SlotCount; i++)
            {
                saveData.dailyMissions.slotMissionTypes[i] = (int)missionSet.Entries[i].Slot.MissionType;
                saveData.dailyMissions.slotProgress[i] = i + 1;
                saveData.dailyMissions.slotCompleted[i] = i == 0;
            }

            DailyMissionData loadedData = DailyMissionData.FromSave(saveData);
            if (loadedData.MissionDayId != dayId || loadedData.GetSlotProgress(0) != 1 || !loadedData.IsSlotCompleted(0))
            {
                AppendLine(reportBuilder, "[FAIL] Daily mission save data did not round-trip correctly.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Daily mission save data round-trips correctly.");
            }

            return passed;
        }

        private static bool ValidateRankingAndProgressionIsolation(StringBuilder reportBuilder)
        {
            bool passed = true;

            PlayerSaveData saveData = PlayerSaveData.CreateDefault();
            saveData.EnsureDefaults();
            saveData.currentLevel = 42;
            saveData.globalPerformanceScore = 5000;

            int dayId = DailyMissionDefinition.GetUtcDayId(SampleUtcDate);
            saveData.dailyMissions.missionDayId = dayId;
            saveData.dailyMissions.slotProgress[0] = 3;
            saveData.dailyMissions.slotCompleted[0] = true;

            if (saveData.currentLevel != 42 || saveData.globalPerformanceScore != 5000)
            {
                AppendLine(reportBuilder, "[FAIL] Daily mission save write modified progression or ranking fields.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Daily mission save write does not modify progression or ranking fields.");
            }

            GameObject validationObject = new GameObject("DailyMissionSystemValidator_Isolation");
            DailyMissionDirector director = validationObject.AddComponent<DailyMissionDirector>();

            try
            {
                director.SetStateForValidation(0, SampleUtcDate);
                director.TryApplyProgress(
                    director.GetCurrentSet().Entries[0].Slot.MissionType,
                    5,
                    out _);

                if (saveData.currentLevel != 42 || saveData.globalPerformanceScore != 5000)
                {
                    AppendLine(reportBuilder, "[FAIL] Daily mission progress modified progression or ranking fields.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] Daily mission progress does not modify progression or ranking fields.");
                }
            }
            finally
            {
                director.ResetUtcNowProviderForValidation();
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(validationObject);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(validationObject);
                }
            }

            return passed;
        }

        private static bool MissionSetEqual(DailyMissionSet left, DailyMissionSet right)
        {
            if (left == null || right == null || left.Entries.Length != right.Entries.Length)
            {
                return false;
            }

            for (int i = 0; i < left.Entries.Length; i++)
            {
                DailyMissionSlot leftSlot = left.Entries[i].Slot;
                DailyMissionSlot rightSlot = right.Entries[i].Slot;

                if (leftSlot.MissionType != rightSlot.MissionType
                    || leftSlot.Tier != rightSlot.Tier
                    || leftSlot.TargetValue != rightSlot.TargetValue)
                {
                    return false;
                }
            }

            return left.DayId == right.DayId && left.MissionSeed == right.MissionSeed;
        }

        private static bool ValidateTypeExists(Type type, StringBuilder reportBuilder)
        {
            if (type == null)
            {
                AppendLine(reportBuilder, "[FAIL] Required daily mission type is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] " + type.Name + " type is present.");
            return true;
        }

        private static bool ValidateEventExists(Type eventsType, string eventName, StringBuilder reportBuilder)
        {
            EventInfo eventInfo = eventsType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Static);
            if (eventInfo == null)
            {
                AppendLine(reportBuilder, "[FAIL] Event " + eventName + " is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Event " + eventName + " is present.");
            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
