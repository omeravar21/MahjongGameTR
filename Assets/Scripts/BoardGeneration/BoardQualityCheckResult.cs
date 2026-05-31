namespace MahjongGame.BoardGeneration
{
    public readonly struct BoardQualityCheckResult
    {
        public bool IsValid { get; }

        public bool GridIntegrityPassed { get; }

        public bool LayerIntegrityPassed { get; }

        public bool TilePairValidityPassed { get; }

        public bool OpeningMovePassed { get; }

        public bool SelectableCountPassed { get; }

        public bool DeadlockRiskPassed { get; }

        public bool ClosedTileFairnessPassed { get; }

        public bool JokerAccessibilityPassed { get; }

        public bool DensityPassed { get; }

        public string FailureReason { get; }

        public BoardQualityCheckResult(
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
            IsValid = isValid;
            GridIntegrityPassed = gridIntegrityPassed;
            LayerIntegrityPassed = layerIntegrityPassed;
            TilePairValidityPassed = tilePairValidityPassed;
            OpeningMovePassed = openingMovePassed;
            SelectableCountPassed = selectableCountPassed;
            DeadlockRiskPassed = deadlockRiskPassed;
            ClosedTileFairnessPassed = closedTileFairnessPassed;
            JokerAccessibilityPassed = jokerAccessibilityPassed;
            DensityPassed = densityPassed;
            FailureReason = failureReason ?? string.Empty;
        }
    }
}
