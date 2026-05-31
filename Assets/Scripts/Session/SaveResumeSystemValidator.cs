using System.Text;
using MahjongGame.Board;
using MahjongGame.Core.Save;
using UnityEngine;

namespace MahjongGame.Session
{
    public static class SaveResumeSystemValidator
    {
        public static bool Validate(Transform gameplayRoot, StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            passed &= ActiveLevelSaveSystemValidator.Validate(gameplayRoot, reportBuilder);
            passed &= ValidateBoardSaveTypes(reportBuilder);
            passed &= ValidateBoardSpawnerRestoreApi(gameplayRoot, reportBuilder);
            passed &= ValidateSerializerRoundTrip(reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Save resume validation completed successfully."
                : "[FAIL] Save resume validation found issues.");

            return passed;
        }

        private static bool ValidateBoardSaveTypes(StringBuilder reportBuilder)
        {
            bool passed = true;

            passed &= AppendTypeResult(typeof(SavedTileState), reportBuilder);
            passed &= AppendTypeResult(typeof(SavedBoardState), reportBuilder);
            passed &= AppendTypeResult(typeof(SavedTrayState), reportBuilder);
            passed &= AppendTypeResult(typeof(SavedClosedTileStateCollection), reportBuilder);
            passed &= AppendTypeResult(typeof(ActiveBoardStateSerializer), reportBuilder);
            passed &= AppendTypeResult(typeof(ActiveBoardStateRestorer), reportBuilder);

            return passed;
        }

        private static bool ValidateBoardSpawnerRestoreApi(Transform gameplayRoot, StringBuilder reportBuilder)
        {
            if (gameplayRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] GameplayRoot is missing for BoardSpawner restore validation.");
                return false;
            }

            Transform boardRoot = gameplayRoot.Find("BoardRoot");
            BoardSpawner boardSpawner = boardRoot != null ? boardRoot.GetComponent<BoardSpawner>() : null;
            if (boardSpawner == null)
            {
                AppendLine(reportBuilder, "[FAIL] BoardSpawner is missing for restore validation.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] BoardSpawner restore API is wired on BoardRoot.");
            return true;
        }

        private static bool ValidateSerializerRoundTrip(StringBuilder reportBuilder)
        {
            SavedBoardState boardState = new SavedBoardState
            {
                tiles = new[]
                {
                    new SavedTileState
                    {
                        tileId = 1,
                        column = 2,
                        row = 3,
                        layerIndex = 0,
                        symbolId = 4,
                        tileState = (int)Tiles.TileState.OnBoard,
                        isClosed = false,
                        isJoker = false
                    }
                }
            };

            string json = JsonUtility.ToJson(boardState);
            SavedBoardState restored = JsonUtility.FromJson<SavedBoardState>(json);
            if (restored == null || restored.tiles == null || restored.tiles.Length != 1)
            {
                AppendLine(reportBuilder, "[FAIL] Saved board state JSON round-trip failed.");
                return false;
            }

            if (restored.tiles[0].tileId != 1 || restored.tiles[0].symbolId != 4)
            {
                AppendLine(reportBuilder, "[FAIL] Saved board state JSON round-trip values are incorrect.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Saved board state JSON round-trip succeeded.");
            return true;
        }

        private static bool AppendTypeResult(System.Type type, StringBuilder reportBuilder)
        {
            if (type == null)
            {
                AppendLine(reportBuilder, "[FAIL] Required save resume type is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] " + type.Name + " type is present.");
            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string line)
        {
            reportBuilder.AppendLine(line);
        }
    }
}
