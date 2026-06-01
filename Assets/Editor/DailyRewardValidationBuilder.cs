#if UNITY_EDITOR
using System.Text;
using MahjongGame.DailyRewards;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class DailyRewardValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Daily Rewards")]
        public static void ValidateDailyRewards()
        {
            RunValidation(exitOnBatchMode: false);
        }

        public static void ExecuteValidateDailyRewards()
        {
            RunValidation(exitOnBatchMode: true);
        }

        private static void RunValidation(bool exitOnBatchMode)
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = DailyRewardSystemValidator.Validate(reportBuilder);
            LogValidationResult("DailyRewardValidationBuilder", reportBuilder, passed, exitOnBatchMode);
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
