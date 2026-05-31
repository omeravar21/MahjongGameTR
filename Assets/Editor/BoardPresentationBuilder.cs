#if UNITY_EDITOR
using MahjongGame.Board;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class BoardPresentationBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("MahjongGame/Build Board Presentation")]
        public static void BuildBoardPresentation()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            Transform boardRoot = FindBoardRoot(scene);
            if (boardRoot == null)
            {
                Debug.LogError("[BoardPresentationBuilder] BoardRoot was not found in GameScene.");
                return;
            }

            ApplyBoardPresentation(boardRoot, FindMainCamera(scene));
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[BoardPresentationBuilder] Board presentation and camera alignment complete.");
        }

        public static void ApplyBoardPresentation(Transform boardRoot, Camera mainCamera)
        {
            if (boardRoot == null)
            {
                return;
            }

            if (boardRoot.GetComponent<BoardPresentationController>() == null)
            {
                boardRoot.gameObject.AddComponent<BoardPresentationController>();
            }

            BoardPresentationController presentationController = boardRoot.GetComponent<BoardPresentationController>();
            presentationController.ApplyPresentation();
            BoardFrameVisualController.BuildFrameVisual(boardRoot);

            if (mainCamera != null)
            {
                if (mainCamera.GetComponent<GameplayCameraController>() == null)
                {
                    mainCamera.gameObject.AddComponent<GameplayCameraController>();
                }

                GameplayCameraController cameraController = mainCamera.GetComponent<GameplayCameraController>();
                cameraController.ApplyCameraPresentation(force: true);
            }
        }

        private static Transform FindBoardRoot(Scene scene)
        {
            Transform gameplayRoot = FindGameplayRoot(scene);
            return gameplayRoot != null ? gameplayRoot.Find("BoardRoot") : null;
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

        private static Camera FindMainCamera(Scene scene)
        {
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                if (rootObject.CompareTag("MainCamera"))
                {
                    Camera camera = rootObject.GetComponent<Camera>();
                    if (camera != null)
                    {
                        return camera;
                    }
                }
            }

            return Camera.main;
        }
    }
}
#endif
