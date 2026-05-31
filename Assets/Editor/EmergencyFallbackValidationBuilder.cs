#if UNITY_EDITOR
using System.Text;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class EmergencyFallbackValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Emergency Fallback")]
        public static void ValidateEmergencyFallback()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = EmergencyFallbackSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[EmergencyFallbackValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[EmergencyFallbackValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
