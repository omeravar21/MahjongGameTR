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
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = DailyBoardSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[DailyBoardValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[DailyBoardValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
