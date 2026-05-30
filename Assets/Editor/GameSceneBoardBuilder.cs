#if UNITY_EDITOR
using MahjongGame.Board;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class GameSceneBoardBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Game Scene Board Hierarchy")]
        public static void BuildGameSceneBoardHierarchy()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);

            Transform gameplayRoot = FindGameplayRoot(scene);
            if (gameplayRoot == null)
            {
                Debug.LogError("[GameSceneBoardBuilder] GameplayRoot was not found in GameScene.");
                return;
            }

            Transform boardRoot = gameplayRoot.Find("BoardRoot");
            if (boardRoot == null)
            {
                Debug.LogError("[GameSceneBoardBuilder] BoardRoot was not found under GameplayRoot.");
                return;
            }

            BoardRootController.BuildBoardContainers(boardRoot);

            if (boardRoot.GetComponent<BoardRootController>() == null)
            {
                boardRoot.gameObject.AddComponent<BoardRootController>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[GameSceneBoardBuilder] Game scene board hierarchy complete.");
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
