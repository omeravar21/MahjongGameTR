using MahjongGame.Core;
using MahjongGame.Core.Save;
using MahjongGame.Score;
using UnityEngine;

namespace MahjongGame.Ranking
{
    public sealed class RankingDirector : MonoBehaviour
    {
        private const string LocalPlayerId = "local_player";

        private static RankingDirector _instance;

        private RankingData _rankingData = RankingData.CreateDefault();
        private LeaderboardData _cachedLeaderboardData = LeaderboardData.Empty;

        public static RankingDirector Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[RankingDirector] Instance is not available.");
                }

                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        public RankingData RankingState => _rankingData;

        public long GlobalPerformanceScore => _rankingData.GlobalPerformanceScore;

        public int CurrentGlobalRank => _rankingData.CurrentGlobalRank;

        public int HighestGlobalRank => _rankingData.HighestGlobalRank;

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

        private void Start()
        {
            LoadFromSave();
            RefreshGlobalRank();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public void LoadFromSave()
        {
            if (!SaveSystem.HasInstance)
            {
                Debug.LogWarning("[RankingDirector] SaveSystem is not ready.");
                return;
            }

            _rankingData = RankingData.FromSave(SaveSystem.Instance.Data);
        }

        public RankingEntry GetLocalRankingEntry()
        {
            return new RankingEntry(
                LocalPlayerId,
                GlobalLeaderboardDefinition.LocalPlayerDisplayName,
                GlobalPerformanceScore,
                CurrentGlobalRank,
                true);
        }

        public LeaderboardData GetLeaderboardData()
        {
            return _cachedLeaderboardData;
        }

        public void RefreshGlobalRank()
        {
            _cachedLeaderboardData = GlobalLeaderboardBuilder.Build(
                GlobalPerformanceScore,
                GlobalLeaderboardDefinition.LocalPlayerDisplayName);

            int previousRank = CurrentGlobalRank;
            int newRank = _cachedLeaderboardData.LocalPlayerRank;
            _rankingData.SetCurrentGlobalRank(newRank);
            PersistRankingState();

            if (previousRank != newRank)
            {
                RankingEvents.RaiseGlobalRankChanged(new GlobalRankChangedContext(previousRank, newRank));
            }
        }

        public bool TryAccumulateLevelPerformance(
            LevelPerformanceResult result,
            out long previousScore,
            out long newScore)
        {
            previousScore = GlobalPerformanceScore;
            newScore = previousScore;

            if (result == null || result.TotalPerformanceScore <= 0)
            {
                return false;
            }

            newScore = previousScore + result.TotalPerformanceScore;
            _rankingData.SetGlobalPerformanceScore(newScore);
            PersistRankingState();

            RankingEvents.RaiseGlobalPerformanceScoreChanged(
                new GlobalPerformanceScoreChangedContext(previousScore, newScore));

            RefreshGlobalRank();
            return true;
        }

        internal void SetRankingStateForValidation(long globalPerformanceScore, int currentGlobalRank)
        {
            _rankingData.SetGlobalPerformanceScore(globalPerformanceScore);
            _rankingData.SetCurrentGlobalRank(currentGlobalRank);
            _cachedLeaderboardData = GlobalLeaderboardBuilder.Build(
                globalPerformanceScore,
                GlobalLeaderboardDefinition.LocalPlayerDisplayName);
        }

        private void PersistRankingState()
        {
            if (!SaveSystem.HasInstance)
            {
                return;
            }

            _rankingData.WriteToSave(SaveSystem.Instance.Data);
            SaveSystem.Instance.Save();
        }
    }
}
