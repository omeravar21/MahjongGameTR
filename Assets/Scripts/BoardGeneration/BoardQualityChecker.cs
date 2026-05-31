using System.Collections.Generic;
using MahjongGame.Board;
using MahjongGame.Tiles;

namespace MahjongGame.BoardGeneration
{
    public static class BoardQualityChecker
    {
        public const int MinimumSelectableTiles = 8;
        public const int MinimumTileCount = 18;

        public static BoardQualityCheckResult Check(BoardData boardData)
        {
            bool gridIntegrityPassed = ValidateGridIntegrity(boardData, out string gridFailureReason);
            if (!gridIntegrityPassed)
            {
                return CreateResult(false, gridIntegrityPassed, false, false, false, false, false, false, false, false, gridFailureReason);
            }

            bool layerIntegrityPassed = ValidateLayerIntegrity(boardData, out string layerFailureReason);
            if (!layerIntegrityPassed)
            {
                return CreateResult(false, true, layerIntegrityPassed, false, false, false, false, false, false, false, layerFailureReason);
            }

            bool tilePairValidityPassed = ValidateTilePairValidity(boardData, out string pairFailureReason);
            if (!tilePairValidityPassed)
            {
                return CreateResult(false, true, true, tilePairValidityPassed, false, false, false, false, false, false, pairFailureReason);
            }

            bool densityPassed = ValidateDensity(boardData, out string densityFailureReason);
            if (!densityPassed)
            {
                return CreateResult(false, true, true, true, false, false, false, false, false, densityPassed, densityFailureReason);
            }

            OpeningMoveCheckResult openingMoveResult = OpeningMoveChecker.Check(boardData);
            bool openingMovePassed = openingMoveResult.IsValid;
            if (!openingMovePassed)
            {
                return CreateResult(
                    false,
                    true,
                    true,
                    true,
                    false,
                    false,
                    false,
                    false,
                    false,
                    true,
                    openingMoveResult.FailureReason);
            }

            List<TileSymbolAssignment> selectableAssignments = BoardDataLayoutQuery.GetSelectableAssignments(boardData);
            bool selectableCountPassed = selectableAssignments.Count >= MinimumSelectableTiles;
            if (!selectableCountPassed)
            {
                return CreateResult(
                    false,
                    true,
                    true,
                    true,
                    true,
                    false,
                    false,
                    false,
                    false,
                    true,
                    "Selectable tile count is below the launch minimum.");
            }

            DeadlockRiskCheckResult deadlockRiskResult = DeadlockRiskChecker.Check(boardData);
            bool deadlockRiskPassed = deadlockRiskResult.IsValid;
            if (!deadlockRiskPassed)
            {
                return CreateResult(
                    false,
                    true,
                    true,
                    true,
                    true,
                    true,
                    false,
                    false,
                    false,
                    true,
                    deadlockRiskResult.FailureReason);
            }

            bool closedTileFairnessPassed = ValidateClosedTileFairness(boardData, out string closedFailureReason);
            if (!closedTileFairnessPassed)
            {
                return CreateResult(
                    false,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    false,
                    false,
                    true,
                    closedFailureReason);
            }

            bool jokerAccessibilityPassed = ValidateJokerAccessibility(boardData, out string jokerFailureReason);
            if (!jokerAccessibilityPassed)
            {
                return CreateResult(
                    false,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    false,
                    true,
                    jokerFailureReason);
            }

            return CreateResult(true, true, true, true, true, true, true, true, true, true, string.Empty);
        }

        private static bool ValidateGridIntegrity(BoardData boardData, out string failureReason)
        {
            failureReason = string.Empty;

            if (boardData?.TileAssignments == null || boardData.TileCount <= 0)
            {
                failureReason = "Board data is empty.";
                return false;
            }

            for (int index = 0; index < boardData.TileAssignments.Count; index++)
            {
                TileSymbolAssignment assignment = boardData.TileAssignments[index];
                if (!assignment.Position.IsValid
                    || !BoardGridDefinition.IsValidCoordinate(
                        assignment.Position.GridCoordinate.Column,
                        assignment.Position.GridCoordinate.Row))
                {
                    failureReason = "Board contains an invalid grid coordinate.";
                    return false;
                }
            }

            return true;
        }

