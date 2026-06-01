using MahjongGame.Core;
using MahjongGame.Core.Save;
using UnityEngine;

namespace MahjongGame.Ranking
{
    public sealed class RankingSyncController : MonoBehaviour
    {
        private static RankingSyncController _instance;

        private readonly IRankingSyncProvider _syncProvider = new LocalCachedRankingSyncProvider();
        private RankingSyncEntrySaveData[] _activeRemoteEntries = System.Array.Empty<RankingSyncEntrySaveData>();

        public static RankingSyncController Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[RankingSyncController] Instance is not available.");
                }

                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        public bool HasCachedRemoteSnapshot => _activeRemoteEntries != null && _activeRemoteEntries.Length > 0;

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

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public bool RefreshSyncData(long localGlobalPerformanceScore)
        {
            if (!SaveSystem.HasInstance)
            {
                return false;
            }

            PlayerSaveData saveData = SaveSystem.Instance.Data;
            if (saveData == null)
            {
                return false;
            }

            saveData.EnsureDefaults();
            saveData.rankingSync ??= new RankingSyncSaveData();
            saveData.rankingSync.EnsureDefaults();

            bool uploaded = _syncProvider.TryUploadScore(localGlobalPerformanceScore, saveData.rankingSync);
            bool downloaded = _syncProvider.TryDownloadSnapshot(saveData.rankingSync, out RankingSyncEntrySaveData[] remoteEntries);

            _activeRemoteEntries = remoteEntries ?? System.Array.Empty<RankingSyncEntrySaveData>();
            SaveSystem.Instance.Save();

            if (uploaded || downloaded)
            {
                RankingEvents.RaiseRankingSyncCompleted(new RankingSyncCompletedContext(
                    saveData.rankingSync.lastUploadedScore,
                    _activeRemoteEntries.Length,
                    _activeRemoteEntries.Length > 0));
            }

            return uploaded && downloaded;
        }

        internal bool RefreshSyncDataForValidation(RankingSyncSaveData syncSaveData, long localGlobalPerformanceScore)
        {
            if (syncSaveData == null)
            {
                return false;
            }

            syncSaveData.EnsureDefaults();

            bool uploaded = _syncProvider.TryUploadScore(localGlobalPerformanceScore, syncSaveData);
            bool downloaded = _syncProvider.TryDownloadSnapshot(syncSaveData, out RankingSyncEntrySaveData[] remoteEntries);
            _activeRemoteEntries = remoteEntries ?? System.Array.Empty<RankingSyncEntrySaveData>();
            return uploaded && downloaded;
        }

        public RankingSyncEntrySaveData[] GetActiveRemoteEntries()
        {
            return _activeRemoteEntries ?? System.Array.Empty<RankingSyncEntrySaveData>();
        }

        internal void SetActiveRemoteEntriesForValidation(RankingSyncEntrySaveData[] remoteEntries)
        {
            _activeRemoteEntries = remoteEntries ?? System.Array.Empty<RankingSyncEntrySaveData>();
        }
    }
}
