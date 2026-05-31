using System.Collections.Generic;

namespace MahjongGame.BoardGeneration
{
    public static class OpeningMoveChecker
    {
        public const int MinimumMeaningfulOpeningChoices = 2;

        public static OpeningMoveCheckResult Check(BoardData boardData)
        {
            if (boardData == null
                || boardData.TileAssignments == null
                || boardData.TileCount <= 0
                || boardData.TileAssignments.Count != boardData.TileCount)
            {
                return OpeningMoveCheckResult.Failed(
                    0,
                    0,
                    0,
                    "Board data is empty or incomplete.");
            }

            if (boardData.TileCount % 2 != 0)
            {
                return OpeningMoveCheckResult.Failed(
                    0,
                    0,
                    0,
                    "Board tile count is not pairable.");
            }

            List<TileSymbolAssignment> selectableAssignments = BoardDataLayoutQuery.GetSelectableAssignments(boardData);
            int selectableCount = selectableAssignments.Count;
            int accessiblePairCount = BoardDataLayoutQuery.CountAccessiblePairs(selectableAssignments);
            int meaningfulOpeningChoiceCount = BoardDataLayoutQuery.CountMeaningfulOpeningChoices(selectableAssignments);

            if (meaningfulOpeningChoiceCount < MinimumMeaningfulOpeningChoices)
            {
                return OpeningMoveCheckResult.Failed(
                    selectableCount,
                    accessiblePairCount,
                    meaningfulOpeningChoiceCount,
                    "Board provides fewer than "
                    + MinimumMeaningfulOpeningChoices
                    + " meaningful opening choices.");
            }

            return OpeningMoveCheckResult.Passed(
                selectableCount,
                accessiblePairCount,
                meaningfulOpeningChoiceCount);
        }
    }
}
