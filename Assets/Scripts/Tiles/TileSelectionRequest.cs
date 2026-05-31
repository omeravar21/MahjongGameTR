namespace MahjongGame.Tiles
{
    public sealed class TileSelectionRequest
    {
        public Tile Tile { get; }

        public TileIdentity Identity { get; }

        public TileState StateAtRequest { get; }

        public TileSelectionRequest(Tile tile, TileIdentity identity, TileState stateAtRequest)
        {
            Tile = tile;
            Identity = identity;
            StateAtRequest = stateAtRequest;
        }
    }
}
