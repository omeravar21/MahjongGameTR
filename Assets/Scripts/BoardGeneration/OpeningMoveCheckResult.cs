namespace MahjongGame.BoardGeneration
{
    public readonly struct OpeningMoveCheckResult
    {
        public bool IsValid { get; }

        public int SelectableCount { get; }

        public int AccessiblePairCount { get; }

        public int MeaningfulOpeningChoiceCount { get; }

        public string FailureReason { get; }

        public OpeningMoveCheckResult(
            bool isValid,
            int selectableCount,
            int accessiblePairCount,
            int meaningfulOpeningChoiceCount,
            string failureReason)
        {
            IsValid = isValid;
            SelectableCount = selectableCount;
            AccessiblePairCount = accessiblePairCount;
            MeaningfulOpeningChoiceCount = meaningfulOpeningChoiceCount;
            FailureReason = failureReason ?? string.Empty;
        }

        public static OpeningMoveCheckResult Passed(
            int selectableCount,
            int accessiblePairCount,
            int meaningfulOpeningChoiceCount)
        {
            return new OpeningMoveCheckResult(
                true,
                selectableCount,
                accessiblePairCount,
                meaningfulOpeningChoiceCount,
                string.Empty);
        }

        public static OpeningMoveCheckResult Failed(
            int selectableCount,
            int accessiblePairCount,
            int meaningfulOpeningChoiceCount,
            string failureReason)
        {
            return new OpeningMoveCheckResult(
                false,
                selectableCount,
                accessiblePairCount,
                meaningfulOpeningChoiceCount,
                failureReason);
        }
    }
}
