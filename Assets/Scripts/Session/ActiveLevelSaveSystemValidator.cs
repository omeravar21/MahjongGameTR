using System.Reflection;
using System.Text;
using MahjongGame.Core;
using MahjongGame.Core.Save;
using UnityEngine;

namespace MahjongGame.Session
{
    public static class ActiveLevelSaveSystemValidator
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
                AppendLine(reportBuilder, "[FAIL] GameplayRoot is missing for active level save validation.");
                return false;
            }

            ActiveLevelSaveDirector saveDirector = gameplayRoot.GetComponent<ActiveLevelSaveDirector>();
            passed &= ValidateComponents(saveDirector, reportBuilder);
            passed &= ValidateTypesAndEvents(reportBuilder);
            passed &= ValidateSaveDataShape(reportBuilder);

            if (saveDirector != null)
            {
                passed &= ValidatePersistAndClearBehavior(saveDirector, reportBuilder);
            }

            AppendLine(reportBuilder, passed
                ? "[PASS] Active level save validation completed successfully."
                : "[FAIL] Active level save validation found issues.");

            return passed;
        }

        private static bool ValidateComponents(
            ActiveLevelSaveDirector saveDirector,
            StringBuilder reportBuilder)
        {
            if (saveDirector == null)
            {
                AppendLine(reportBuilder, "[FAIL] ActiveLevelSaveDirector is missing on GameplayRoot.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] ActiveLevelSaveDirector is present on GameplayRoot.");
            return true;
        }

        private static bool ValidateTypesAndEvents(StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= ValidateTypeExists(typeof(ActiveLevelStateSaveData), reportBuilder);
            passed &= ValidateEventExists(typeof(SessionEvents), nameof(SessionEvents.SessionStarted), reportBuilder);
            passed &= ValidateEventExists(typeof(SessionEvents), nameof(SessionEvents.SessionEnded), reportBuilder);

            return passed;
        }

        private static bool ValidateSaveDataShape(StringBuilder reportBuilder)
        {
            ActiveLevelStateSaveData saveData = new ActiveLevelStateSaveData();
            bool passed = true;

            if (saveData.boardStateJson == null
                || saveData.trayStateJson == null
                || saveData.closedTileStateJson == null
                || saveData.matchedTilesJson == null
                || saveData.remainingTilesJson == null)
            {
                AppendLine(reportBuilder, "[FAIL] ActiveLevelStateSaveData JSON fields are not initialized.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] ActiveLevelStateSaveData JSON fields are initialized.");
            }

            return passed;
        }

        private static bool ValidatePersistAndClearBehavior(
            ActiveLevelSaveDirector saveDirector,
            StringBuilder reportBuilder)
        {
            if (!SaveSystem.HasInstance || SaveSystem.Instance.Data == null)
            {
                AppendLine(reportBuilder, "[SKIP] SaveSystem is unavailable for persist/clear validation.");
                return true;
            }

            saveDirector.ResetBoosterUsageForValidation();
            saveDirector.ClearActiveSessionSaveForValidation();

            if (SaveSystem.Instance.Data.activeLevelState.hasActiveSession)
            {
                AppendLine(reportBuilder, "[FAIL] Active session flag was not cleared.");
                return false;
            }

            SaveSystem.Instance.Data.activeLevelState.hasActiveSession = true;
            SaveSystem.Instance.Data.activeLevelState.currentLevel = 7;
            SaveSystem.Instance.Data.activeLevelState.currentSeed = 12345;
            SaveSystem.Instance.Data.activeLevelState.score = 42;

            if (!saveDirector.TryPersistActiveSessionForValidation())
            {
                AppendLine(reportBuilder, "[SKIP] Persist validation requires an active session in Play Mode.");
                SaveSystem.Instance.Data.activeLevelState = new ActiveLevelStateSaveData();
                SaveSystem.Instance.Save();
                return true;
            }

            saveDirector.ClearActiveSessionSaveForValidation();
            if (SaveSystem.Instance.Data.activeLevelState.hasActiveSession)
            {
                AppendLine(reportBuilder, "[FAIL] Clear active session save did not reset hasActiveSession.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Active level save clear behavior is correct.");
            return true;
        }

        private static bool ValidateTypeExists(System.Type type, StringBuilder reportBuilder)
        {
            if (type == null)
            {
                AppendLine(reportBuilder, "[FAIL] Required active level save type is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] " + type.Name + " type is present.");
            return true;
        }

        private static bool ValidateEventExists(
            System.Type eventsType,
            string eventName,
            StringBuilder reportBuilder)
        {
            EventInfo eventInfo = eventsType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Static);
            if (eventInfo == null)
            {
                AppendLine(reportBuilder, "[FAIL] " + eventsType.Name + "." + eventName + " event is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] " + eventsType.Name + "." + eventName + " event is present.");
            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string line)
        {
            reportBuilder.AppendLine(line);
        }
    }
}
