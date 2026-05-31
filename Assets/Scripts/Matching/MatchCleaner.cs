using MahjongGame.Tiles;
using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Matching
{
    public static class MatchCleaner
    {
        public static bool CleanupMatch(MatchRequest request, TrayController trayController)
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

            if (!EnsureSlotsReleased(request, trayController))
            {
                return false;
            }

            trayController.ClearPendingAdmissionForTile(firstTile);
            trayController.ClearPendingAdmissionForTile(secondTile);

            DestroyMatchedTile(firstTile);
            DestroyMatchedTile(secondTile);

            if (!trayController.ValidateSlotEmpty(request.FirstSlotIndex)
                || !trayController.ValidateSlotEmpty(request.SecondSlotIndex))
            {
                return false;
            }

            return true;
        }

        private static bool EnsureSlotsReleased(MatchRequest request, TrayController trayController)
        {
            if (trayController.ValidateSlotEmpty(request.FirstSlotIndex)
                && trayController.ValidateSlotEmpty(request.SecondSlotIndex))
            {
                return true;
            }

            return trayController.TryReleaseMatchedTiles(request);
        }

        private static void DestroyMatchedTile(Tile tile)
        {
            if (tile == null)
            {
                return;
            }

            GameObject tileObject = tile.gameObject;
            if (tileObject == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(tileObject);
            }
            else
            {
                Object.DestroyImmediate(tileObject);
            }
        }
    }
}
