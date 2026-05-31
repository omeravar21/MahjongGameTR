using System.Collections.Generic;
using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.Board
{
    public static class BoardTileOccupancyQuery
    {
        public static bool OccupiesBoardCell(Tile tile)
        {
            if (tile == null)
            {
                return false;
            }

            switch (tile.State)
            {
                case TileState.OnBoard:
                case TileState.Closed:
                case TileState.Revealed:
                    return tile.GridCoordinate.IsValid
                        && BoardLayerDefinition.IsValidLayerIndex(tile.LayerIndex);
                default:
                    return false;
            }
        }

        public static bool IsCellOccupied(
            Transform boardRoot,
            int column,
            int row,
            int layerIndex,
            Tile excludeTile)
        {
            return TryGetOccupyingTile(boardRoot, column, row, layerIndex, excludeTile, out _);
        }

        public static bool TryGetOccupyingTile(
            Transform boardRoot,
            int column,
            int row,
            int layerIndex,
            Tile excludeTile,
            out Tile occupyingTile)
        {
            occupyingTile = null;

            if (boardRoot == null
                || !BoardGridDefinition.IsValidCoordinate(column, row)
                || !BoardLayerDefinition.IsValidLayerIndex(layerIndex))
            {
                return false;
            }

            Transform layerContainer = boardRoot.Find(BoardRootController.GetLayerContainerName(layerIndex));
            if (layerContainer == null)
            {
                return false;
            }

            Tile[] tiles = layerContainer.GetComponentsInChildren<Tile>(includeInactive: false);
            for (int i = 0; i < tiles.Length; i++)
            {
                Tile candidate = tiles[i];
                if (candidate == null || candidate == excludeTile || !OccupiesBoardCell(candidate))
                {
                    continue;
                }

                if (candidate.GridCoordinate.Column == column && candidate.GridCoordinate.Row == row)
                {
                    occupyingTile = candidate;
                    return true;
                }
            }

            return false;
        }

        public static bool HasUpperBlockingTile(Transform boardRoot, Tile tile)
        {
            if (tile == null || boardRoot == null)
            {
                return false;
            }

            BoardGridCoordinate coordinate = tile.GridCoordinate;
            if (!coordinate.IsValid)
            {
                return false;
            }

            for (int layerIndex = tile.LayerIndex + 1; layerIndex < BoardLayerDefinition.MaxLayerCount; layerIndex++)
            {
                if (IsCellOccupied(boardRoot, coordinate.Column, coordinate.Row, layerIndex, excludeTile: null))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasBothSidesBlocked(Transform boardRoot, Tile tile)
        {
            if (tile == null || boardRoot == null)
            {
                return false;
            }

            BoardGridCoordinate coordinate = tile.GridCoordinate;
            int layerIndex = tile.LayerIndex;
            if (!coordinate.IsValid || !BoardLayerDefinition.IsValidLayerIndex(layerIndex))
            {
                return false;
            }

            bool leftBlocked = coordinate.Column > 0
                && IsCellOccupied(boardRoot, coordinate.Column - 1, coordinate.Row, layerIndex, excludeTile: tile);
            bool rightBlocked = coordinate.Column < BoardGridDefinition.ColumnCount - 1
                && IsCellOccupied(boardRoot, coordinate.Column + 1, coordinate.Row, layerIndex, excludeTile: tile);

            return leftBlocked && rightBlocked;
        }

        public static Transform ResolveBoardRoot(Tile tile)
        {
            if (tile == null)
            {
                return null;
            }

            Transform current = tile.transform;
            while (current != null)
            {
                if (current.name == "BoardRoot" && BoardRootController.HasRequiredBoardHierarchy(current))
                {
                    return current;
                }

                current = current.parent;
            }

            return null;
        }

        public static Transform ResolveBoardRootFromScene()
        {
            BoardRootController boardRootController = Object.FindFirstObjectByType<BoardRootController>();
            return boardRootController != null ? boardRootController.BoardRootTransform : null;
        }

        public static List<Tile> CollectOccupyingTiles(Transform boardRoot)
        {
            List<Tile> occupyingTiles = new List<Tile>();
            if (boardRoot == null)
            {
                return occupyingTiles;
            }

            for (int layerIndex = 0; layerIndex < BoardLayerDefinition.MaxLayerCount; layerIndex++)
            {
                Transform layerContainer = boardRoot.Find(BoardRootController.GetLayerContainerName(layerIndex));
                if (layerContainer == null)
                {
                    continue;
                }

                Tile[] tiles = layerContainer.GetComponentsInChildren<Tile>(includeInactive: false);
                for (int i = 0; i < tiles.Length; i++)
                {
                    Tile tile = tiles[i];
                    if (OccupiesBoardCell(tile))
                    {
                        occupyingTiles.Add(tile);
                    }
                }
            }

            return occupyingTiles;
        }
    }
}
