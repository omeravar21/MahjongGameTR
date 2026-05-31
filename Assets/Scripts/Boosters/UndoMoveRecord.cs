using MahjongGame.Board;
using MahjongGame.Tiles;

namespace MahjongGame.Boosters
{
    public sealed class UndoMoveRecord
    {
        public Tile Tile { get; }

        public int SlotIndex { get; }

        public TileBoardPosition OriginalBoardPosition { get; }

        public TileState RestoreState { get; }

        public UndoMoveRecord(
            Tile tile,
            int slotIndex,
            TileBoardPosition originalBoardPosition,
            TileState restoreState)
        {
            Tile = tile;
            SlotIndex = slotIndex;
            OriginalBoardPosition = originalBoardPosition;
            RestoreState = restoreState;
        }

        public static UndoMoveRecord FromMovementRequest(TileMovementRequest request)
        {
            if (request == null || request.Tile == null)
            {
                return null;
            }

            Tile tile = request.Tile;
            return new UndoMoveRecord(
                tile,
                request.SlotIndex,
                tile.OriginalBoardPosition,
                request.PreviousTileState);
        }

        public bool CanUndo()
        {
            return Tile != null && Tile.State == TileState.InTray;
        }
    }
}
