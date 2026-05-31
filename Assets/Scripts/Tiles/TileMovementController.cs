using System.Collections;
using System.Collections.Generic;
using MahjongGame.Tray;
using UnityEngine;

namespace MahjongGame.Tiles
{
    public sealed class TileMovementController : MonoBehaviour
    {
        [SerializeField] private Transform trayRootTransform;
        [SerializeField] private float movementDurationSeconds = TrayMovementLayout.MovementDurationSeconds;

        private readonly HashSet<Tile> _tilesInFlight = new HashSet<Tile>();

        private void Awake()
        {
            ResolveTrayRootTransform();
        }

        public bool TryBeginMovement(Tile tile)
        {
            return TryBeginMovement(tile, out _);
        }

        public bool TryBeginMovement(Tile tile, out TileInteractionBlockReason blockReason)
        {
            blockReason = TileInteractionBlockReason.None;

            if (tile == null)
            {
                blockReason = TileInteractionBlockReason.InvalidTile;
                return false;
            }

            if (_tilesInFlight.Contains(tile))
            {
                blockReason = TileInteractionBlockReason.AlreadyMoving;
                return false;
            }

            if (!CanBeginMovement(tile))
            {
                blockReason = TileInteractionBlockReason.InvalidState;
                return false;
            }

            if (ResolveTrayContainerTransform() == null)
            {
                blockReason = TileInteractionBlockReason.MissingSceneWiring;
                return false;
            }

            if (!TryFindAvailableSlot(out int slotIndex, out Transform slotTransform))
            {
                blockReason = TileInteractionBlockReason.NoTraySlotAvailable;
                return false;
            }

            Vector3 startWorldPosition = tile.transform.position;
            Vector3 targetWorldPosition = slotTransform.position;
            TileMovementRequest request = new TileMovementRequest(
                tile,
                slotIndex,
                startWorldPosition,
                targetWorldPosition,
                slotTransform);

            _tilesInFlight.Add(tile);
            tile.SetState(TileState.MovingToTray);
            tile.SetColliderEnabled(false);
            TileMovementEvents.RaiseTileMovementStarted(request);

            if (TileMovementAnimationHooks.TryInvokeCustomMovement(request, () => CompleteMovement(request)))
            {
                return true;
            }

            StartCoroutine(AnimateMovementCoroutine(request));
            return true;
        }

        public static bool CanBeginMovement(Tile tile)
        {
            if (tile == null)
            {
                return false;
            }

            switch (tile.State)
            {
                case TileState.OnBoard:
                case TileState.Closed:
                case TileState.Revealed:
                    return true;
                default:
                    return false;
            }
        }

        private IEnumerator AnimateMovementCoroutine(TileMovementRequest request)
        {
            Tile tile = request.Tile;
            Transform tileTransform = tile.transform;
            float duration = Mathf.Max(0.01f, movementDurationSeconds);
            float elapsed = 0f;

            tileTransform.SetParent(transform, worldPositionStays: true);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(elapsed / duration);
                float easedTime = EaseOutQuad(normalizedTime);
                tileTransform.position = Vector3.Lerp(
                    request.StartWorldPosition,
                    request.TargetWorldPosition,
                    easedTime);
                yield return null;
            }

            CompleteMovement(request);
        }

        private void CompleteMovement(TileMovementRequest request)
        {
            if (request == null || request.Tile == null)
            {
                return;
            }

            Tile tile = request.Tile;
            _tilesInFlight.Remove(tile);

            Transform slotTransform = request.TargetSlotTransform;
            if (slotTransform != null)
            {
                Vector3 preservedScale = tile.transform.localScale;
                tile.transform.SetParent(slotTransform, worldPositionStays: false);
                tile.transform.localPosition = Vector3.zero;
                tile.transform.localRotation = Quaternion.identity;
                tile.transform.localScale = preservedScale;
            }

            tile.ApplyTraySorting(request.SlotIndex);
            tile.SetState(TileState.InTray);
            tile.SetColliderEnabled(false);
            TileMovementEvents.RaiseTileMovementCompleted(request);
        }

        private bool TryFindAvailableSlot(out int slotIndex, out Transform slotTransform)
        {
            slotIndex = -1;
            slotTransform = null;

            Transform trayContainer = ResolveTrayContainerTransform();
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

        private static bool SlotHasOccupyingTile(Transform slotTransform)
        {
            for (int childIndex = 0; childIndex < slotTransform.childCount; childIndex++)
            {
                Tile tile = slotTransform.GetChild(childIndex).GetComponent<Tile>();
                if (tile == null)
                {
                    continue;
                }

                if (tile.State == TileState.InTray || tile.State == TileState.MovingToTray)
                {
                    return true;
                }
            }

            return false;
        }

        private Transform ResolveTrayContainerTransform()
        {
            Transform trayRoot = ResolveTrayRootTransform();
            if (trayRoot == null)
            {
                return null;
            }

            TrayRootController trayRootController = trayRoot.GetComponent<TrayRootController>();
            if (trayRootController != null)
            {
                Transform container = trayRootController.GetTrayContainer();
                if (container != null)
                {
                    return container;
                }
            }

            Transform trayContainer = trayRoot.Find(TrayRootDefinition.TrayContainerName);
            return trayContainer != null ? trayContainer : trayRoot;
        }

        private Transform ResolveTrayRootTransform()
        {
            if (trayRootTransform != null)
            {
                return trayRootTransform;
            }

            trayRootTransform = transform.Find("TrayRoot");
            if (trayRootTransform != null)
            {
                return trayRootTransform;
            }

            GameObject trayRootObject = GameObject.Find("TrayRoot");
            trayRootTransform = trayRootObject != null ? trayRootObject.transform : null;
            return trayRootTransform;
        }

        private static float EaseOutQuad(float normalizedTime)
        {
            float inverted = 1f - normalizedTime;
            return 1f - inverted * inverted;
        }
    }
}
