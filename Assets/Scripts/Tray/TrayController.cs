using System.Collections.Generic;
using MahjongGame.Matching;
using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.Tray
{
    public sealed class TrayController : MonoBehaviour
    {
        [SerializeField] private Transform trayRootTransform;
        [SerializeField] private TrayCapacityController trayCapacityController;

        private readonly Tile[] _tilesBySlot = new Tile[TrayRootDefinition.SlotCount];
        private readonly Dictionary<Tile, int> _pendingAdmissions = new Dictionary<Tile, int>();
        private readonly Tile[] _trayTilesInSlotOrderBuffer = new Tile[TrayRootDefinition.SlotCount];

        public int Capacity => trayCapacityController != null
            ? trayCapacityController.Capacity
            : TrayRootDefinition.Capacity;

        public int StoredTileCount => CountStoredTiles();

        public int ReservedTileCount => StoredTileCount + _pendingAdmissions.Count;

        private void Awake()
        {
            ResolveTrayWiring();
        }

        private void OnEnable()
        {
            TileMovementEvents.TileMovementCompleted += HandleTileMovementCompleted;
            TrayCapacityEvents.TrayCapacityOverflowDetected += HandleTrayCapacityOverflowDetected;
        }

        private void OnDisable()
        {
            TileMovementEvents.TileMovementCompleted -= HandleTileMovementCompleted;
            TrayCapacityEvents.TrayCapacityOverflowDetected -= HandleTrayCapacityOverflowDetected;
        }

        public bool TryBeginTileAdmission(Tile tile, out int slotIndex, out Transform slotTransform)
        {
            slotIndex = -1;
            slotTransform = null;

            if (tile == null)
            {
                return false;
            }

            if (_pendingAdmissions.ContainsKey(tile))
            {
                return false;
            }

            TrayCapacityController capacityController = ResolveTrayCapacityController();
            if (capacityController == null || !capacityController.TryReserveSlot(tile, out slotIndex, out slotTransform))
            {
                return false;
            }

            _pendingAdmissions[tile] = slotIndex;
            TrayEvents.RaiseTrayTileAdmissionStarted(
                new TrayTileAdmissionContext(tile, slotIndex, ReservedTileCount));
            return true;
        }

        public bool TryGetTileAtSlot(int slotIndex, out Tile tile)
        {
            tile = null;
            if (!TrayRootDefinition.IsValidSlotIndex(slotIndex))
            {
                return false;
            }

            tile = _tilesBySlot[slotIndex];
            return tile != null;
        }

        public IReadOnlyList<Tile> GetTrayTilesInSlotOrder()
        {
            for (int slotIndex = 0; slotIndex < TrayRootDefinition.SlotCount; slotIndex++)
            {
                _trayTilesInSlotOrderBuffer[slotIndex] = _tilesBySlot[slotIndex];
            }

            return _trayTilesInSlotOrderBuffer;
        }

        public bool IsAtCapacity()
        {
            TrayCapacityController capacityController = ResolveTrayCapacityController();
            return capacityController != null && capacityController.IsAtCapacity();
        }

        public bool HasAvailableSlot()
        {
            TrayCapacityController capacityController = ResolveTrayCapacityController();
            return capacityController != null && capacityController.HasAvailableSlot();
        }

        public bool ValidateSlotEmpty(int slotIndex)
        {
            if (!TrayRootDefinition.IsValidSlotIndex(slotIndex))
            {
                return false;
            }

            return _tilesBySlot[slotIndex] == null;
        }

        public void ClearPendingAdmissionForTile(Tile tile)
        {
            if (tile == null)
            {
                return;
            }

            _pendingAdmissions.Remove(tile);
        }

        public bool TryReleaseMatchedTiles(MatchRequest matchRequest)
        {
            if (matchRequest == null)
            {
                return false;
            }

            int firstSlotIndex = matchRequest.FirstSlotIndex;
            int secondSlotIndex = matchRequest.SecondSlotIndex;
            if (!TrayRootDefinition.IsValidSlotIndex(firstSlotIndex)
                || !TrayRootDefinition.IsValidSlotIndex(secondSlotIndex))
            {
                return false;
            }

            Tile firstTile = matchRequest.FirstTile;
            Tile secondTile = matchRequest.SecondTile;
            if (firstTile == null || secondTile == null)
            {
                return false;
            }

            if (_tilesBySlot[firstSlotIndex] != firstTile
                || _tilesBySlot[secondSlotIndex] != secondTile)
            {
                return false;
            }

            _tilesBySlot[firstSlotIndex] = null;
            _tilesBySlot[secondSlotIndex] = null;
            return true;
        }

        private void HandleTileMovementCompleted(TileMovementRequest request)
        {
            if (request == null || request.Tile == null)
            {
                return;
            }

            Tile tile = request.Tile;
            if (!_pendingAdmissions.TryGetValue(tile, out int pendingSlotIndex))
            {
                return;
            }

            if (pendingSlotIndex != request.SlotIndex)
            {
                Debug.LogWarning(
                    "[TrayController] Movement completed with mismatched slot index for tile "
                    + tile.name
                    + ". Expected "
                    + pendingSlotIndex
                    + ", got "
                    + request.SlotIndex);
            }

            int slotIndex = request.SlotIndex;
            if (!TrayRootDefinition.IsValidSlotIndex(slotIndex))
            {
                _pendingAdmissions.Remove(tile);
                return;
            }

            _pendingAdmissions.Remove(tile);
            _tilesBySlot[slotIndex] = tile;
            TrayEvents.RaiseTrayTileStored(
                new TrayTileStoredContext(tile, slotIndex, StoredTileCount));
        }

        private void HandleTrayCapacityOverflowDetected(TrayCapacityOverflowContext context)
        {
            TrayEvents.RaiseTrayCapacityOverflowDetected(context);
        }

        public void ResetRuntimeState()
        {
            for (int slotIndex = 0; slotIndex < TrayRootDefinition.SlotCount; slotIndex++)
            {
                DestroyRuntimeTile(_tilesBySlot[slotIndex]);
                _tilesBySlot[slotIndex] = null;
            }

            if (_pendingAdmissions.Count > 0)
            {
                foreach (Tile pendingTile in _pendingAdmissions.Keys)
                {
                    DestroyRuntimeTile(pendingTile);
                }

                _pendingAdmissions.Clear();
            }
        }

        private static void DestroyRuntimeTile(Tile tile)
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

        private int CountStoredTiles()
        {
            int storedCount = 0;
            for (int slotIndex = 0; slotIndex < TrayRootDefinition.SlotCount; slotIndex++)
            {
                if (_tilesBySlot[slotIndex] != null)
                {
                    storedCount++;
                }
            }

            return storedCount;
        }

        private void ResolveTrayWiring()
        {
            if (trayRootTransform == null)
            {
                trayRootTransform = transform.Find(TrayRootDefinition.TrayRootName);
                if (trayRootTransform == null)
                {
                    GameObject trayRootObject = GameObject.Find(TrayRootDefinition.TrayRootName);
                    trayRootTransform = trayRootObject != null ? trayRootObject.transform : null;
                }
            }

            if (trayCapacityController == null && trayRootTransform != null)
            {
                trayCapacityController = trayRootTransform.GetComponent<TrayCapacityController>();
            }

            if (trayCapacityController != null && trayRootTransform == null)
            {
                trayRootTransform = trayCapacityController.TrayRootTransform;
            }
        }

        private TrayCapacityController ResolveTrayCapacityController()
        {
            if (trayCapacityController != null)
            {
                return trayCapacityController;
            }

            ResolveTrayWiring();
            return trayCapacityController;
        }
    }
}
