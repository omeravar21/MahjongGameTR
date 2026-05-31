using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.Tray
{
    public sealed class TrayCapacityController : MonoBehaviour
    {
        [SerializeField] private Transform trayRootTransform;

        public int Capacity => TrayRootDefinition.Capacity;

        public Transform TrayRootTransform => trayRootTransform != null ? trayRootTransform : transform;

        private void Awake()
        {
            if (trayRootTransform == null)
            {
                trayRootTransform = transform;
            }
        }

        public int GetOccupiedSlotCount()
        {
            Transform trayContainer = ResolveTrayContainer();
            return trayContainer != null ? TrayOccupancyQuery.CountOccupiedSlots(trayContainer) : 0;
        }

        public int GetReservedTileCount()
        {
            Transform trayContainer = ResolveTrayContainer();
            return trayContainer != null ? TrayOccupancyQuery.CountReservedTrayTiles(trayContainer) : 0;
        }

        public bool HasAvailableSlot()
        {
            return GetReservedTileCount() < Capacity;
        }

        public bool IsAtCapacity()
        {
            return GetReservedTileCount() >= Capacity;
        }

        public bool WouldAcceptAnotherTile()
        {
            return HasAvailableSlot();
        }

        public bool TryReserveSlot(Tile tile, out int slotIndex, out Transform slotTransform)
        {
            slotIndex = -1;
            slotTransform = null;

            Transform trayContainer = ResolveTrayContainer();
            if (trayContainer == null)
            {
                return false;
            }

            int reservedCount = TrayOccupancyQuery.CountReservedTrayTiles(trayContainer);
            if (reservedCount >= Capacity)
            {
                TrayCapacityEvents.RaiseTrayCapacityOverflowDetected(
                    new TrayCapacityOverflowContext(Capacity, reservedCount));
                return false;
            }

            if (!TrayOccupancyQuery.TryFindFirstAvailableSlot(trayContainer, out slotIndex, out slotTransform))
            {
                TrayCapacityEvents.RaiseTrayCapacityOverflowDetected(
                    new TrayCapacityOverflowContext(Capacity, reservedCount));
                return false;
            }

            TrayCapacityEvents.RaiseTraySlotAssigned(
                new TrayCapacitySlotAssignmentContext(tile, slotIndex, reservedCount + 1));
            return true;
        }

        internal static bool TryReserveSlot(Transform trayRoot, Tile tile, out int slotIndex, out Transform slotTransform)
        {
            slotIndex = -1;
            slotTransform = null;

            if (trayRoot == null)
            {
                return false;
            }

            TrayCapacityController capacityController = trayRoot.GetComponent<TrayCapacityController>();
            if (capacityController != null)
            {
                return capacityController.TryReserveSlot(tile, out slotIndex, out slotTransform);
            }

            Transform trayContainer = trayRoot.Find(TrayRootDefinition.TrayContainerName);
            if (trayContainer == null)
            {
                return false;
            }

            int reservedCount = TrayOccupancyQuery.CountReservedTrayTiles(trayContainer);
            if (reservedCount >= TrayRootDefinition.Capacity)
            {
                TrayCapacityEvents.RaiseTrayCapacityOverflowDetected(
                    new TrayCapacityOverflowContext(TrayRootDefinition.Capacity, reservedCount));
                return false;
            }

            return TrayOccupancyQuery.TryFindFirstAvailableSlot(trayContainer, out slotIndex, out slotTransform);
        }

        private Transform ResolveTrayContainer()
        {
            TrayRootController trayRootController = TrayRootTransform.GetComponent<TrayRootController>();
            if (trayRootController != null)
            {
                Transform container = trayRootController.GetTrayContainer();
                if (container != null)
                {
                    return container;
                }
            }

            return TrayRootTransform.Find(TrayRootDefinition.TrayContainerName);
        }
    }
}
