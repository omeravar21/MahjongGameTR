using MahjongGame.BoardGeneration;
using MahjongGame.Progression;
using UnityEngine;

namespace MahjongGame.Core
{
    public sealed class GameBootstrapper : MonoBehaviour
    {
        private static bool _isInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void ResetInitializationFlag()
        {
            _isInitialized = false;
        }

        private void Awake()
        {
            if (_isInitialized)
            {
                Destroy(gameObject);
                return;
            }

            _isInitialized = true;
            DontDestroyOnLoad(gameObject);

            if (GetComponent<SceneLoadController>() == null)
            {
                gameObject.AddComponent<SceneLoadController>();
            }

            if (GetComponent<SaveSystem>() == null)
            {
                gameObject.AddComponent<SaveSystem>();
            }

            if (GetComponent<PlayerProgressionDirector>() == null)
            {
                gameObject.AddComponent<PlayerProgressionDirector>();
            }

            if (GetComponent<DifficultyDirector>() == null)
            {
                gameObject.AddComponent<DifficultyDirector>();
            }

            if (GetComponent<VisualVarietyDirector>() == null)
            {
                gameObject.AddComponent<VisualVarietyDirector>();
            }
        }

        private void Start()
        {
            if (GameState.Current == AppGameState.None)
            {
                GameState.SetState(AppGameState.Booting);
            }

            if (SceneLoadController.HasInstance)
            {
                SceneLoadController.Instance.LoadMainMenu();
            }
        }
    }
}