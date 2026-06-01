using MahjongGame.BoardGeneration;
using MahjongGame.DailyBoard;
using MahjongGame.DailyMissions;
using MahjongGame.DailyRewards;
using MahjongGame.Progression;
using MahjongGame.Ranking;
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

            if (GetComponent<RankingDirector>() == null)
            {
                gameObject.AddComponent<RankingDirector>();
            }

            if (GetComponent<RankingSyncController>() == null)
            {
                gameObject.AddComponent<RankingSyncController>();
            }

            if (GetComponent<DailyBoardDirector>() == null)
            {
                gameObject.AddComponent<DailyBoardDirector>();
            }

            if (GetComponent<DailyMissionDirector>() == null)
            {
                gameObject.AddComponent<DailyMissionDirector>();
            }

            if (GetComponent<DailyMissionProgressTracker>() == null)
            {
                gameObject.AddComponent<DailyMissionProgressTracker>();
            }

            if (GetComponent<DailyRewardDirector>() == null)
            {
                gameObject.AddComponent<DailyRewardDirector>();
            }

            if (GetComponent<DailyMissionRewardController>() == null)
            {
                gameObject.AddComponent<DailyMissionRewardController>();
            }

            if (GetComponent<DifficultyDirector>() == null)
            {
                gameObject.AddComponent<DifficultyDirector>();
            }

            if (GetComponent<VisualVarietyDirector>() == null)
            {
                gameObject.AddComponent<VisualVarietyDirector>();
            }

            if (GetComponent<LevelRecipeGenerator>() == null)
            {
                gameObject.AddComponent<LevelRecipeGenerator>();
            }

            if (GetComponent<GridMaskGenerator>() == null)
            {
                gameObject.AddComponent<GridMaskGenerator>();
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