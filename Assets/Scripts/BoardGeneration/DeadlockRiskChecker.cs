using System.Collections.Generic;

namespace MahjongGame.BoardGeneration
{
    public static class DeadlockRiskChecker
    {
        public const int MinimumSelectableTiles = 8;
        public const int MinimumAccessiblePairs = 2;
        public const float MaximumBlockedTileRatio = 0.70f;

        public static DeadlockRiskCheckResult Check(BoardData boardData)
        {
            if (boardData == null
                || boardData.TileAssignments == null
                || boardData.TileCount <= 0)
            {
                return DeadlockRiskCheckResult.Failed(
                    0,
                    0,
                    1f,
                    100,
                    "Board data is empty or incomplete.");
            }

            List<TileSymbolAssignment> selectableAssignments = BoardDataLayoutQuery.GetSelectableAssignments(boardData);
            int selectableCount = selectableAssignments.Count;
            int accessiblePairCount = BoardDataLayoutQuery.CountAccessiblePairs(selectableAssignments);
            float blockedTileRatio = ComputeBlockedTileRatio(boardData.TileCount, selectableCount);
            int riskScore = ComputeRiskScore(selectableCount, accessiblePairCount, blockedTileRatio);

            if (selectableCount < MinimumSelectableTiles)
            {
                return DeadlockRiskCheckResult.Failed(
                    selectableCount,
                    accessiblePairCount,
                    blockedTileRatio,
                    riskScore,
                    "Selectable tile count is below the launch minimum.");
            }

            if (accessiblePairCount < MinimumAccessiblePairs)
            {
                return DeadlockRiskCheckResult.Failed(
                    selectableCount,
                    accessiblePairCount,
                    blockedTileRatio,
                    riskScore,
                    "Accessible pair count is below the launch minimum.");
            }

            if (blockedTileRatio > MaximumBlockedTileRatio)
            {
                return DeadlockRiskCheckResult.Failed(
                    selectableCount,
                    accessiblePairCount,
                    blockedTileRatio,
                    riskScore,
                    "Blocked tile ratio exceeds the launch maximum.");
            }

            if (HasEarlyLayerChoke(boardData))
            {
                return DeadlockRiskCheckResult.Failed(
                    selectableCount,
                    accessiblePairCount,
                    blockedTileRatio,
                    riskScore,
                    "Board has early layer choke pressure.");
            }

            return DeadlockRiskCheckResult.Passed(
                selectableCount,
                accessiblePairCount,
                blockedTileRatio,
                riskScore);
        }

        private static float ComputeBlockedTileRatio(int totalTileCount, int selectableCount)
        {
            if (totalTileCount <= 0)
            {
                return 1f;
            }

            int blockedCount = totalTileCount - selectableCount;
            return blockedCount / (float)totalTileCount;
        }

        private static int ComputeRiskScore(
            int selectableCount,
            int accessiblePairCount,
            float blockedTileRatio)
        {
            int score = 0;

            if (selectableCount < MinimumSelectableTiles)
            {
                score += (MinimumSelectableTiles - selectableCount) * 8;
            }

            if (accessiblePairCount < MinimumAccessiblePairs)
            {
                score += (MinimumAccessiblePairs - accessiblePairCount) * 12;
            }

            if (blockedTileRatio > MaximumBlockedTileRatio)
            {
                score += (int)((blockedTileRatio - MaximumBlockedTileRatio) * 100f);
            }

            return score;
        }

        private static bool HasEarlyLayerChoke(BoardData boardData)
        {
            if (boardData == null || boardData.LayerDepth <= 1)
            {
                return false;
            }

            int topLayerIndex = boardData.LayerDepth - 1;
            int topLayerSelectableCount = BoardDataLayoutQuery.CountSelectableOnLayer(boardData, topLayerIndex);
            if (topLayerSelectableCount <= 0)
            {
                return false;
            }

            for (int layerIndex = 1; layerIndex < topLayerIndex; layerIndex++)
            {
                if (BoardDataLayoutQuery.CountSelectableOnLayer(boardData, layerIndex) == 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
