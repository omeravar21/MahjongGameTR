#if UNITY_EDITOR
using MahjongGame.Boosters;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class BoosterControllerBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Booster Controller")]
        public static void BuildBoosterController()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[BoosterControllerBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            Transform boosterRoot = gameplayRoot.Find("BoosterRoot");
            if (boosterRoot == null)
            {
                Debug.LogError("[BoosterControllerBuilder] BoosterRoot was not found in GameScene.");
                return;
            }

            if (boosterRoot.GetComponent<ShuffleBooster>() == null)
            {
                boosterRoot.gameObject.AddComponent<ShuffleBooster>();
            }

            BoosterController boosterController = boosterRoot.GetComponent<BoosterController>();
            if (boosterController == null)
            {
                boosterController = boosterRoot.gameObject.AddComponent<BoosterController>();
            }

            BoosterEconomyDirector economyDirector = gameplayRoot.GetComponent<BoosterEconomyDirector>();
            ShuffleBooster shuffleBooster = boosterRoot.GetComponent<ShuffleBooster>();

            SerializedObject serializedController = new SerializedObject(boosterController);
            SerializedProperty economyDirectorProperty = serializedController.FindProperty("economyDirector");
            if (economyDirectorProperty != null)
            {
                economyDirectorProperty.objectReferenceValue = economyDirector;
            }

            SerializedProperty shuffleBoosterProperty = serializedController.FindProperty("shuffleBooster");
            if (shuffleBoosterProperty != null)
            {
                shuffleBoosterProperty.objectReferenceValue = shuffleBooster;
            }

            serializedController.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[BoosterControllerBuilder] BoosterController and ShuffleBooster wired on BoosterRoot.");
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
