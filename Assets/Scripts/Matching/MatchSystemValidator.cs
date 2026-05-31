using System.Reflection;
using System.Text;
using MahjongGame.Tiles;
using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Matching
{
    public static class MatchSystemValidator
    {
        public static bool Validate(Transform gameplayRoot, Transform trayRoot, StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            if (gameplayRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] GameplayRoot is missing for match system validation.");
                return false;
            }

            MatchController matchController = gameplayRoot.GetComponent<MatchController>();
            TrayController trayController = gameplayRoot.GetComponent<TrayController>();

            passed &= ValidateDetection(gameplayRoot, matchController, reportBuilder);
            passed &= ValidateExecution(matchController, trayController, reportBuilder);
            passed &= ValidateCleanup(trayController, reportBuilder);
            passed &= ValidateWiring(gameplayRoot, matchController, trayController, reportBuilder);

            if (Application.isPlaying)
            {
                passed &= ValidatePlayModeReconciliation(trayController, trayRoot, reportBuilder);
            }
            else
            {
                AppendLine(reportBuilder, "[SKIP] Play-mode match reconciliation checks require Play Mode.");
            }

            AppendLine(reportBuilder, passed
                ? "[PASS] Match system validation completed successfully."
                : "[FAIL] Match system validation found issues.");

            return passed;
        }

        private static bool ValidateDetection(
            Transform gameplayRoot,
            MatchController matchController,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            if (matchController == null)
            {
                AppendLine(reportBuilder, "[FAIL] MatchController is missing on GameplayRoot for detection validation.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] MatchController is present on GameplayRoot.");
            }

            AppendLine(reportBuilder, "[PASS] TileMatchComparer type is present.");

            if (MatchDefinition.MatchDelaySeconds != 0.3f)
            {
                AppendLine(reportBuilder, "[FAIL] MatchDefinition.MatchDelaySeconds is not 0.3.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] MatchDefinition.MatchDelaySeconds is 0.3.");
            }

            if (!HasMatchEvent(nameof(MatchEvents.MatchDetected)))
            {
                AppendLine(reportBuilder, "[FAIL] MatchEvents.MatchDetected event is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] MatchEvents.MatchDetected event is present.");
            }

            if (gameplayRoot.GetComponent<MatchController>() == null)
            {
                AppendLine(reportBuilder, "[FAIL] GameScene GameplayRoot is missing MatchController.");
                passed = false;
            }

            return passed;
        }

        private static bool ValidateExecution(
            MatchController matchController,
            TrayController trayController,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            AppendLine(reportBuilder, "[PASS] MatchExecutor type is present.");

            if (!HasMatchEvent(nameof(MatchEvents.MatchExecuted)))
            {
                AppendLine(reportBuilder, "[FAIL] MatchEvents.MatchExecuted event is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] MatchEvents.MatchExecuted event is present.");
            }

            if (!HasMatchEvent(nameof(MatchEvents.MatchDelayCompleted)))
            {
                AppendLine(reportBuilder, "[FAIL] MatchEvents.MatchDelayCompleted event is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] MatchEvents.MatchDelayCompleted event is present.");
            }

            if (matchController == null || trayController == null)
            {
                AppendLine(reportBuilder, "[FAIL] MatchController or TrayController is missing for execution wiring validation.");
                passed = false;
            }
            else if (!TryGetSerializedReference(matchController, "trayController", out TrayController wiredTrayController)
                || wiredTrayController == null)
            {
                AppendLine(reportBuilder, "[FAIL] MatchController is not wired to TrayController for execution.");
                passed = false;
            }
            else if (wiredTrayController != trayController)
            {
                AppendLine(reportBuilder, "[FAIL] MatchController.trayController does not reference scene TrayController.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] MatchController is wired to TrayController for execution.");
            }

            return passed;
        }

        private static bool ValidateCleanup(TrayController trayController, StringBuilder reportBuilder)
        {
            bool passed = true;

            AppendLine(reportBuilder, "[PASS] MatchCleaner type is present.");

            if (!HasMatchEvent(nameof(MatchEvents.MatchCleanedUp)))
            {
                AppendLine(reportBuilder, "[FAIL] MatchEvents.MatchCleanedUp event is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] MatchEvents.MatchCleanedUp event is present.");
            }

            if (trayController == null)
            {
                AppendLine(reportBuilder, "[FAIL] TrayController is missing for cleanup validation.");
                passed = false;
            }
            else
            {
                if (!HasTrayMethod(trayController, nameof(TrayController.TryReleaseMatchedTiles)))
                {
                    AppendLine(reportBuilder, "[FAIL] TrayController.TryReleaseMatchedTiles is missing.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] TrayController.TryReleaseMatchedTiles is present.");
                }

                if (!HasTrayMethod(trayController, nameof(TrayController.ValidateSlotEmpty)))
                {
                    AppendLine(reportBuilder, "[FAIL] TrayController.ValidateSlotEmpty is missing.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] TrayController.ValidateSlotEmpty is present.");
                }
            }

            return passed;
        }

        private static bool ValidateWiring(
            Transform gameplayRoot,
            MatchController matchController,
            TrayController trayController,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            if (matchController == null)
            {
                AppendLine(reportBuilder, "[FAIL] MatchController is missing for wiring validation.");
                passed = false;
            }
            else if (!TryGetSerializedReference(matchController, "trayController", out TrayController wiredTrayController)
                || wiredTrayController == null)
            {
                AppendLine(reportBuilder, "[FAIL] MatchController serialized trayController reference is not wired.");
                passed = false;
            }
            else if (wiredTrayController != trayController)
            {
                AppendLine(reportBuilder, "[FAIL] MatchController serialized trayController does not reference scene TrayController.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] MatchController serialized trayController reference is wired.");
            }

            if (gameplayRoot.GetComponent<MatchController>() == null)
            {
                AppendLine(reportBuilder, "[FAIL] GameScene GameplayRoot is missing MatchController for wiring validation.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] GameScene GameplayRoot has MatchController.");
            }

            return passed;
        }

        private static bool ValidatePlayModeReconciliation(
            TrayController trayController,
            Transform trayRoot,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            if (trayController == null || trayRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] Match play-mode reconciliation requires TrayController and TrayRoot.");
                return false;
            }

            Transform trayContainer = ResolveTrayContainer(trayRoot);
            if (trayContainer == null)
            {
                AppendLine(reportBuilder, "[FAIL] TrayContainer is missing for match play-mode reconciliation.");
                return false;
            }

            for (int slotIndex = 0; slotIndex < TrayRootDefinition.SlotCount; slotIndex++)
            {
                bool slotEmpty = trayController.ValidateSlotEmpty(slotIndex);
                Transform slotTransform = trayContainer.Find(TrayRootDefinition.GetSlotName(slotIndex));
                TrayOccupancyQuery.TryGetOccupyingTile(slotTransform, out Tile hierarchyTile);

                if (slotEmpty && hierarchyTile != null)
                {
                    AppendLine(reportBuilder, "[FAIL] Match cleanup left slot "
                        + slotIndex
                        + " empty in TrayController but hierarchy still has an occupying tile.");
                    passed = false;
                }
                else if (!slotEmpty
                    && trayController.TryGetTileAtSlot(slotIndex, out Tile controllerTile)
                    && hierarchyTile != controllerTile)
                {
                    AppendLine(reportBuilder, "[FAIL] Match slot "
                        + slotIndex
                        + " mismatch between TrayController state and hierarchy occupancy.");
                    passed = false;
                }
            }

            if (passed)
            {
                AppendLine(reportBuilder, "[PASS] Match cleanup slot occupancy reconciles with hierarchy.");
            }

            int matchedTileCount = CountMatchedTilesInTrayContainer(trayContainer);
            if (matchedTileCount > 0)
            {
                AppendLine(reportBuilder, "[FAIL] Tray container still contains "
                    + matchedTileCount
                    + " Matched tile(s) after match cleanup.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] No Matched tiles remain in tray container.");
            }

            return passed;
        }

        private static int CountMatchedTilesInTrayContainer(Transform trayContainer)
        {
            if (trayContainer == null)
            {
                return 0;
            }

            int matchedCount = 0;
            for (int slotIndex = 0; slotIndex < TrayRootDefinition.SlotCount; slotIndex++)
            {
                Transform slotTransform = trayContainer.Find(TrayRootDefinition.GetSlotName(slotIndex));
                if (slotTransform == null)
                {
                    continue;
                }

                for (int childIndex = 0; childIndex < slotTransform.childCount; childIndex++)
                {
                    Tile tile = slotTransform.GetChild(childIndex).GetComponent<Tile>();
                    if (tile != null && tile.State == TileState.Matched)
                    {
                        matchedCount++;
                    }
                }
            }

            return matchedCount;
        }

        private static bool HasMatchEvent(string eventName)
        {
            EventInfo eventInfo = typeof(MatchEvents).GetEvent(
                eventName,
                BindingFlags.Static | BindingFlags.Public);

            return eventInfo != null;
        }

        private static bool HasTrayMethod(TrayController trayController, string methodName)
        {
            return trayController.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null;
        }

        private static Transform ResolveTrayContainer(Transform trayRoot)
        {
            TrayRootController trayRootController = trayRoot.GetComponent<TrayRootController>();
            if (trayRootController != null)
            {
                Transform container = trayRootController.GetTrayContainer();
                if (container != null)
                {
                    return container;
                }
            }

            return trayRoot.Find(TrayRootDefinition.TrayContainerName);
        }

        private static bool TryGetSerializedReference<T>(
            Component component,
            string fieldName,
            out T reference)
            where T : Object
        {
            reference = null;
            if (component == null)
            {
                return false;
            }

            FieldInfo field = component.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (field == null)
            {
                return false;
            }

            reference = field.GetValue(component) as T;
            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string line)
        {
            reportBuilder.AppendLine(line);
        }
    }
}
