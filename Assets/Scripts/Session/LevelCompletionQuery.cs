using MahjongGame.Board;
using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Session
{
    public static class LevelCompletionQuery
    {
        public static bool IsLevelComplete(Transform boardRoot, TrayController trayController)
        {
            if (boardRoot == null || trayController == null)
            {
                return false;
            }

            if (BoardTileOccupancyQuery.CollectOccupyingTiles(boardRoot).Count > 0)
            {
                return false;
            }

            return trayController.ReservedTileCount == 0;
        }
    }
}
