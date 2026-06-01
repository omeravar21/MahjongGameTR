#if UNITY_EDITOR
using System.Text;
using MahjongGame.Progression;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class ProgressionValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Progression System")]
        public static void ValidateProgressionSystem()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = ProgressionSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[ProgressionValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[ProgressionValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
