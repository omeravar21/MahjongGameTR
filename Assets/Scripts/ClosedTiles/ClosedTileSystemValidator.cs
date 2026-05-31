using System.Reflection;
using System.Text;
using MahjongGame.Board;
using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.ClosedTiles
{
    public static class ClosedTileSystemValidator
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
                AppendLine(reportBuilder, "[FAIL] GameplayRoot is missing for closed tile system validation.");
                return false;
            }

            ClosedTileController closedTileController = gameplayRoot.GetComponent<ClosedTileController>();

            passed &= ValidateComponents(closedTileController, reportBuilder);
            passed &= ValidateTypesAndEvents(reportBuilder);
            passed &= ValidateDefinition(reportBuilder);
            passed &= ValidateRegistryBehavior(closedTileController, reportBuilder);
            passed &= ValidateRevealBehavior(closedTileController, reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Closed tile system validation completed successfully."
                : "[FAIL] Closed tile system validation found issues.");

            return passed;
        }

        private static bool ValidateComponents(
            ClosedTileController closedTileController,
            StringBuilder reportBuilder)
        {
            if (closedTileController == null)
            {
                AppendLine(reportBuilder, "[FAIL] ClosedTileController is missing on GameplayRoot.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] ClosedTileController is present on GameplayRoot.");
            return true;
        }

        private static bool ValidateTypesAndEvents(StringBuilder reportBuilder)
        {
            bool passed = true;

            AppendLine(reportBuilder, typeof(ClosedTileData) != null
                ? "[PASS] ClosedTileData type is present."
                : "[FAIL] ClosedTileData type is missing.");

            AppendLine(reportBuilder, typeof(ClosedTileState) != null
                ? "[PASS] ClosedTileState type is present."
                : "[FAIL] ClosedTileState type is missing.");

            AppendLine(reportBuilder, typeof(ClosedTileDefinition) != null
                ? "[PASS] ClosedTileDefinition type is present."
                : "[FAIL] ClosedTileDefinition type is missing.");

            passed &= ValidateEventExists(
                typeof(ClosedTileEvents),
                nameof(ClosedTileEvents.ClosedTileRegistered),
                reportBuilder);
            passed &= ValidateEventExists(
                typeof(ClosedTileEvents),
                nameof(ClosedTileEvents.ClosedTileStateChanged),
                reportBuilder);
            passed &= ValidateEventExists(
                typeof(ClosedTileEvents),
                nameof(ClosedTileEvents.ClosedTileRuntimeReset),
                reportBuilder);

            return passed;
        }

        private static bool ValidateDefinition(StringBuilder reportBuilder)
        {
            if (ClosedTileDefinition.ActivationLevel != 10)
            {
                AppendLine(reportBuilder, "[FAIL] ClosedTileDefinition activation level is not 10.");
                return false;
            }

            if (ClosedTileDefinition.IsClosedTileMechanicActive(9))
            {
                AppendLine(reportBuilder, "[FAIL] Closed tile mechanic is active before level 10.");
                return false;
            }

            if (!ClosedTileDefinition.IsClosedTileMechanicActive(10))
            {
                AppendLine(reportBuilder, "[FAIL] Closed tile mechanic is inactive at level 10.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] ClosedTileDefinition activation rules are valid.");
            return true;
        }

        private static bool ValidateRegistryBehavior(
            ClosedTileController closedTileController,
            StringBuilder reportBuilder)
        {
            if (closedTileController == null)
            {
                AppendLine(reportBuilder, "[FAIL] ClosedTileController is unavailable for registry validation.");
                return false;
            }

            closedTileController.ResetRuntimeState();

            if (closedTileController.GetRegisteredClosedTileCount() != 0)
            {
                AppendLine(reportBuilder, "[FAIL] Closed tile registry is not empty after reset.");
                return false;
            }

            TileBoardPosition boardPosition = new TileBoardPosition(new BoardGridCoordinate(1, 1), 0);
            if (!ClosedTileData.TryCreate(500, 12, boardPosition, out ClosedTileData closedTileData))
            {
                AppendLine(reportBuilder, "[FAIL] Synthetic ClosedTileData could not be created.");
                return false;
            }

            if (!closedTileController.TryRegisterClosedTile(closedTileData))
            {
                AppendLine(reportBuilder, "[FAIL] ClosedTileController rejected synthetic registration.");
                return false;
            }

            if (closedTileController.GetRegisteredClosedTileCount() != 1
                || !closedTileController.IsClosedTile(500))
            {
                AppendLine(reportBuilder, "[FAIL] Closed tile registry count is incorrect after registration.");
                return false;
            }

            if (!closedTileController.TryGetClosedTileData(500, out ClosedTileData registeredData)
                || registeredData.State != ClosedTileState.Closed)
            {
                AppendLine(reportBuilder, "[FAIL] Closed tile registry lookup failed after registration.");
                return false;
            }

            if (!closedTileController.TrySetClosedTileStateForValidation(500, ClosedTileState.Revealed)
                || !closedTileController.HasRevealedClosedTile)
            {
                AppendLine(reportBuilder, "[FAIL] Closed tile state transition validation failed.");
                return false;
            }

            closedTileController.ResetRuntimeState();

            if (closedTileController.GetRegisteredClosedTileCount() != 0
                || closedTileController.HasRevealedClosedTile)
            {
                AppendLine(reportBuilder, "[FAIL] Closed tile registry did not reset cleanly.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Closed tile registry registration and reset behave correctly.");
            return true;
        }

        private static bool ValidateRevealBehavior(
            ClosedTileController closedTileController,
            StringBuilder reportBuilder)
        {
            if (closedTileController == null)
            {
                AppendLine(reportBuilder, "[FAIL] ClosedTileController is unavailable for reveal validation.");
                return false;
            }

            closedTileController.ResetRuntimeState();

            GameObject tempTileObject = new GameObject("ClosedTileRevealValidationTile");
            Tile tile = tempTileObject.AddComponent<Tile>();
            TileData tileData = new TileData(
                501,
                new BoardGridCoordinate(2, 2),
                0,
                TileType.Closed,
                isClosed: true,
                symbolId: 5);
            tile.Initialize(tileData);

            bool passed = true;

            if (!closedTileController.TryRegisterClosedTile(tile)
                || closedTileController.GetRegisteredClosedTileCount() != 1
                || tile.State != TileState.Closed)
            {
                AppendLine(reportBuilder, "[FAIL] Closed tile registration for reveal validation failed.");
                passed = false;
            }

            if (passed && !closedTileController.TryRevealClosedTile(tile))
            {
                AppendLine(reportBuilder, "[FAIL] TryRevealClosedTile did not reveal a registered closed tile.");
                passed = false;
            }

            if (passed
                && (!closedTileController.TryGetClosedTileState(501, out ClosedTileState revealedState)
                    || revealedState != ClosedTileState.Revealed
                    || tile.State != TileState.Revealed
                    || !closedTileController.HasRevealedClosedTile))
            {
                AppendLine(reportBuilder, "[FAIL] Closed tile reveal did not update registry and tile state.");
                passed = false;
            }

            if (passed && closedTileController.TryRevealClosedTile(tile))
            {
                AppendLine(reportBuilder, "[FAIL] TryRevealClosedTile should not reveal an already revealed tile.");
                passed = false;
            }

            if (passed && !closedTileController.RequiresTrayMove(tile))
            {
                AppendLine(reportBuilder, "[FAIL] RequiresTrayMove should be true for a revealed closed tile.");
                passed = false;
            }

            closedTileController.ResetRuntimeState();
            Object.DestroyImmediate(tempTileObject);

            if (closedTileController.GetRegisteredClosedTileCount() != 0
                || closedTileController.HasRevealedClosedTile)
            {
                AppendLine(reportBuilder, "[FAIL] Closed tile registry did not reset cleanly after reveal validation.");
                passed = false;
            }

            AppendLine(reportBuilder, passed
                ? "[PASS] Closed tile reveal behavior is valid."
                : "[FAIL] Closed tile reveal behavior validation failed.");

            return passed;
        }

        private static bool ValidateEventExists(
            System.Type eventsType,
            string eventName,
            StringBuilder reportBuilder)
        {
            EventInfo eventInfo = eventsType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Static);
            if (eventInfo == null)
            {
                AppendLine(reportBuilder, "[FAIL] Event " + eventName + " is missing on " + eventsType.Name + ".");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Event " + eventName + " is present on " + eventsType.Name + ".");
            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
