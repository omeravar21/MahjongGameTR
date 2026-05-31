using MahjongGame.Tiles;
using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Matching
{
    public static class MatchExecutor
    {
        public static bool ExecuteMatch(MatchRequest request, TrayController trayController)
        {
            if (request == null || trayController == null)
            {
                return false;
            }

            Tile firstTile = request.FirstTile;
            Tile secondTile = request.SecondTile;
            if (firstTile == null || secondTile == null)
            {
                return false;
            }

            if (!trayController.TryGetTileAtSlot(request.FirstSlotIndex, out Tile firstTileAtSlot)
                || firstTileAtSlot != firstTile)
            {
                return false;
            }

            if (!trayController.TryGetTileAtSlot(request.SecondSlotIndex, out Tile secondTileAtSlot)
                || secondTileAtSlot != secondTile)
            {
                return false;
            }

            if (!TileMatchComparer.AreMatching(firstTile, secondTile))
            {
                return false;
            }

            if (!trayController.TryReleaseMatchedTiles(request))
            {
                return false;
            }

            firstTile.SetState(TileState.Matched);
            secondTile.SetState(TileState.Matched);

            firstTile.SetColliderEnabled(false);
            secondTile.SetColliderEnabled(false);

            HideMatchedTile(firstTile);
            HideMatchedTile(secondTile);

            return true;
        }

        private static void HideMatchedTile(Tile tile)
        {
            if (tile == null)
            {
                return;
            }

            Transform tileTransform = tile.transform;
            if (tileTransform.parent != null)
            {
                tileTransform.SetParent(null, worldPositionStays: true);
            }

            tile.gameObject.SetActive(false);
        }
    }
}
