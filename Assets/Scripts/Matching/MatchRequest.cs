using MahjongGame.Tiles;

namespace MahjongGame.Matching
{
    public sealed class MatchRequest
    {
        public Tile FirstTile { get; }

        public Tile SecondTile { get; }

        public int FirstSlotIndex { get; }

        public int SecondSlotIndex { get; }

        public MatchRequest(Tile firstTile, Tile secondTile, int firstSlotIndex, int secondSlotIndex)
        {
            FirstTile = firstTile;
            SecondTile = secondTile;
            FirstSlotIndex = firstSlotIndex;
            SecondSlotIndex = secondSlotIndex;
        }
    }
}
