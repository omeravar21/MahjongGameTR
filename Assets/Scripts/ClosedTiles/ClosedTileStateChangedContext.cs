namespace MahjongGame.ClosedTiles
{
    public sealed class ClosedTileStateChangedContext
    {
        public int TileId { get; }

        public ClosedTileState PreviousState { get; }

        public ClosedTileState CurrentState { get; }

        public ClosedTileStateChangedContext(
            int tileId,
            ClosedTileState previousState,
            ClosedTileState currentState)
        {
            TileId = tileId;
            PreviousState = previousState;
            CurrentState = currentState;
        }
    }
}
