using System.Collections.Generic;
using System.Text;
using MahjongGame.Board;
using MahjongGame.Tiles;

namespace MahjongGame.BoardGeneration
{
    public static class OpeningMoveCheckerSystemValidator
    {
        public static bool Validate(StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateSyntheticOpeningBoard(reportBuilder);
            passed &= ValidateEmptyBoardRejection(reportBuilder);
            passed &= ValidateLaunchBoardSamples(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] OpeningMoveChecker system validation completed successfully."
                : "[FAIL] OpeningMoveChecker system validation found issues.");

            return passed;
        }

        private static bool ValidateSyntheticOpeningBoard(StringBuilder reportBuilder)
        {
            TileSymbolAssignment[] assignments =
            {
                new TileSymbolAssignment(0, new TileBoardPosition(new BoardGridCoordinate(0, 0), 0), 10),
                new TileSymbolAssignment(1, new TileBoardPosition(new BoardGridCoordinate(1, 0), 0), 10),
                new TileSymbolAssignment(2, new TileBoardPosition(new BoardGridCoordinate(3, 0), 0), 20),
                new TileSymbolAssignment(3, new TileBoardPosition(new BoardGridCoordinate(4, 0), 0), 20),
            };

            BoardData boardData = new BoardData(
                1,
                0,
                BoardArchetypeId.Diamond,
                0,
                HolePatternId.SingleCenter,
                1,
                assignments.Length,
                0,
                0,
                false,
                assignments);

            OpeningMoveCheckResult result = OpeningMoveChecker.Check(boardData);
            if (!result.IsValid || result.MeaningfulOpeningChoiceCount < OpeningMoveChecker.MinimumMeaningfulOpeningChoices)
            {
                AppendLine(reportBuilder, "[FAIL] Synthetic opening board failed OpeningMoveChecker.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Synthetic opening board passes OpeningMoveChecker.");
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

            OpeningMoveCheckResult result = OpeningMoveChecker.Check(emptyBoard);
            if (result.IsValid)
            {
                AppendLine(reportBuilder, "[FAIL] Empty board was accepted by OpeningMoveChecker.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Empty board is rejected by OpeningMoveChecker.");
            return true;
        }

        private static bool ValidateLaunchBoardSamples(StringBuilder reportBuilder)
        {
            bool passed = true;

            for (int levelNumber = 1; levelNumber <= 10; levelNumber++)
            {
                BoardData boardData = BoardGenerationPipeline.GenerateBoardData(levelNumber);
                OpeningMoveCheckResult result = OpeningMoveChecker.Check(boardData);

                if (result.SelectableCount < 0 || result.MeaningfulOpeningChoiceCount < 0)
                {
                    AppendLine(
                        reportBuilder,
                        "[FAIL] OpeningMoveChecker returned invalid metrics for level "
                        + levelNumber
                        + ".");
                    passed = false;
                    continue;
                }

                AppendLine(
                    reportBuilder,
                    "[PASS] OpeningMoveChecker evaluated level "
                    + levelNumber
                    + " board (valid="
                    + result.IsValid
                    + ", choices="
                    + result.MeaningfulOpeningChoiceCount
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
