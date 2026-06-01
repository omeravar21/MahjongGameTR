#if UNITY_EDITOR
using System.Text;
using MahjongGame.DailyMissions;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class DailyMissionValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Daily Mission Architecture")]
        public static void ValidateDailyMissionArchitecture()
        {
            RunArchitectureValidation(exitOnBatchMode: false);
        }

        public static void ExecuteValidateArchitecture()
        {
            RunArchitectureValidation(exitOnBatchMode: true);
        }

        private static void RunArchitectureValidation(bool exitOnBatchMode)
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = DailyMissionSystemValidator.Validate(reportBuilder);
            LogValidationResult("DailyMissionValidationBuilder", reportBuilder, passed, exitOnBatchMode);
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
