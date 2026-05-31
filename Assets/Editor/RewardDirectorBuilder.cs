#if UNITY_EDITOR
using MahjongGame.Rewards;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class RewardDirectorBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Reward Joker System")]
        public static void BuildRewardJokerSystem()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[RewardDirectorBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            JokerTileController jokerTileController = gameplayRoot.GetComponent<JokerTileController>();
            if (jokerTileController == null)
            {
                jokerTileController = gameplayRoot.gameObject.AddComponent<JokerTileController>();
            }

            RewardDirector rewardDirector = gameplayRoot.GetComponent<RewardDirector>();
            if (rewardDirector == null)
            {
                rewardDirector = gameplayRoot.gameObject.AddComponent<RewardDirector>();
            }

            SerializedObject serializedDirector = new SerializedObject(rewardDirector);
            SerializedProperty jokerControllerProperty = serializedDirector.FindProperty("jokerTileController");
            if (jokerControllerProperty != null)
            {
                jokerControllerProperty.objectReferenceValue = jokerTileController;
                serializedDirector.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[RewardDirectorBuilder] RewardDirector and JokerTileController wired on GameplayRoot.");
        }

        private static Transform FindGameplayRoot(Scene scene)
        {
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                if (rootObject.name == "GameplayRoot")
                {
                    return rootObject.transform;
                }
            }

            return null;
        }
    }
}
#endif
