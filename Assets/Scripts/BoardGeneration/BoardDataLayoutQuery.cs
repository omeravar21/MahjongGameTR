using System.Collections.Generic;
using MahjongGame.Board;
using MahjongGame.Tiles;

namespace MahjongGame.BoardGeneration
{
    public static class BoardDataLayoutQuery
    {
        public static bool IsCellOccupied(
            IReadOnlyList<TileSymbolAssignment> assignments,
            int column,
            int row,
            int layerIndex,
            int excludeAssignmentIndex)
        {
            if (assignments == null
                || !BoardGridDefinition.IsValidCoordinate(column, row)
                || !BoardLayerDefinition.IsValidLayerIndex(layerIndex))
            {
                return false;
            }

            for (int index = 0; index < assignments.Count; index++)
            {
                if (index == excludeAssignmentIndex)
                {
                    continue;
                }

                TileSymbolAssignment assignment = assignments[index];
                if (!assignment.Position.IsValid)
                {
                    continue;
                }

                BoardGridCoordinate coordinate = assignment.Position.GridCoordinate;
                if (coordinate.Column == column
                    && coordinate.Row == row
                    && assignment.Position.LayerIndex == layerIndex)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasUpperBlockingTile(
            IReadOnlyList<TileSymbolAssignment> assignments,
            TileSymbolAssignment assignment,
            int assignmentIndex)
        {
            if (assignments == null || !assignment.Position.IsValid)
            {
                return false;
            }

            BoardGridCoordinate coordinate = assignment.Position.GridCoordinate;
            for (int layerIndex = assignment.Position.LayerIndex + 1;
                layerIndex < BoardLayerDefinition.MaxLayerCount;
                layerIndex++)
            {
                if (IsCellOccupied(assignments, coordinate.Column, coordinate.Row, layerIndex, excludeAssignmentIndex: -1))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasBothSidesBlocked(
            IReadOnlyList<TileSymbolAssignment> assignments,
            TileSymbolAssignment assignment,
            int assignmentIndex)
        {
            if (assignments == null || !assignment.Position.IsValid)
            {
                return false;
            }

            BoardGridCoordinate coordinate = assignment.Position.GridCoordinate;
            int layerIndex = assignment.Position.LayerIndex;

            bool leftBlocked = coordinate.Column > 0
                && IsCellOccupied(
                    assignments,
                    coordinate.Column - 1,
                    coordinate.Row,
                    layerIndex,
                    assignmentIndex);
            bool rightBlocked = coordinate.Column < BoardGridDefinition.ColumnCount - 1
                && IsCellOccupied(
                    assignments,
                    coordinate.Column + 1,
                    coordinate.Row,
                    layerIndex,
                    assignmentIndex);

            return leftBlocked && rightBlocked;
        }

        public static bool IsSelectable(
            IReadOnlyList<TileSymbolAssignment> assignments,
            TileSymbolAssignment assignment,
            int assignmentIndex)
        {
            if (assignments == null || !assignment.Position.IsValid)
            {
                return false;
            }

            if (HasUpperBlockingTile(assignments, assignment, assignmentIndex))
            {
                return false;
            }

            if (HasBothSidesBlocked(assignments, assignment, assignmentIndex))
            {
                return false;
            }

            return true;
        }

        public static List<TileSymbolAssignment> GetSelectableAssignments(BoardData boardData)
        {
            List<TileSymbolAssignment> selectableAssignments = new List<TileSymbolAssignment>();
            if (boardData?.TileAssignments == null)
            {
                return selectableAssignments;
            }

            IReadOnlyList<TileSymbolAssignment> assignments = boardData.TileAssignments;
            for (int index = 0; index < assignments.Count; index++)
            {
                TileSymbolAssignment assignment = assignments[index];
                if (IsSelectable(assignments, assignment, index))
                {
                    selectableAssignments.Add(assignment);
                }
            }

            return selectableAssignments;
        }

        public static int CountAccessiblePairs(IReadOnlyList<TileSymbolAssignment> selectableAssignments)
        {
            if (selectableAssignments == null || selectableAssignments.Count == 0)
            {
                return 0;
            }

            Dictionary<int, int> symbolCounts = new Dictionary<int, int>();
            for (int index = 0; index < selectableAssignments.Count; index++)
            {
                int symbolId = selectableAssignments[index].SymbolId;
                if (!symbolCounts.ContainsKey(symbolId))
                {
                    symbolCounts[symbolId] = 0;
                }

                symbolCounts[symbolId]++;
            }

            int accessiblePairCount = 0;
            foreach (KeyValuePair<int, int> entry in symbolCounts)
            {
                accessiblePairCount += entry.Value / 2;
            }

            return accessiblePairCount;
        }

        public static int CountMeaningfulOpeningChoices(IReadOnlyList<TileSymbolAssignment> selectableAssignments)
        {
            if (selectableAssignments == null || selectableAssignments.Count == 0)
            {
                return 0;
            }

            Dictionary<int, int> symbolCounts = new Dictionary<int, int>();
            for (int index = 0; index < selectableAssignments.Count; index++)
            {
                int symbolId = selectableAssignments[index].SymbolId;
                if (!symbolCounts.ContainsKey(symbolId))
                {
                    symbolCounts[symbolId] = 0;
                }

                symbolCounts[symbolId]++;
            }

            int meaningfulOpeningChoiceCount = 0;
            foreach (KeyValuePair<int, int> entry in symbolCounts)
            {
                if (entry.Value >= 2)
                {
                    meaningfulOpeningChoiceCount++;
                }
            }

            return meaningfulOpeningChoiceCount;
        }

        public static int CountSelectableOnLayer(
            BoardData boardData,
            int layerIndex)
        {
            if (boardData?.TileAssignments == null || !BoardLayerDefinition.IsValidLayerIndex(layerIndex))
            {
                return 0;
            }

            IReadOnlyList<TileSymbolAssignment> assignments = boardData.TileAssignments;
            int selectableCount = 0;
            for (int index = 0; index < assignments.Count; index++)
            {
                TileSymbolAssignment assignment = assignments[index];
                if (assignment.Position.LayerIndex == layerIndex
                    && IsSelectable(assignments, assignment, index))
                {
                    selectableCount++;
                }
            }

            return selectableCount;
        }
    }
}
