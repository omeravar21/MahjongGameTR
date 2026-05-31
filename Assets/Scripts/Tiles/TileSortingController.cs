using MahjongGame.Board;

namespace MahjongGame.Tiles
{
    public static class TileSortingController
    {
        public static int CalculateSortingOrder(int layerIndex, int row, int column)
        {
            return layerIndex * BoardLayerDefinition.SortingOrdersPerLayer
                + row * BoardGridDefinition.ColumnCount
                + column;
        }

        public static void ApplySorting(TileView view, int layerIndex, int row, int column)
        {
            if (view == null)
            {
                return;
            }

            view.ApplySorting(CalculateSortingOrder(layerIndex, row, column));
        }
    }
}
