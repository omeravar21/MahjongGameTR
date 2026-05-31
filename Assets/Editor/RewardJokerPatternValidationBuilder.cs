#if UNITY_EDITOR
using System.Text;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class RewardJokerPatternValidationBuilder
    {
        [MenuItem("MahjongGame/Validate Reward Joker Pattern System")]
        public static void ValidateRewardJokerPatternSystem()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = RewardJokerPatternSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[RewardJokerPatternValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[RewardJokerPatternValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
