#if UNITY_EDITOR
using System.Text;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class DeadlockRiskValidationBuilder
    {
        [MenuItem("MahjongGame/Validate DeadlockRiskChecker")]
        public static void ValidateDeadlockRiskChecker()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = DeadlockRiskCheckerSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[DeadlockRiskValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[DeadlockRiskValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
