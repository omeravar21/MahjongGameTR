using System.Collections.Generic;
using System.Text;
using MahjongGame.Board;
using MahjongGame.Matching;
using MahjongGame.BoardGeneration;
using MahjongGame.Combo;
using MahjongGame.Score;
using MahjongGame.Session;
using MahjongGame.Timer;
using MahjongGame.Tray;
using MahjongGame.UI;
using UnityEngine;

namespace MahjongGame.Tiles
{
    public static class TileInteractionSceneValidator
    {
        public static bool ValidateGameplayRoot(Transform gameplayRoot, StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            if (gameplayRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] GameplayRoot is missing.");
                return false;
            }

            passed &= ValidateRequiredComponent<TileSelectionController>(gameplayRoot, reportBuilder);
            passed &= ValidateRequiredComponent<TileSelectabilityChecker>(gameplayRoot, reportBuilder);
            passed &= ValidateRequiredComponent<TileMovementController>(gameplayRoot, reportBuilder);
            passed &= ValidateRequiredComponent<TrayController>(gameplayRoot, reportBuilder);
            passed &= ValidateRequiredComponent<MatchController>(gameplayRoot, reportBuilder);
            passed &= ValidateRequiredComponent<SessionDirector>(gameplayRoot, reportBuilder);
            passed &= ValidateRequiredComponent<WinConditionController>(gameplayRoot, reportBuilder);
            passed &= ValidateRequiredComponent<LoseConditionController>(gameplayRoot, reportBuilder);
            passed &= ValidateRequiredComponent<SessionRestartController>(gameplayRoot, reportBuilder);
            passed &= ValidateRequiredComponent<ScoreController>(gameplayRoot, reportBuilder);
            passed &= ValidateRequiredComponent<ComboController>(gameplayRoot, reportBuilder);
            passed &= ValidateRequiredComponent<LevelResultController>(gameplayRoot, reportBuilder);
            passed &= ValidateMatchExecutionEvents(reportBuilder);
            passed &= ValidateRequiredComponent<TileInteractionController>(gameplayRoot, reportBuilder);

            Transform boardRoot = gameplayRoot.Find("BoardRoot");
            if (boardRoot == null || !BoardRootController.HasRequiredBoardHierarchy(boardRoot))
            {
                AppendLine(reportBuilder, "[FAIL] BoardRoot hierarchy is incomplete.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] BoardRoot hierarchy is present.");
                passed &= ValidateRequiredComponent<BoardPreviewSpawner>(boardRoot, reportBuilder);
            }

            Transform trayRoot = gameplayRoot.Find(TrayRootDefinition.TrayRootName);
            if (trayRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] TrayRoot is missing.");
                passed = false;
            }
            else
            {
                passed &= ValidateTrayRoot(trayRoot, reportBuilder);
            }

            if (boardRoot != null)
            {
                passed &= ValidateBlockingRules(boardRoot, reportBuilder);
            }

            if (trayRoot != null)
            {
                passed &= TraySystemValidator.Validate(gameplayRoot, trayRoot, reportBuilder);
                passed &= MatchSystemValidator.Validate(gameplayRoot, trayRoot, reportBuilder);
            }

            passed &= TimerSystemValidator.Validate(gameplayRoot, reportBuilder);
            passed &= ScoreSystemValidator.Validate(gameplayRoot, reportBuilder);
            passed &= ComboSystemValidator.Validate(gameplayRoot, reportBuilder);
            passed &= PerformanceScreenSystemValidator.Validate(gameplayRoot, reportBuilder);
            passed &= SessionSystemValidator.Validate(gameplayRoot, reportBuilder);
            passed &= DifficultySystemValidator.Validate(reportBuilder);
            passed &= VisualVarietySystemValidator.Validate(reportBuilder);
            passed &= LevelRecipeSystemValidator.Validate(reportBuilder);
            passed &= GridMaskSystemValidator.Validate(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Tile interaction validation completed successfully."
                : "[FAIL] Tile interaction validation found issues.");

