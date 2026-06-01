using MahjongGame.Core;
using MahjongGame.Core.Save;
using UnityEngine;

namespace MahjongGame.Ranking
{
    public sealed class RankingDirector : MonoBehaviour
    {
        private const string LocalPlayerId = "local_player";

        private static RankingDirector _instance;

        private RankingData _rankingData = RankingData.CreateDefault();

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
                "Player",
                GlobalPerformanceScore,
                CurrentGlobalRank,
                true);
        }

        public LeaderboardData GetLeaderboardData()
        {
            return LeaderboardData.Empty;
        }

        internal void SetRankingStateForValidation(long globalPerformanceScore, int currentGlobalRank)
        {
            _rankingData.SetGlobalPerformanceScore(globalPerformanceScore);
            _rankingData.SetCurrentGlobalRank(currentGlobalRank);
        }
    }
}
