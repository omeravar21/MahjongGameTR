#if UNITY_EDITOR
using System.Text;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class DifficultyValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Difficulty System")]
        public static void ValidateDifficultySystem()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = DifficultySystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[DifficultyValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[DifficultyValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
