using System.Collections;
using System.Collections.Generic;
using MahjongGame.Tiles;
using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Matching
{
    public sealed class MatchController : MonoBehaviour
    {
        [SerializeField] private TrayController trayController;

        private readonly MatchQueue _matchQueue = new MatchQueue();
        private Coroutine _processingCoroutine;
        private MatchRequest _activeMatchRequest;

        private void Awake()
        {
            ResolveTrayController();
        }

        private void OnEnable()
        {
            TrayEvents.TrayTileStored += HandleTrayTileStored;
        }

        private void OnDisable()
        {
            TrayEvents.TrayTileStored -= HandleTrayTileStored;
            StopQueueProcessing();
        }

        public bool TryDetectMatchInTray(out MatchRequest matchRequest)
        {
            matchRequest = null;
            TrayController resolvedTrayController = ResolveTrayController();
            if (resolvedTrayController == null)
            {
                return false;
            }

            IReadOnlyList<Tile> trayTiles = resolvedTrayController.GetTrayTilesInSlotOrder();
            for (int firstSlotIndex = 0; firstSlotIndex < TrayRootDefinition.SlotCount; firstSlotIndex++)
            {
                Tile firstTile = trayTiles[firstSlotIndex];
                if (firstTile == null)
                {
                    continue;
                }

                for (int secondSlotIndex = firstSlotIndex + 1; secondSlotIndex < TrayRootDefinition.SlotCount; secondSlotIndex++)
                {
                    Tile secondTile = trayTiles[secondSlotIndex];
                    if (secondTile == null)
                    {
                        continue;
                    }

                    if (!TileMatchComparer.AreMatching(firstTile, secondTile))
                    {
                        continue;
                    }

                    matchRequest = new MatchRequest(
                        firstTile,
                        secondTile,
                        firstSlotIndex,
                        secondSlotIndex);
                    return true;
                }
            }

            return false;
        }

        private void HandleTrayTileStored(TrayTileStoredContext context)
        {
            if (context == null)
            {
                return;
            }

            if (!TryDetectMatchInTray(out MatchRequest matchRequest))
            {
                return;
            }

            if (IsPairPending(matchRequest.FirstTile, matchRequest.SecondTile))
            {
                return;
            }

            if (!_matchQueue.Enqueue(matchRequest))
            {
                return;
            }

            MatchEvents.RaiseMatchDetected(matchRequest);
            TryStartQueueProcessing();
        }

        private bool IsPairPending(Tile firstTile, Tile secondTile)
        {
            return _matchQueue.ContainsTiles(firstTile, secondTile)
                || MatchQueue.ReferencesSamePair(_activeMatchRequest, firstTile, secondTile);
        }

        private void TryStartQueueProcessing()
        {
            if (_processingCoroutine != null || _matchQueue.Count == 0)
            {
                return;
            }

            _processingCoroutine = StartCoroutine(ProcessMatchQueueCoroutine());
        }

        private IEnumerator ProcessMatchQueueCoroutine()
        {
            while (_matchQueue.TryDequeue(out MatchRequest matchRequest))
            {
                _activeMatchRequest = matchRequest;
                yield return new WaitForSeconds(MatchDefinition.MatchDelaySeconds);
                _activeMatchRequest = null;
                MatchEvents.RaiseMatchDelayCompleted(matchRequest);

                TrayController resolvedTrayController = ResolveTrayController();
                if (MatchExecutor.ExecuteMatch(matchRequest, resolvedTrayController))
                {
                    MatchEvents.RaiseMatchExecuted(new MatchExecutionContext(matchRequest));
                }
            }

            _processingCoroutine = null;
        }

        private void StopQueueProcessing()
        {
            if (_processingCoroutine != null)
            {
                StopCoroutine(_processingCoroutine);
                _processingCoroutine = null;
            }

            _activeMatchRequest = null;
            _matchQueue.Clear();
        }

        private TrayController ResolveTrayController()
        {
            if (trayController != null)
            {
                return trayController;
            }

            trayController = GetComponent<TrayController>();
            return trayController;
        }
    }
}
