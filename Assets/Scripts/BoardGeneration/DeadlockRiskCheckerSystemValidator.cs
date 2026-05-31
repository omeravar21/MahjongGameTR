using System.Text;

namespace MahjongGame.BoardGeneration
{
    public static class DeadlockRiskCheckerSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateSyntheticLowRiskBoard(reportBuilder);
            passed &= ValidateEmptyBoardRejection(reportBuilder);
            passed &= ValidateLaunchBoardSamples(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] DeadlockRiskChecker system validation completed successfully."
                : "[FAIL] DeadlockRiskChecker system validation found issues.");

            return passed;
        }

        private static bool ValidateSyntheticLowRiskBoard(StringBuilder reportBuilder)
        {
            BoardData boardData = BoardGenerationPipeline.GenerateBoardData(1);
            DeadlockRiskCheckResult result = DeadlockRiskChecker.Check(boardData);

            if (result.SelectableCount < 0 || result.BlockedTileRatio < 0f || result.BlockedTileRatio > 1f)
            {
                AppendLine(reportBuilder, "[FAIL] DeadlockRiskChecker returned invalid metrics.");
                return false;
            }

            AppendLine(
                reportBuilder,
                "[PASS] DeadlockRiskChecker evaluated launch board metrics (valid="
                + result.IsValid
                + ", selectable="
                + result.SelectableCount
                + ", risk="
                + result.RiskScore
                + ").");
            return true;
        }

        private static bool ValidateEmptyBoardRejection(StringBuilder reportBuilder)
        {
            BoardData emptyBoard = new BoardData(
                1,
                0,
                BoardArchetypeId.Diamond,
                0,
                HolePatternId.SingleCenter,
                1,
                0,
                0,
                0,
                false,
                new TileSymbolAssignment[0]);

            DeadlockRiskCheckResult result = DeadlockRiskChecker.Check(emptyBoard);
            if (result.IsValid)
            {
                AppendLine(reportBuilder, "[FAIL] Empty board was accepted by DeadlockRiskChecker.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Empty board is rejected by DeadlockRiskChecker.");
            return true;
        }

        private static bool ValidateLaunchBoardSamples(StringBuilder reportBuilder)
        {
            bool passed = true;

            for (int levelNumber = 1; levelNumber <= 10; levelNumber++)
            {
                BoardData boardData = BoardGenerationPipeline.GenerateBoardData(levelNumber);
                DeadlockRiskCheckResult result = DeadlockRiskChecker.Check(boardData);

                if (result.RiskScore < 0)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] DeadlockRiskChecker returned invalid risk score for level "
                        + levelNumber
                        + ".");
                    passed = false;
                    continue;
                }

                AppendLine(
                    reportBuilder,
                    "[PASS] DeadlockRiskChecker evaluated level "
                    + levelNumber
                    + " board (valid="
                    + result.IsValid
                    + ", blockedRatio="
                    + result.BlockedTileRatio.ToString("0.00")
                    + ").");
            }

            return passed;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
