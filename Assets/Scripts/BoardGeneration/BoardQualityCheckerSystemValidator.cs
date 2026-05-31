using System.Text;

namespace MahjongGame.BoardGeneration
{
    public static class BoardQualityCheckerSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateEmptyBoardRejection(reportBuilder);
            passed &= ValidateLaunchBoardSamples(reportBuilder);
            passed &= ValidateCheckerOrchestration(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] BoardQualityChecker system validation completed successfully."
                : "[FAIL] BoardQualityChecker system validation found issues.");

            return passed;
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

            BoardQualityCheckResult result = BoardQualityChecker.Check(emptyBoard);
            if (result.IsValid)
            {
                AppendLine(reportBuilder, "[FAIL] Empty board was accepted by BoardQualityChecker.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Empty board is rejected by BoardQualityChecker.");
            return true;
        }

        private static bool ValidateLaunchBoardSamples(StringBuilder reportBuilder)
        {
            bool passed = true;

            for (int levelNumber = 1; levelNumber <= 10; levelNumber++)
            {
                BoardData boardData = BoardGenerationPipeline.GenerateBoardData(levelNumber);
                BoardQualityCheckResult result = BoardQualityChecker.Check(boardData);

                if (!result.GridIntegrityPassed || !result.LayerIntegrityPassed || !result.TilePairValidityPassed)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] Structural checks failed for level "
                        + levelNumber
                        + " board.");
                    passed = false;
                    continue;
                }

                AppendLine(
                    reportBuilder,
                    "[PASS] BoardQualityChecker evaluated level "
                    + levelNumber
                    + " board (valid="
                    + result.IsValid
                    + ").");
            }

            return passed;
        }

        private static bool ValidateCheckerOrchestration(StringBuilder reportBuilder)
        {
            BoardData boardData = BoardGenerationPipeline.GenerateBoardData(5);
            BoardQualityCheckResult result = BoardQualityChecker.Check(boardData);

            if (!result.GridIntegrityPassed || !result.LayerIntegrityPassed || !result.TilePairValidityPassed)
            {
                AppendLine(reportBuilder, "[FAIL] BoardQualityChecker structural orchestration failed.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] BoardQualityChecker orchestrates structural and risk sub-checks.");
            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
