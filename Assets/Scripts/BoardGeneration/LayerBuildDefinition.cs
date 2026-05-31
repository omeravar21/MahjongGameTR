using System.Collections.Generic;
using MahjongGame.Board;
using MahjongGame.Tiles;

namespace MahjongGame.BoardGeneration
{
    public static class LayerBuildDefinition
    {
        public static IReadOnlyList<TileBoardPosition> BuildPositions(
            GridMask mask,
            int tileCount,
            int layerDepth,
            int seed)
        {
            if (mask == null || tileCount <= 0 || layerDepth <= 0)
            {
                return new TileBoardPosition[0];
            }

            int clampedLayerDepth = ClampLayerDepth(layerDepth);
            List<BoardGridCoordinate> sortedCoordinates = SortEligibleCoordinates(
                mask.GetEligibleCoordinates(),
                seed);

            if (sortedCoordinates.Count == 0)
            {
                return new TileBoardPosition[0];
            }

            List<TileBoardPosition> slotCatalog = BuildSlotCatalog(sortedCoordinates, clampedLayerDepth);
            int assignedCount = tileCount < slotCatalog.Count ? tileCount : slotCatalog.Count;

            if (assignedCount <= 0)
            {
                return new TileBoardPosition[0];
            }

            return slotCatalog.GetRange(0, assignedCount);
        }

        public static int CountAvailableSlots(GridMask mask, int layerDepth)
        {
            if (mask == null || layerDepth <= 0)
            {
                return 0;
            }

            int clampedLayerDepth = ClampLayerDepth(layerDepth);
            return mask.ActiveCellCount * clampedLayerDepth;
        }

        public static int ClampLayerDepth(int layerDepth)
        {
            if (layerDepth <= 0)
            {
                return 1;
            }

            if (layerDepth > DifficultyDefinition.MaximumLayerDepth)
            {
                return DifficultyDefinition.MaximumLayerDepth;
            }

            return layerDepth;
        }

        private static List<TileBoardPosition> BuildSlotCatalog(
            IReadOnlyList<BoardGridCoordinate> sortedCoordinates,
            int layerDepth)
        {
            List<TileBoardPosition> slotCatalog = new List<TileBoardPosition>(
                sortedCoordinates.Count * layerDepth);

            for (int layerIndex = 0; layerIndex < layerDepth; layerIndex++)
            {
                for (int coordinateIndex = 0; coordinateIndex < sortedCoordinates.Count; coordinateIndex++)
                {
                    slotCatalog.Add(new TileBoardPosition(sortedCoordinates[coordinateIndex], layerIndex));
                }
            }

            return slotCatalog;
        }

        private static List<BoardGridCoordinate> SortEligibleCoordinates(
            IReadOnlyList<BoardGridCoordinate> eligibleCoordinates,
            int seed)
        {
            List<BoardGridCoordinate> sortedCoordinates = new List<BoardGridCoordinate>(eligibleCoordinates.Count);

            for (int index = 0; index < eligibleCoordinates.Count; index++)
            {
                sortedCoordinates.Add(eligibleCoordinates[index]);
            }

            sortedCoordinates.Sort((left, right) =>
            {
                int leftScore = ComputeCoordinateSortScore(left, seed);
                int rightScore = ComputeCoordinateSortScore(right, seed);

                if (leftScore != rightScore)
                {
                    return leftScore.CompareTo(rightScore);
                }

                if (left.Row != right.Row)
                {
                    return left.Row.CompareTo(right.Row);
                }

                return left.Column.CompareTo(right.Column);
            });

            return sortedCoordinates;
        }

        private static int ComputeCoordinateSortScore(BoardGridCoordinate coordinate, int seed)
        {
            unchecked
            {
                int flatIndex = coordinate.ToFlatIndex();
                return (flatIndex * 73856093) ^ (seed * 19349663);
            }
        }
    }
}
