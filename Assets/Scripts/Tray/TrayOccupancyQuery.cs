using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.Tray
{
    public static class TrayOccupancyQuery
    {
        public static bool OccupiesTraySlot(Tile tile)
        {
            if (tile == null)
            {
                return false;
            }

            return tile.State == TileState.InTray || tile.State == TileState.MovingToTray;
        }

        public static int CountOccupiedSlots(Transform trayContainer)
        {
            if (trayContainer == null)
            {
                return 0;
            }

            int occupiedCount = 0;
            for (int slotIndex = 0; slotIndex < TrayRootDefinition.SlotCount; slotIndex++)
            {
                Transform slotTransform = trayContainer.Find(TrayRootDefinition.GetSlotName(slotIndex));
                if (slotTransform != null && SlotHasOccupyingTile(slotTransform))
                {
                    occupiedCount++;
                }
            }

            return occupiedCount;
        }

        public static int CountTilesInFlight(Transform trayContainer)
        {
            if (trayContainer == null)
            {
                return 0;
            }

            int inFlightCount = 0;
            for (int slotIndex = 0; slotIndex < TrayRootDefinition.SlotCount; slotIndex++)
            {
                Transform slotTransform = trayContainer.Find(TrayRootDefinition.GetSlotName(slotIndex));
                if (slotTransform == null)
                {
                    continue;
                }

                inFlightCount += CountTilesInSlotWithState(slotTransform, TileState.MovingToTray);
            }

            return inFlightCount;
        }

        public static int CountReservedTrayTiles(Transform trayContainer)
        {
            if (trayContainer == null)
            {
                return 0;
            }

            int reservedCount = 0;
            for (int slotIndex = 0; slotIndex < TrayRootDefinition.SlotCount; slotIndex++)
            {
                Transform slotTransform = trayContainer.Find(TrayRootDefinition.GetSlotName(slotIndex));
                if (slotTransform == null)
                {
                    continue;
                }

                reservedCount += CountTilesInSlotWithState(slotTransform, TileState.InTray, TileState.MovingToTray);
            }

            return reservedCount;
        }

        public static bool SlotHasOccupyingTile(Transform slotTransform)
        {
            return TryGetOccupyingTile(slotTransform, out _);
        }

        public static bool TryGetOccupyingTile(Transform slotTransform, out Tile occupyingTile)
        {
            occupyingTile = null;
            if (slotTransform == null)
            {
                return false;
            }

            for (int childIndex = 0; childIndex < slotTransform.childCount; childIndex++)
            {
                Tile tile = slotTransform.GetChild(childIndex).GetComponent<Tile>();
                if (tile == null || !OccupiesTraySlot(tile))
                {
                    continue;
                }

                occupyingTile = tile;
                return true;
            }

            return false;
        }

        public static bool TryFindFirstAvailableSlot(
            Transform trayContainer,
            out int slotIndex,
            out Transform slotTransform)
        {
            slotIndex = -1;
            slotTransform = null;

            if (trayContainer == null)
            {
                return false;
            }

            for (int i = 0; i < TrayRootDefinition.SlotCount; i++)
            {
                Transform candidateSlot = trayContainer.Find(TrayRootDefinition.GetSlotName(i));
                if (candidateSlot == null)
                {
                    continue;
                }

                if (SlotHasOccupyingTile(candidateSlot))
                {
                    continue;
                }

                slotIndex = i;
                slotTransform = candidateSlot;
                return true;
            }

            return false;
        }

        private static int CountTilesInSlotWithState(Transform slotTransform, params TileState[] states)
        {
            int count = 0;
            for (int childIndex = 0; childIndex < slotTransform.childCount; childIndex++)
            {
                Tile tile = slotTransform.GetChild(childIndex).GetComponent<Tile>();
                if (tile == null)
                {
                    continue;
                }

                for (int stateIndex = 0; stateIndex < states.Length; stateIndex++)
                {
                    if (tile.State == states[stateIndex])
                    {
                        count++;
                        break;
                    }
                }
            }

            return count;
        }
    }
}
