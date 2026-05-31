using MahjongGame.Board;

namespace MahjongGame.Tiles
{
    public static class TileSortingController
    {
        public const int TraySortingBase =
            BoardLayerDefinition.MaxLayerCount * BoardLayerDefinition.SortingOrdersPerLayer;

        public static int CalculateSortingOrder(int layerIndex, int row, int column)
        {
            return layerIndex * BoardLayerDefinition.SortingOrdersPerLayer
                + row * BoardGridDefinition.ColumnCount
                + column;
        }

        public static int CalculateTraySortingOrder(int slotIndex)
        {
            return TraySortingBase + slotIndex * 3;
        }

        public static void ApplySorting(TileView view, int layerIndex, int row, int column)
        {
            if (view == null)
            {
                return;
            }

            view.ApplySorting(CalculateSortingOrder(layerIndex, row, column));
        }

        public static void ApplyTraySorting(TileView view, int slotIndex)
        {
            if (view == null)
            {
                return;
            }

            view.ApplySorting(CalculateTraySortingOrder(slotIndex));
        }
    }
}