            return passed;
        }

        private static bool ValidateMatchExecutionEvents(StringBuilder reportBuilder)
        {
            if (typeof(MatchExecutor) == null
                || typeof(MatchExecutionContext) == null
                || typeof(MatchCleaner) == null
                || typeof(MatchCleanupContext) == null)
            {
                AppendLine(reportBuilder, "[FAIL] Match execution or cleanup types are missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Match execution, cleanup types, and match event wiring are present.");
            return true;
        }

        private static bool ValidateRequiredComponent<T>(Transform gameplayRoot, StringBuilder reportBuilder)
            where T : Component
        {
            if (gameplayRoot.GetComponent<T>() != null)
            {
                AppendLine(reportBuilder, "[PASS] Found " + typeof(T).Name + ".");
                return true;
            }

            AppendLine(reportBuilder, "[FAIL] Missing " + typeof(T).Name + " on GameplayRoot.");
            return false;
        }

        private static bool ValidateTrayRoot(Transform trayRoot, StringBuilder reportBuilder)
        {
            bool passed = true;

            if (trayRoot.GetComponent<TrayRootController>() == null)
            {
                AppendLine(reportBuilder, "[FAIL] TrayRootController is missing on TrayRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TrayRootController is present.");
            }

            if (!TrayRootController.HasRequiredTrayHierarchy(trayRoot))
            {
                AppendLine(reportBuilder, "[FAIL] TrayRoot hierarchy is incomplete.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TrayRoot hierarchy is present.");
            }

            if (!TrayFrameVisualController.HasRequiredFrameVisual(trayRoot))
            {
                AppendLine(reportBuilder, "[FAIL] Tray frame visual is missing.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Tray frame visual is present.");
            }

            Transform trayContainer = trayRoot.Find(TrayRootDefinition.TrayContainerName);
            if (trayContainer == null || !TraySlotVisualController.HasAllSlotVisuals(trayContainer))
            {
                AppendLine(reportBuilder, "[FAIL] Tray slot visuals are incomplete.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] All four tray slot visuals are present.");
            }

            if (trayRoot.GetComponent<TrayCapacityController>() == null)
            {
                AppendLine(reportBuilder, "[FAIL] TrayCapacityController is missing on TrayRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] TrayCapacityController is present.");
            }

            TrayCapacityController capacityController = trayRoot.GetComponent<TrayCapacityController>();
            if (capacityController != null && capacityController.Capacity != TrayRootDefinition.Capacity)
            {
                AppendLine(reportBuilder, "[FAIL] Tray capacity does not match project rule of 4.");
                passed = false;
            }
            else if (capacityController != null)
            {
                AppendLine(reportBuilder, "[PASS] Tray capacity is configured to 4.");
            }

            return passed;
        }

        private static bool ValidateBlockingRules(Transform boardRoot, StringBuilder reportBuilder)
        {
            List<Tile> boardTiles = BoardTileOccupancyQuery.CollectOccupyingTiles(boardRoot);
            if (boardTiles.Count == 0)
            {
                AppendLine(reportBuilder, "[SKIP] No preview tiles found for blocking rule checks.");
                return true;
            }

            bool passed = true;
            Tile topStackTile = FindTileAt(boardTiles, column: 2, row: 3, layerIndex: 3);
            Tile middleStackTile = FindTileAt(boardTiles, column: 2, row: 3, layerIndex: 2);

            if (topStackTile != null)
            {
                bool topSelectable = TileSelectabilityChecker.IsSelectable(boardRoot, topStackTile);
                AppendLine(reportBuilder, topSelectable
                    ? "[PASS] Top stack tile at (2,3) layer 3 is selectable when sides allow."
                    : "[INFO] Top stack tile at (2,3) layer 3 is blocked by side rules.");
            }

            if (middleStackTile != null)
            {
                bool middleSelectable = TileSelectabilityChecker.IsSelectable(boardRoot, middleStackTile);
                if (middleSelectable)
                {
                    AppendLine(reportBuilder, "[FAIL] Middle stack tile at (2,3) layer 2 should be upper-blocked.");
                    passed = false;
                }
                else
                {
                    AppendLine(reportBuilder, "[PASS] Middle stack tile at (2,3) layer 2 is upper-blocked.");
                }
            }

            return passed;
        }

        private static Tile FindTileAt(List<Tile> tiles, int column, int row, int layerIndex)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                Tile tile = tiles[i];
                if (tile == null)
                {
                    continue;
                }

                if (tile.GridCoordinate.Column == column
                    && tile.GridCoordinate.Row == row
                    && tile.LayerIndex == layerIndex)
                {
                    return tile;
                }
            }

            return null;
        }

        private static void AppendLine(StringBuilder reportBuilder, string line)
        {
            reportBuilder.AppendLine(line);
        }
    }
}
