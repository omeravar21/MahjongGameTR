using System.Collections.Generic;
using MahjongGame.Tiles;
using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Matching
{
    public sealed class MatchController : MonoBehaviour
    {
        [SerializeField] private TrayController trayController;

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

            MatchEvents.RaiseMatchDetected(matchRequest);
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
