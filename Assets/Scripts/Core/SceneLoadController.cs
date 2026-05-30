using UnityEngine;
using UnityEngine.SceneManagement;

namespace MahjongGame.Core
{
    public sealed class SceneLoadController : MonoBehaviour
    {
        public const string BootSceneName = "BootScene";
        public const string MainMenuSceneName = "MainMenuScene";
        public const string GameSceneName = "GameScene";

        private static SceneLoadController _instance;

        public static SceneLoadController Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[SceneLoadController] Instance is not available.");
                }

                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public void LoadMainMenu()
        {
            LoadScene(MainMenuSceneName);
        }

        public void LoadGame()
        {
            LoadScene(GameSceneName);
        }

        public void LoadBoot()
        {
            LoadScene(BootSceneName);
        }

        private void LoadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("[SceneLoadController] Scene name is empty.");
                return;
            }

            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError("[SceneLoadController] Scene '" + sceneName + "' is not in the build settings.");
                return;
            }

            GameState.SetState(AppGameState.Loading);
            GameEvents.RaiseSceneLoadStarted(sceneName);
            SceneManager.LoadScene(sceneName);
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameEvents.RaiseSceneLoadCompleted(scene.name);

            switch (scene.name)
            {
                case MainMenuSceneName:
                    GameState.SetState(AppGameState.MainMenu);
                    break;
                case GameSceneName:
                    GameState.SetState(AppGameState.Gameplay);
                    break;
                case BootSceneName:
                    GameState.SetState(AppGameState.Booting);
                    break;
            }
        }
    }
}