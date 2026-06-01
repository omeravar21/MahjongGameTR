#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    public static class LevelCompletionFlowBuilder
    {
        [MenuItem("MahjongGame/Build Level Completion Flow")]
        public static void BuildLevelCompletionFlow()
        {
            PerformanceScreenBuilder.BuildPerformanceScreen();
            Debug.Log("[LevelCompletionFlowBuilder] Level completion flow wired on GameScene.");
        }
    }
}
#endif
