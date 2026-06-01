#if UNITY_EDITOR
using MahjongGame.Board;
using MahjongGame.Core;
using MahjongGame.Session;
using MahjongGame.Tiles;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Editor
{
    public static class GameSceneSpawnVerificationBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";
        private const string BootScenePath = "Assets/Scenes/BootScene.unity";

        private static bool _isRunning;
        private static int _playModeFramesWaited;
        private static string _verificationMode;

        [MenuItem("MahjongGame/Verify GameScene Runtime Tile Spawn")]
        public static void VerifyGameSceneRuntimeTileSpawn()
        {
            StartVerification("DirectGameScene");
        }

        public static void VerifyGameSceneRuntimeTileSpawnBatch()
        {
            StartVerification("DirectGameScene");
        }

        public static void VerifyBootSceneHasNoTileSpawnBatch()
        {
            StartVerification("BootSceneNoTiles");
        }

        private static void StartVerification(string mode)
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;
            _playModeFramesWaited = 0;
            _verificationMode = mode;
            EditorApplication.update -= HandlePlayModeVerificationUpdate;
            EditorApplication.update += HandlePlayModeVerificationUpdate;

            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            if (mode == "BootSceneNoTiles")
            {
                EditorSceneManager.OpenScene(BootScenePath, OpenSceneMode.Single);
            }
            else
            {
                EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            }

            EditorApplication.isPlaying = true;
        }

        private static void HandlePlayModeVerificationUpdate()
        {
            if (!_isRunning)
            {
                return;
            }

            if (!EditorApplication.isPlaying)
            {
                if (_playModeFramesWaited > 0)
                {
                    FinishVerification(false, "Play Mode exited before verification completed.");
                }

                return;
            }

            _playModeFramesWaited++;

            if (_verificationMode == "BootSceneNoTiles")
            {
                if (_playModeFramesWaited < 1)
                {
                    return;
                }

                bool bootPassed = VerifyBootSceneHasNoTiles();
                FinishVerification(bootPassed, bootPassed ? "Verification passed." : "Verification failed.");
                return;
            }

            if (_playModeFramesWaited < 3)
            {
                return;
            }

            bool passed = VerifyDirectGameSceneSpawn();

            FinishVerification(passed, passed ? "Verification passed." : "Verification failed.");
        }

        private static bool VerifyDirectGameSceneSpawn()
        {
            if (SceneManager.GetActiveScene().name != SceneLoadController.GameSceneName)
            {
                Debug.LogError(
                    "[GameSceneSpawnVerificationBuilder] Active scene is not GameScene: "
                    + SceneManager.GetActiveScene().name
                    + ".");
                return false;
            }

            if (!SessionDirector.HasInstance || !SessionDirector.Instance.IsSessionActive)
            {
                Debug.LogError("[GameSceneSpawnVerificationBuilder] SessionDirector session is not active.");
                return false;
            }

            Transform boardRoot = FindBoardRoot();
            if (boardRoot == null)
            {
                Debug.LogError("[GameSceneSpawnVerificationBuilder] BoardRoot was not found.");
                return false;
            }

            int tileCount = BoardTileOccupancyQuery.CollectOccupyingTiles(boardRoot).Count;
            if (tileCount <= 0)
            {
                Debug.LogError(
                    "[GameSceneSpawnVerificationBuilder] No runtime tiles found under BoardRoot.");
                return false;
            }

            Debug.Log(
                "[GameSceneSpawnVerificationBuilder] Direct GameScene play verified: tiles="
                + tileCount
                + ".");
            return true;
        }

        private static bool VerifyBootSceneHasNoTiles()
        {
            if (SceneManager.GetActiveScene().name != SceneLoadController.BootSceneName)
            {
                Debug.LogError(
                    "[GameSceneSpawnVerificationBuilder] Active scene is not BootScene: "
                    + SceneManager.GetActiveScene().name
                    + ".");
                return false;
            }

            Tile[] tiles = Object.FindObjectsByType<Tile>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (tiles.Length > 0)
            {
                Debug.LogError(
                    "[GameSceneSpawnVerificationBuilder] BootScene contains "
                    + tiles.Length
                    + " tile object(s); expected zero.");
                return false;
            }

            Debug.Log("[GameSceneSpawnVerificationBuilder] BootScene has no runtime tiles.");
            return true;
        }

        private static Transform FindBoardRoot()
        {
            GameObject gameplayRoot = GameObject.Find("GameplayRoot");
            return gameplayRoot != null ? gameplayRoot.transform.Find("BoardRoot") : null;
        }

        private static void FinishVerification(bool passed, string message)
        {
            EditorApplication.update -= HandlePlayModeVerificationUpdate;
            _isRunning = false;

            if (passed)
            {
                Debug.Log("[GameSceneSpawnVerificationBuilder] " + message);
            }
            else
            {
                Debug.LogError("[GameSceneSpawnVerificationBuilder] " + message);
            }

            EditorApplication.isPlaying = false;

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(passed ? 0 : 1);
            }
        }
    }
}
#endif
