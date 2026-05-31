using MahjongGame.Progression;

namespace MahjongGame.ClosedTiles
{
    public static class ClosedTileDefinition
    {
        public const int ActivationLevel = 10;

        public static bool IsClosedTileMechanicActive(int levelNumber)
        {
            return LevelProgressData.ClampLevel(levelNumber) >= ActivationLevel;
        }
    }
}