        private static bool ValidateLayerIntegrity(BoardData boardData, out string failureReason)
        {
            failureReason = string.Empty;

            if (boardData?.TileAssignments == null)
            {
                failureReason = "Board assignments are missing.";
                return false;
            }

            HashSet<TileBoardPosition> uniquePositions = new HashSet<TileBoardPosition>();
            for (int index = 0; index < boardData.TileAssignments.Count; index++)
            {
                TileSymbolAssignment assignment = boardData.TileAssignments[index];
                if (!BoardLayerDefinition.IsValidLayerIndex(assignment.Position.LayerIndex))
                {
                    failureReason = "Board contains an invalid layer index.";
                    return false;
                }

                if (!uniquePositions.Add(assignment.Position))
                {
                    failureReason = "Board contains duplicate tile positions.";
                    return false;
                }
            }

            return true;
        }

        private static bool ValidateTilePairValidity(BoardData boardData, out string failureReason)
        {
            failureReason = string.Empty;

            if (boardData?.TileAssignments == null || boardData.TileCount <= 0)
            {
                failureReason = "Board data is empty.";
                return false;
            }

            if (boardData.TileCount % 2 != 0)
            {
                failureReason = "Board tile count is not pairable.";
                return false;
            }

            Dictionary<int, int> symbolCounts = new Dictionary<int, int>();
            for (int index = 0; index < boardData.TileAssignments.Count; index++)
            {
                int symbolId = boardData.TileAssignments[index].SymbolId;
                if (!symbolCounts.ContainsKey(symbolId))
                {
                    symbolCounts[symbolId] = 0;
                }

                symbolCounts[symbolId]++;
            }

            foreach (KeyValuePair<int, int> entry in symbolCounts)
            {
                if (entry.Value != 2)
                {
                    failureReason = "Board contains invalid symbol pair counts.";
                    return false;
                }
            }

            return true;
        }

        private static bool ValidateDensity(BoardData boardData, out string failureReason)
        {
            failureReason = string.Empty;

            if (boardData == null || boardData.TileCount < MinimumTileCount)
            {
                failureReason = "Board tile count is below the launch density minimum.";
                return false;
            }

            return true;
        }

        private static bool ValidateClosedTileFairness(BoardData boardData, out string failureReason)
        {
            failureReason = string.Empty;

            if (boardData == null)
            {
                failureReason = "Board data is missing.";
                return false;
            }

            if (boardData.ClosedTileCount <= 0)
            {
                return true;
            }

            if (boardData.TileAssignments == null)
            {
                failureReason = "Board assignments are missing for closed tile validation.";
                return false;
            }

            int closedAssignmentCount = 0;
            for (int index = 0; index < boardData.TileAssignments.Count; index++)
            {
                if (boardData.TileAssignments[index].IsClosed)
                {
                    closedAssignmentCount++;
                }
            }

            if (closedAssignmentCount != boardData.ClosedTileCount)
            {
                failureReason = "Closed tile assignment count does not match recipe closed tile count.";
                return false;
            }

            return true;
        }

        private static bool ValidateJokerAccessibility(BoardData boardData, out string failureReason)
        {
            failureReason = string.Empty;

            if (boardData == null)
            {
                failureReason = "Board data is missing.";
                return false;
            }

            if (boardData.JokerCount <= 0)
            {
                return true;
            }

            int jokerAssignmentCount = CountJokerAssignments(boardData.TileAssignments);
            if (jokerAssignmentCount != boardData.JokerCount)
            {
                failureReason = "Joker assignment count does not match recipe joker count.";
                return false;
            }

            return true;
        }

        private static int CountJokerAssignments(IReadOnlyList<TileSymbolAssignment> assignments)
        {
            if (assignments == null)
            {
                return 0;
            }

            int count = 0;
            for (int index = 0; index < assignments.Count; index++)
            {
                if (assignments[index].IsJoker)
                {
                    count++;
                }
            }

            return count;
        }

        private static BoardQualityCheckResult CreateResult(
            bool isValid,
            bool gridIntegrityPassed,
            bool layerIntegrityPassed,
            bool tilePairValidityPassed,
            bool openingMovePassed,
            bool selectableCountPassed,
            bool deadlockRiskPassed,
            bool closedTileFairnessPassed,
            bool jokerAccessibilityPassed,
            bool densityPassed,
            string failureReason)
        {
            return new BoardQualityCheckResult(
                isValid,
                gridIntegrityPassed,
                layerIntegrityPassed,
                tilePairValidityPassed,
                openingMovePassed,
                selectableCountPassed,
                deadlockRiskPassed,
                closedTileFairnessPassed,
                jokerAccessibilityPassed,
                densityPassed,
                failureReason);
        }
    }
}
