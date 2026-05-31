#if UNITY_EDITOR
using System.Text;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class LevelRecipeValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Level Recipe System")]
        public static void ValidateLevelRecipeSystem()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = LevelRecipeSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[LevelRecipeValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[LevelRecipeValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
