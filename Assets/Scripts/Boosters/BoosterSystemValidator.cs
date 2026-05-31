using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MahjongGame.Board;
using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.Boosters
{
    public static class BoosterSystemValidator
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
                AppendLine(reportBuilder, "[FAIL] GameplayRoot is missing for booster system validation.");
                return false;
            }

            Transform boosterRoot = gameplayRoot.Find("BoosterRoot");
            BoosterEconomyDirector economyDirector = gameplayRoot.GetComponent<BoosterEconomyDirector>();
            BoosterController boosterController = boosterRoot != null
                ? boosterRoot.GetComponent<BoosterController>()
                : null;
            ShuffleBooster shuffleBooster = boosterRoot != null
                ? boosterRoot.GetComponent<ShuffleBooster>()
                : null;
            UndoBooster undoBooster = boosterRoot != null
                ? boosterRoot.GetComponent<UndoBooster>()
                : null;
            HintBooster hintBooster = boosterRoot != null
                ? boosterRoot.GetComponent<HintBooster>()
                : null;

            passed &= ValidateRoots(boosterRoot, reportBuilder);
            passed &= ValidateComponents(
                economyDirector,
                boosterController,
                shuffleBooster,
                undoBooster,
                hintBooster,
                reportBuilder);
            passed &= ValidateTypesAndEvents(reportBuilder);
            passed &= BoosterEconomySystemValidator.Validate(gameplayRoot, reportBuilder);
            passed &= ValidateControllerWiring(
                boosterController,
                economyDirector,
                shuffleBooster,
                undoBooster,
                hintBooster,
                reportBuilder);
            passed &= ValidateShuffleBehavior(shuffleBooster, gameplayRoot, reportBuilder);
            passed &= ValidateUndoBehavior(undoBooster, reportBuilder);
            passed &= ValidateHintBehavior(hintBooster, gameplayRoot, reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Booster system validation completed successfully."
                : "[FAIL] Booster system validation found issues.");

            return passed;
        }

        private static bool ValidateRoots(Transform boosterRoot, StringBuilder reportBuilder)
        {
            if (boosterRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoosterRoot is missing under GameplayRoot.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] BoosterRoot is present under GameplayRoot.");
            return true;
        }

        private static bool ValidateComponents(
            BoosterEconomyDirector economyDirector,
            BoosterController boosterController,
            ShuffleBooster shuffleBooster,
            UndoBooster undoBooster,
            HintBooster hintBooster,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            if (economyDirector == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoosterEconomyDirector is missing on GameplayRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] BoosterEconomyDirector is present on GameplayRoot.");
            }

            if (boosterController == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoosterController is missing on BoosterRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] BoosterController is present on BoosterRoot.");
            }

            if (shuffleBooster == null)
            {
                AppendLine(reportBuilder, "[FAIL] ShuffleBooster is missing on BoosterRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] ShuffleBooster is present on BoosterRoot.");
            }

            if (undoBooster == null)
            {
                AppendLine(reportBuilder, "[FAIL] UndoBooster is missing on BoosterRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] UndoBooster is present on BoosterRoot.");
            }

            if (hintBooster == null)
            {
                AppendLine(reportBuilder, "[FAIL] HintBooster is missing on BoosterRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] HintBooster is present on BoosterRoot.");
            }

            return passed;
        }

        private static bool ValidateTypesAndEvents(StringBuilder reportBuilder)
        {
            bool passed = true;

            AppendLine(reportBuilder, typeof(UndoMoveRecord) != null
                ? "[PASS] UndoMoveRecord type is present."
                : "[FAIL] UndoMoveRecord type is missing.");

            AppendLine(reportBuilder, typeof(HintPresentationContext) != null
                ? "[PASS] HintPresentationContext type is present."
                : "[FAIL] HintPresentationContext type is missing.");

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
            passed &= ValidateEventExists(
                typeof(BoosterEvents),
                nameof(BoosterEvents.ShuffleExecuted),
                reportBuilder);

            return passed;
        }

        private static bool ValidateControllerWiring(
            BoosterController boosterController,
            BoosterEconomyDirector economyDirector,
            ShuffleBooster shuffleBooster,
            UndoBooster undoBooster,
            HintBooster hintBooster,
            StringBuilder reportBuilder)
        {
            if (boosterController == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoosterController wiring cannot be validated without component.");
                return false;
            }

            bool passed = true;

            if (boosterController.GetEconomyDirector() != economyDirector)
            {
                AppendLine(reportBuilder, "[FAIL] BoosterController is not wired to BoosterEconomyDirector.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] BoosterController is wired to BoosterEconomyDirector.");
            }

            if (boosterController.GetShuffleBooster() != shuffleBooster)
            {
                AppendLine(reportBuilder, "[FAIL] BoosterController is not wired to ShuffleBooster.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] BoosterController is wired to ShuffleBooster.");
            }

            if (boosterController.GetUndoBooster() != undoBooster)
            {
                AppendLine(reportBuilder, "[FAIL] BoosterController is not wired to UndoBooster.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] BoosterController is wired to UndoBooster.");
            }

            if (boosterController.GetHintBooster() != hintBooster)
            {
                AppendLine(reportBuilder, "[FAIL] BoosterController is not wired to HintBooster.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] BoosterController is wired to HintBooster.");
            }

            return passed;
        }

        private static bool ValidateShuffleBehavior(
            ShuffleBooster shuffleBooster,
            Transform gameplayRoot,
            StringBuilder reportBuilder)
        {
            if (shuffleBooster == null || gameplayRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] ShuffleBooster is unavailable for shuffle validation.");
                return false;
            }

            Transform boardRoot = gameplayRoot.Find("BoardRoot");
            if (boardRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoardRoot is missing for shuffle validation.");
                return false;
            }

            List<Tile> boardTiles = CollectOnBoardTiles(boardRoot);
            if (boardTiles.Count < 2)
            {
                AppendLine(reportBuilder, "[SKIP] Not enough on-board tiles found for shuffle validation.");
                return true;
            }

            Dictionary<int, int> symbolCountsBefore = BuildSymbolMultiset(boardTiles);
            Dictionary<int, Vector3> positionsBefore = BuildTilePositions(boardTiles);
            List<int> symbolsBefore = CollectSymbolIds(boardTiles);
            bool expectSymbolOrderChange = HasAtLeastTwoDistinctSymbols(symbolCountsBefore);

            if (!shuffleBooster.TryExecuteShuffle(out int shuffledTileCount))
            {
                AppendLine(reportBuilder, "[FAIL] ShuffleBooster failed to execute shuffle.");
                return false;
            }

            if (shuffledTileCount < 2)
            {
                AppendLine(reportBuilder, "[FAIL] ShuffleBooster reported an invalid shuffled tile count.");
                return false;
            }

            Dictionary<int, int> symbolCountsAfter = BuildSymbolMultiset(boardTiles);
            if (!MultisetsEqual(symbolCountsBefore, symbolCountsAfter))
            {
                AppendLine(reportBuilder, "[FAIL] Shuffle changed the symbol multiset.");
                return false;
            }

            if (!PositionsUnchanged(boardTiles, positionsBefore))
            {
                AppendLine(reportBuilder, "[FAIL] Shuffle changed tile positions.");
                return false;
            }

            List<int> symbolsAfter = CollectSymbolIds(boardTiles);
            if (expectSymbolOrderChange && !SymbolOrderPossiblyChanged(symbolsBefore, symbolsAfter))
            {
                AppendLine(reportBuilder, "[FAIL] Shuffle did not redistribute symbols.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Shuffle preserves positions and multiset while changing symbols.");
            return true;
        }

        private static bool HasAtLeastTwoDistinctSymbols(Dictionary<int, int> symbolCounts)
        {
            int distinctSymbolCount = 0;
            foreach (KeyValuePair<int, int> pair in symbolCounts)
            {
                if (pair.Value <= 0)
                {
                    continue;
                }

                distinctSymbolCount++;
                if (distinctSymbolCount >= 2)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ValidateUndoBehavior(UndoBooster undoBooster, StringBuilder reportBuilder)
        {
            if (undoBooster == null)
            {
                AppendLine(reportBuilder, "[FAIL] UndoBooster is unavailable for undo validation.");
                return false;
            }

            TileBoardPosition boardPosition = new TileBoardPosition(new BoardGridCoordinate(1, 1), 0);
            UndoMoveRecord moveRecord = new UndoMoveRecord(
                null,
                slotIndex: 0,
                boardPosition,
                TileState.OnBoard);

            if (moveRecord.CanUndo())
            {
                AppendLine(reportBuilder, "[FAIL] UndoMoveRecord accepted an invalid undo target.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] UndoMoveRecord rejects invalid undo targets.");
            AppendLine(reportBuilder, "[PASS] UndoBooster is present and undo record validation behaves correctly.");
            return true;
        }

        private static bool ValidateHintBehavior(
            HintBooster hintBooster,
            Transform gameplayRoot,
            StringBuilder reportBuilder)
        {
            if (hintBooster == null || gameplayRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] HintBooster is unavailable for hint validation.");
                return false;
            }

            Transform boardRoot = gameplayRoot.Find("BoardRoot");
            if (boardRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoardRoot is missing for hint validation.");
                return false;
            }

            if (!hintBooster.TryFindSelectablePairForValidation(
                    boardRoot,
                    out Tile firstTile,
                    out Tile secondTile))
            {
                AppendLine(reportBuilder, "[SKIP] No selectable matching pair found for hint validation.");
                return true;
            }

            if (firstTile == null
                || secondTile == null
                || firstTile.SymbolId != secondTile.SymbolId)
            {
                AppendLine(reportBuilder, "[FAIL] Hint pair validation returned invalid matching tiles.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] HintBooster identifies a valid selectable matching pair.");
            return true;
        }

        private static List<Tile> CollectOnBoardTiles(Transform boardRoot)
        {
            List<Tile> allTiles = BoardTileOccupancyQuery.CollectOccupyingTiles(boardRoot);
            List<Tile> onBoardTiles = new List<Tile>(allTiles.Count);

            for (int index = 0; index < allTiles.Count; index++)
            {
                Tile tile = allTiles[index];
                if (tile == null)
                {
                    continue;
                }

                switch (tile.State)
                {
                    case TileState.OnBoard:
                    case TileState.Closed:
                    case TileState.Revealed:
                        onBoardTiles.Add(tile);
                        break;
                }
            }

            return onBoardTiles;
        }

        private static Dictionary<int, int> BuildSymbolMultiset(IReadOnlyList<Tile> tiles)
        {
            Dictionary<int, int> counts = new Dictionary<int, int>();
            for (int index = 0; index < tiles.Count; index++)
            {
                Tile tile = tiles[index];
                if (tile == null || !tile.HasAssignedSymbol)
                {
                    continue;
                }

                int symbolId = tile.SymbolId;
                counts.TryGetValue(symbolId, out int currentCount);
                counts[symbolId] = currentCount + 1;
            }

            return counts;
        }

        private static Dictionary<int, Vector3> BuildTilePositions(IReadOnlyList<Tile> tiles)
        {
            Dictionary<int, Vector3> positions = new Dictionary<int, Vector3>();
            for (int index = 0; index < tiles.Count; index++)
            {
                Tile tile = tiles[index];
                if (tile == null)
                {
                    continue;
                }

                positions[tile.TileId] = tile.transform.position;
            }

            return positions;
        }

        private static List<int> CollectSymbolIds(IReadOnlyList<Tile> tiles)
        {
            List<int> symbolIds = new List<int>();
            for (int index = 0; index < tiles.Count; index++)
            {
                Tile tile = tiles[index];
                if (tile == null || !tile.HasAssignedSymbol)
                {
                    continue;
                }

                symbolIds.Add(tile.SymbolId);
            }

            return symbolIds;
        }

        private static bool MultisetsEqual(
            Dictionary<int, int> left,
            Dictionary<int, int> right)
        {
            if (left.Count != right.Count)
            {
                return false;
            }

            foreach (KeyValuePair<int, int> pair in left)
            {
                if (!right.TryGetValue(pair.Key, out int rightCount) || rightCount != pair.Value)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool PositionsUnchanged(
            IReadOnlyList<Tile> tiles,
            Dictionary<int, Vector3> positionsBefore)
        {
            for (int index = 0; index < tiles.Count; index++)
            {
                Tile tile = tiles[index];
                if (tile == null)
                {
                    continue;
                }

                if (!positionsBefore.TryGetValue(tile.TileId, out Vector3 previousPosition))
                {
                    continue;
                }

                if ((tile.transform.position - previousPosition).sqrMagnitude > 0.0001f)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool SymbolOrderPossiblyChanged(IReadOnlyList<int> before, IReadOnlyList<int> after)
        {
            if (before.Count != after.Count || before.Count < 2)
            {
                return false;
            }

            for (int index = 0; index < before.Count; index++)
            {
                if (before[index] != after[index])
                {
                    return true;
                }
            }

            return false;
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
