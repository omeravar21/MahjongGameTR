using System.Reflection;
using System.Text;
using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.Tray
{
    public static class TraySystemValidator
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
                AppendLine(reportBuilder, "[FAIL] GameplayRoot is missing for tray system validation.");
                return false;
            }

            if (trayRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] TrayRoot is missing for tray system validation.");
                return false;
            }

            TrayController trayController = gameplayRoot.GetComponent<TrayController>();
            TrayCapacityController capacityController = trayRoot.GetComponent<TrayCapacityController>();
            TileMovementController movementController = gameplayRoot.GetComponent<TileMovementController>();

            passed &= ValidateCapacity(trayController, capacityController, trayRoot, reportBuilder);
            passed &= ValidateStorage(trayController, capacityController, trayRoot, movementController, reportBuilder);
            passed &= ValidateOverflow(trayController, capacityController, reportBuilder);
            passed &= ValidateWiring(trayController, movementController, reportBuilder);

            if (Application.isPlaying)
            {
                passed &= ValidatePlayModeReconciliation(
                    trayController,
                    capacityController,
                    trayRoot,
                    reportBuilder);
            }
            else
            {
                AppendLine(reportBuilder, "[SKIP] Play-mode tray reconciliation checks require Play Mode.");
            }

            AppendLine(reportBuilder, passed
                ? "[PASS] Tray system validation completed successfully."
                : "[FAIL] Tray system validation found issues.");

            return passed;
        }

        private static bool ValidateCapacity(
            TrayController trayController,
            TrayCapacityController capacityController,
            Transform trayRoot,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            if (capacityController == null)
            {
                AppendLine(reportBuilder, "[FAIL] TrayCapacityController is missing for capacity validation.");
                passed = false;
            }
            else if (capacityController.Capacity != TrayRootDefinition.Capacity)
            {
                AppendLine(reportBuilder, "[FAIL] TrayCapacityController capacity is not 4.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TrayCapacityController capacity is 4.");
            }

            if (trayController == null)
            {
                AppendLine(reportBuilder, "[FAIL] TrayController is missing for capacity validation.");
                passed = false;
            }
            else if (trayController.Capacity != TrayRootDefinition.Capacity)
            {
                AppendLine(reportBuilder, "[FAIL] TrayController capacity is not 4.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TrayController capacity is 4.");
            }

            if (!TrayRootController.HasRequiredTrayHierarchy(trayRoot))
            {
                AppendLine(reportBuilder, "[FAIL] Tray slot anchors are incomplete (expected 4).");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] All four tray slot anchors exist.");
            }

            return passed;
        }

        private static bool ValidateStorage(
            TrayController trayController,
            TrayCapacityController capacityController,
            Transform trayRoot,
            TileMovementController movementController,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            if (trayController == null)
            {
                AppendLine(reportBuilder, "[FAIL] TrayController is missing for storage validation.");
                return false;
            }

            if (!TryGetSerializedReference(trayController, "trayRootTransform", out Transform wiredTrayRoot)
                || wiredTrayRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] TrayController trayRootTransform reference is not wired.");
                passed = false;
            }
            else if (wiredTrayRoot != trayRoot)
            {
                AppendLine(reportBuilder, "[FAIL] TrayController trayRootTransform does not reference scene TrayRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TrayController trayRootTransform is wired.");
            }

            if (!TryGetSerializedReference(trayController, "trayCapacityController", out TrayCapacityController wiredCapacity)
                || wiredCapacity == null)
            {
                AppendLine(reportBuilder, "[FAIL] TrayController trayCapacityController reference is not wired.");
                passed = false;
            }
            else if (wiredCapacity != capacityController)
            {
                AppendLine(reportBuilder, "[FAIL] TrayController trayCapacityController does not reference scene TrayCapacityController.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TrayController trayCapacityController is wired.");
            }

            if (trayController.GetTrayTilesInSlotOrder().Count != TrayRootDefinition.SlotCount)
            {
                AppendLine(reportBuilder, "[FAIL] TrayController.GetTrayTilesInSlotOrder length is not 4.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TrayController.GetTrayTilesInSlotOrder returns slot-ordered length 4.");
            }

            if (movementController == null)
            {
                AppendLine(reportBuilder, "[FAIL] TileMovementController is missing for storage routing validation.");
                passed = false;
            }
            else if (movementController.GetComponent<TrayCapacityController>() != null)
            {
                AppendLine(reportBuilder, "[FAIL] TileMovementController must not host TrayCapacityController directly.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TileMovementController routes tray admission through TrayController.");
            }

            return passed;
        }

        private static bool ValidateOverflow(
            TrayController trayController,
            TrayCapacityController capacityController,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            if (capacityController == null)
            {
                AppendLine(reportBuilder, "[FAIL] TrayCapacityController is missing for overflow validation.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TrayCapacityController is present for overflow detection.");
            }

            if (trayController == null)
            {
                AppendLine(reportBuilder, "[FAIL] TrayController is missing for overflow validation.");
                return passed;
            }

            bool hasAvailableSlot = trayController.HasAvailableSlot();
            bool isAtCapacity = trayController.IsAtCapacity();
            if (hasAvailableSlot == isAtCapacity)
            {
                AppendLine(reportBuilder, "[FAIL] TrayController HasAvailableSlot/IsAtCapacity are inconsistent on empty tray.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TrayController HasAvailableSlot/IsAtCapacity are consistent.");
            }

            if (Application.isPlaying && IsTrayHierarchyEmpty(capacityController))
            {
                if (!hasAvailableSlot || isAtCapacity)
                {
                    AppendLine(reportBuilder, "[FAIL] Empty tray should expose an available slot and not be at capacity.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] Empty tray reports available slot and not at capacity.");
                }
            }

            if (Application.isPlaying && trayController.enabled)
            {
                passed &= ValidateOverflowEventWiring(trayController, reportBuilder);
            }
            else if (!Application.isPlaying)
            {
                AppendLine(reportBuilder, "[SKIP] Overflow event subscription wiring requires Play Mode.");
            }

            return passed;
        }

        private static bool ValidateOverflowEventWiring(TrayController trayController, StringBuilder reportBuilder)
        {
            FieldInfo eventBackingField = typeof(TrayCapacityEvents).GetField(
                "TrayCapacityOverflowDetected",
                BindingFlags.Static | BindingFlags.NonPublic);

            if (eventBackingField == null)
            {
                AppendLine(reportBuilder, "[FAIL] TrayCapacityEvents overflow event backing field was not found.");
                return false;
            }

            if (!(eventBackingField.GetValue(null) is System.Delegate subscribers))
            {
                AppendLine(reportBuilder, "[FAIL] TrayCapacityEvents overflow event has no subscribers in Play Mode.");
                return false;
            }

            bool trayControllerSubscribed = false;
            foreach (System.Delegate subscriber in subscribers.GetInvocationList())
            {
                if (ReferenceEquals(subscriber.Target, trayController))
                {
                    trayControllerSubscribed = true;
                    break;
                }
            }

            if (!trayControllerSubscribed)
            {
                AppendLine(reportBuilder, "[FAIL] TrayController is not subscribed to TrayCapacityEvents overflow forwarding.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] TrayController is subscribed to TrayCapacityEvents overflow forwarding.");
            return true;
        }

        private static bool ValidateWiring(
            TrayController trayController,
            TileMovementController movementController,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            if (movementController == null)
            {
                AppendLine(reportBuilder, "[FAIL] TileMovementController is missing for tray wiring validation.");
                passed = false;
            }
            else if (!TryGetSerializedReference(movementController, "trayController", out TrayController wiredTrayController)
                || wiredTrayController == null)
            {
                AppendLine(reportBuilder, "[FAIL] TileMovementController.trayController reference is not wired.");
                passed = false;
            }
            else if (wiredTrayController != trayController)
            {
                AppendLine(reportBuilder, "[FAIL] TileMovementController.trayController does not reference scene TrayController.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TileMovementController.trayController reference is wired.");
            }

            return passed;
        }

        private static bool ValidatePlayModeReconciliation(
            TrayController trayController,
            TrayCapacityController capacityController,
            Transform trayRoot,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            if (trayController == null || capacityController == null)
            {
                AppendLine(reportBuilder, "[FAIL] Tray play-mode reconciliation requires TrayController and TrayCapacityController.");
                return false;
            }

            Transform trayContainer = ResolveTrayContainer(trayRoot);
            if (trayContainer == null)
            {
                AppendLine(reportBuilder, "[FAIL] TrayContainer is missing for play-mode reconciliation.");
                return false;
            }

            for (int slotIndex = 0; slotIndex < TrayRootDefinition.SlotCount; slotIndex++)
            {
                trayController.TryGetTileAtSlot(slotIndex, out Tile controllerTile);
                TrayOccupancyQuery.TryGetOccupyingTile(
                    trayContainer.Find(TrayRootDefinition.GetSlotName(slotIndex)),
                    out Tile hierarchyTile);

                if (controllerTile != hierarchyTile)
                {
                    AppendLine(reportBuilder, "[FAIL] Tray slot "
                        + slotIndex
                        + " mismatch between TrayController state and hierarchy occupancy.");
                    passed = false;
                }
            }

            if (passed)
            {
                AppendLine(reportBuilder, "[PASS] TrayController slot occupancy matches hierarchy per slot.");
            }

            int hierarchyStoredCount = TrayOccupancyQuery.CountOccupiedSlots(trayContainer);
            if (trayController.StoredTileCount != hierarchyStoredCount)
            {
                AppendLine(reportBuilder, "[FAIL] TrayController StoredTileCount does not match hierarchy stored tiles.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TrayController StoredTileCount matches hierarchy stored tiles.");
            }

            int hierarchyReservedCount = TrayOccupancyQuery.CountReservedTrayTiles(trayContainer);
            if (trayController.ReservedTileCount < hierarchyReservedCount)
            {
                AppendLine(reportBuilder, "[FAIL] TrayController ReservedTileCount is lower than hierarchy reserved tiles.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TrayController ReservedTileCount reconciles with hierarchy reserved tiles.");
            }

            return passed;
        }

        private static bool IsTrayHierarchyEmpty(TrayCapacityController capacityController)
        {
            if (capacityController == null)
            {
                return false;
            }

            Transform trayContainer = ResolveTrayContainer(capacityController.TrayRootTransform);
            return trayContainer != null && TrayOccupancyQuery.CountReservedTrayTiles(trayContainer) == 0;
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
