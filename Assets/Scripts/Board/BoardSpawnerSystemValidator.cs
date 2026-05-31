using System.Text;
using MahjongGame.BoardGeneration;
using UnityEngine;

namespace MahjongGame.Board
{
    public static class BoardSpawnerSystemValidator
    {
        public static bool Validate(Transform boardRoot, StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ValidateComponentWiring(boardRoot, reportBuilder);
            passed &= ValidateGeneratedBoardData(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] BoardSpawner system validation completed successfully."
                : "[FAIL] BoardSpawner system validation found issues.");

            return passed;
        }

        private static bool ValidateComponentWiring(Transform boardRoot, StringBuilder reportBuilder)
        {
            if (boardRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoardRoot is missing for BoardSpawner validation.");
                return false;
            }

            BoardSpawner boardSpawner = boardRoot.GetComponent<BoardSpawner>();
            if (boardSpawner == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoardSpawner is missing on BoardRoot.");
                return false;
            }

            if (!boardSpawner.HasTilePrefab)
            {
                AppendLine(reportBuilder, "[FAIL] BoardSpawner tile prefab is not configured.");
                return false;
            }

            if (boardRoot.GetComponent<BoardLayerVisualController>() == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoardLayerVisualController is missing on BoardRoot.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] BoardSpawner wiring is valid on BoardRoot.");
            return true;
        }

        private static bool ValidateGeneratedBoardData(StringBuilder reportBuilder)
        {
            BoardData boardData = BoardGenerationPipeline.GenerateBoardData(18);

            if (boardData.TileCount <= 0 || boardData.TileAssignments.Count != boardData.TileCount)
            {
                AppendLine(reportBuilder, "[FAIL] Generated board data is empty for spawn validation.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Generated board data is ready for runtime spawning.");
            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
