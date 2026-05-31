using MahjongGame.Tiles;

namespace MahjongGame.Boosters
{
    public sealed class HintPresentationContext
    {
        public Tile FirstTile { get; }

        public Tile SecondTile { get; }

        public HintPresentationContext(Tile firstTile, Tile secondTile)
        {
            FirstTile = firstTile;
            SecondTile = secondTile;
        }
    }
}
