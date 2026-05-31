namespace MahjongGame.BoardGeneration
{
    public readonly struct DeadlockRiskCheckResult
    {
        public bool IsValid { get; }

        public int SelectableCount { get; }

        public int AccessiblePairCount { get; }

        public float BlockedTileRatio { get; }

        public int RiskScore { get; }

        public string FailureReason { get; }

        public DeadlockRiskCheckResult(
            bool isValid,
            int selectableCount,
            int accessiblePairCount,
            float blockedTileRatio,
            int riskScore,
            string failureReason)
        {
            IsValid = isValid;
            SelectableCount = selectableCount;
            AccessiblePairCount = accessiblePairCount;
            BlockedTileRatio = blockedTileRatio;
            RiskScore = riskScore;
            FailureReason = failureReason ?? string.Empty;
        }

        public static DeadlockRiskCheckResult Passed(
            int selectableCount,
            int accessiblePairCount,
            float blockedTileRatio,
            int riskScore)
        {
            return new DeadlockRiskCheckResult(
                true,
                selectableCount,
                accessiblePairCount,
                blockedTileRatio,
                riskScore,
                string.Empty);
        }

        public static DeadlockRiskCheckResult Failed(
            int selectableCount,
            int accessiblePairCount,
            float blockedTileRatio,
            int riskScore,
            string failureReason)
        {
            return new DeadlockRiskCheckResult(
                false,
                selectableCount,
                accessiblePairCount,
                blockedTileRatio,
                riskScore,
                failureReason);
        }
    }
}
