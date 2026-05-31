using System;

namespace MahjongGame.Core.Save
{
    [Serializable]
    public sealed class SavedTileState
    {
        public int tileId;
        public int column;
        public int row;
        public int layerIndex;
        public int symbolId;
        public int tileState;
        public bool isClosed;
        public bool isJoker;
    }

    [Serializable]
    public sealed class SavedBoardState
    {
        public SavedTileState[] tiles = Array.Empty<SavedTileState>();
    }

    [Serializable]
    public sealed class SavedTraySlotState
    {
        public int slotIndex;
        public int tileId;
        public int column;
        public int row;
        public int layerIndex;
        public int symbolId;
        public int tileState;
        public bool isClosed;
        public bool isJoker;
    }

    [Serializable]
    public sealed class SavedTrayState
    {
        public SavedTraySlotState[] slots = Array.Empty<SavedTraySlotState>();
    }

    [Serializable]
    public sealed class SavedClosedTileEntry
    {
        public int tileId;
        public int closedTileState;
    }

    [Serializable]
    public sealed class SavedClosedTileStateCollection
    {
        public SavedClosedTileEntry[] entries = Array.Empty<SavedClosedTileEntry>();
    }

    [Serializable]
    public sealed class SavedTileIdCollection
    {
        public int[] tileIds = Array.Empty<int>();
    }
}
