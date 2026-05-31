#if UNITY_EDITOR
using System.Text;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class ArchetypeValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Archetype System")]
        public static void ValidateArchetypeSystem()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = ArchetypeSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[ArchetypeValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[ArchetypeValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
