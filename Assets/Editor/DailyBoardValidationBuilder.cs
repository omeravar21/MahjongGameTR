#if UNITY_EDITOR
using System.Text;
using MahjongGame.DailyBoard;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class DailyBoardValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Daily Board Architecture")]
        public static void ValidateDailyBoardArchitecture()
        {
            RunArchitectureValidation(exitOnBatchMode: false);
        }

        [MenuItem("MahjongGame/Validate Daily Board Generation")]
        public static void ValidateDailyBoardGeneration()
        {
            RunGenerationValidation(exitOnBatchMode: false);
        }

        public static void ExecuteValidateArchitecture()
        {
            RunArchitectureValidation(exitOnBatchMode: true);
        }

        public static void ExecuteValidateGeneration()
        {
            RunGenerationValidation(exitOnBatchMode: true);
        }

        private static void RunArchitectureValidation(bool exitOnBatchMode)
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = DailyBoardSystemValidator.Validate(reportBuilder);
            LogValidationResult("DailyBoardValidationBuilder", reportBuilder, passed, exitOnBatchMode);
        }

        private static void RunGenerationValidation(bool exitOnBatchMode)
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = DailyBoardGenerationSystemValidator.Validate(reportBuilder);
            LogValidationResult("DailyBoardValidationBuilder", reportBuilder, passed, exitOnBatchMode);
        }

        private static void LogValidationResult(string prefix, StringBuilder reportBuilder, bool passed, bool exitOnBatchMode)
        {
            string report = reportBuilder.ToString().TrimEnd();

            if (passed)
            {
                Debug.Log("[" + prefix + "] " + report);
            }
            else
            {
                Debug.LogWarning("[" + prefix + "] " + report);
            }

            if (exitOnBatchMode && Application.isBatchMode)
            {
                EditorApplication.Exit(passed ? 0 : 1);
            }
        }
    }
}
#endif
