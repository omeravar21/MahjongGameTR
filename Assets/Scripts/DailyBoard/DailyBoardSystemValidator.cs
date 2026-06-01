using System;
using System.Reflection;
using System.Text;
using MahjongGame.Core.Save;
using UnityEngine;

namespace MahjongGame.DailyBoard
{
    public static class DailyBoardSystemValidator
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
            passed &= ValidateSeedRules(reportBuilder);
            passed &= ValidateAvailabilityRules(reportBuilder);
            passed &= ValidateDirectorBehavior(reportBuilder);
            passed &= ValidateSaveRoundTrip(reportBuilder);
            passed &= ValidateProgressionIsolation(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Daily board architecture validation completed successfully."
                : "[FAIL] Daily board architecture validation found issues.");

            return passed;
        }

        private static bool ValidateTypes(StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= ValidateTypeExists(typeof(DailyBoardDirector), reportBuilder);
            passed &= ValidateTypeExists(typeof(DailyBoardData), reportBuilder);
            passed &= ValidateTypeExists(typeof(DailyBoardDefinition), reportBuilder);
            passed &= ValidateTypeExists(typeof(DailyBoardIdentity), reportBuilder);

            return passed;
        }

        private static bool ValidateEvents(StringBuilder reportBuilder)
        {
            EventInfo eventInfo = typeof(DailyBoardEvents).GetEvent(
                nameof(DailyBoardEvents.DailyBoardRefreshed),
                BindingFlags.Public | BindingFlags.Static);

            if (eventInfo == null)
            {
                AppendLine(reportBuilder, "[FAIL] Event DailyBoardRefreshed is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Event DailyBoardRefreshed is present.");
            return true;
        }

        private static bool ValidateSeedRules(StringBuilder reportBuilder)
        {
            bool passed = true;

            int dayId = DailyBoardDefinition.GetUtcDayId(SampleUtcDate);
            int expectedDayId = 20260531;

            if (dayId != expectedDayId)
            {
                AppendLine(reportBuilder, "[FAIL] UTC day id does not match yyyyMMdd format.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] UTC day id matches yyyyMMdd format.");
            }

            int firstSeed = DailyBoardDefinition.ComputeSeed(dayId);
            int secondSeed = DailyBoardDefinition.ComputeSeed(dayId);

            if (firstSeed <= 0 || firstSeed != secondSeed)
            {
                AppendLine(reportBuilder, "[FAIL] Daily seed is not deterministic for the same day id.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Daily seed is deterministic for the same day id.");
            }

            int nextDayId = DailyBoardDefinition.GetUtcDayId(NextUtcDate);
            int nextSeed = DailyBoardDefinition.ComputeSeed(nextDayId);

            if (nextSeed <= 0 || nextSeed == firstSeed)
            {
                AppendLine(reportBuilder, "[FAIL] Different day ids did not produce different daily seeds.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Different day ids produce different daily seeds.");
            }

            return passed;
        }

        private static bool ValidateAvailabilityRules(StringBuilder reportBuilder)
        {
            bool passed = true;

            int dayId = DailyBoardDefinition.GetUtcDayId(SampleUtcDate);

            if (!DailyBoardDefinition.IsAvailable(dayId, 0))
            {
                AppendLine(reportBuilder, "[FAIL] Daily board should be available when never completed.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Daily board is available when never completed.");
            }

            if (DailyBoardDefinition.IsAvailable(dayId, dayId))
            {
                AppendLine(reportBuilder, "[FAIL] Daily board should not be available after completion today.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Daily board is unavailable after completion today.");
            }

            int nextDayId = DailyBoardDefinition.GetUtcDayId(NextUtcDate);
            if (!DailyBoardDefinition.IsAvailable(nextDayId, dayId))
            {
                AppendLine(reportBuilder, "[FAIL] Daily board should become available again on a new UTC day.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Daily board becomes available again on a new UTC day.");
            }

            return passed;
        }

        private static bool ValidateDirectorBehavior(StringBuilder reportBuilder)
        {
            GameObject validationObject = new GameObject("DailyBoardSystemValidator_Temp");
            DailyBoardDirector director = validationObject.AddComponent<DailyBoardDirector>();

            bool passed = true;

            try
            {
                director.SetStateForValidation(0, SampleUtcDate);
                DailyBoardIdentity identity = director.GetCurrentIdentity();

                if (!identity.IsValid()
                    || identity.DayId != DailyBoardDefinition.GetUtcDayId(SampleUtcDate)
                    || identity.DailySeed != DailyBoardDefinition.ComputeSeed(identity.DayId)
                    || !identity.IsAvailable
                    || identity.IsCompletedToday)
                {
                    AppendLine(reportBuilder, "[FAIL] DailyBoardDirector did not build a valid daily identity.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] DailyBoardDirector builds a valid daily identity.");
                }

                if (!director.TryMarkCompletedToday())
                {
                    AppendLine(reportBuilder, "[FAIL] DailyBoardDirector could not mark today's board complete.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] DailyBoardDirector marks today's board complete.");
                }

                DailyBoardIdentity completedIdentity = director.GetCurrentIdentity();
                if (completedIdentity.IsAvailable || !completedIdentity.IsCompletedToday)
                {
                    AppendLine(reportBuilder, "[FAIL] DailyBoardDirector completion state is invalid.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] DailyBoardDirector reflects completion state.");
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

            DailyBoardData dailyBoardData = DailyBoardData.CreateDefault();
            int dayId = DailyBoardDefinition.GetUtcDayId(SampleUtcDate);
            dailyBoardData.SetLastCompletedDayId(dayId);
            dailyBoardData.WriteToSave(saveData);

            DailyBoardData loadedData = DailyBoardData.FromSave(saveData);
            if (loadedData.LastCompletedDayId != dayId)
            {
                AppendLine(reportBuilder, "[FAIL] Daily board completion did not persist in save data.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Daily board completion persists in save data.");
            }

            return passed;
        }

        private static bool ValidateProgressionIsolation(StringBuilder reportBuilder)
        {
            bool passed = true;

            PlayerSaveData saveData = PlayerSaveData.CreateDefault();
            saveData.EnsureDefaults();
            saveData.currentLevel = 42;

            DailyBoardData dailyBoardData = DailyBoardData.CreateDefault();
            dailyBoardData.SetLastCompletedDayId(DailyBoardDefinition.GetUtcDayId(SampleUtcDate));
            dailyBoardData.WriteToSave(saveData);

            if (saveData.currentLevel != 42)
            {
                AppendLine(reportBuilder, "[FAIL] Daily board save write modified current level progression.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Daily board save write does not modify current level progression.");
            }

            return passed;
        }

        private static bool ValidateTypeExists(Type type, StringBuilder reportBuilder)
        {
            if (type == null)
            {
                AppendLine(reportBuilder, "[FAIL] Required daily board type is missing.");
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
