using UnityEngine;

namespace MahjongGame.Tiles
{
    public sealed class TileMovementRequest
    {
        public Tile Tile { get; }

        public int SlotIndex { get; }

        public Vector3 StartWorldPosition { get; }

        public Vector3 TargetWorldPosition { get; }

        public Transform TargetSlotTransform { get; }

        public TileMovementRequest(
            Tile tile,
            int slotIndex,
            Vector3 startWorldPosition,
            Vector3 targetWorldPosition,
            Transform targetSlotTransform)
        {
            Tile = tile;
            SlotIndex = slotIndex;
            StartWorldPosition = startWorldPosition;
            TargetWorldPosition = targetWorldPosition;
            TargetSlotTransform = targetSlotTransform;
        }
    }
}
