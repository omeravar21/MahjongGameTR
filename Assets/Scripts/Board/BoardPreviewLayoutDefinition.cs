namespace MahjongGame.Board
{
    public readonly struct BoardPreviewTileSpec
    {
        public int LayerIndex { get; }

        public int Column { get; }

        public int Row { get; }

        public int TileId { get; }

        public BoardPreviewTileSpec(int layerIndex, int column, int row, int tileId)
        {
            LayerIndex = layerIndex;
            Column = column;
            Row = row;
            TileId = tileId;
        }
    }

    public static class BoardPreviewLayoutDefinition
    {
        public const string PreviewNamePrefix = "LayerPreview_";

        public static readonly BoardPreviewTileSpec[] DefaultTiles =
        {
            new BoardPreviewTileSpec(0, 2, 3, 100),
            new BoardPreviewTileSpec(1, 2, 3, 101),
            new BoardPreviewTileSpec(2, 2, 3, 102),
            new BoardPreviewTileSpec(3, 2, 3, 103),
            new BoardPreviewTileSpec(0, 0, 1, 110),
            new BoardPreviewTileSpec(1, 4, 2, 111),
            new BoardPreviewTileSpec(2, 5, 5, 112),
        };

        public static string GetPreviewTileName(BoardPreviewTileSpec spec)
        {
            return PreviewNamePrefix + spec.LayerIndex + "_" + spec.Column + "_" + spec.Row;
        }
    }
}
