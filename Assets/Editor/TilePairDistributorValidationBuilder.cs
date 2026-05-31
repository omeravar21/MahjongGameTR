#if UNITY_EDITOR
using System.Text;
using MahjongGame.BoardGeneration;
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class TilePairDistributorValidationBuilder
    {
        [MenuItem("MahjongGame/Validate TilePairDistributor")]
        public static void ValidateTilePairDistributor()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = TilePairDistributorSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[TilePairDistributorValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[TilePairDistributorValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }
    }
}
#endif
