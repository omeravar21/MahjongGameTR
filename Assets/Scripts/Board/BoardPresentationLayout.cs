using UnityEngine;

namespace MahjongGame.Board
{
    public static class BoardPresentationLayout
    {
        public static Vector2 GetBoardContentHalfExtents()
        {
            float halfWidth = BoardGridDefinition.ColumnCount * BoardGridDefinition.DefaultCellWidth * 0.5f;
            float halfHeight = BoardGridDefinition.RowCount * BoardGridDefinition.DefaultCellHeight * 0.5f;
            return new Vector2(halfWidth, halfHeight);
        }

        public static Vector2 GetFramedBoardHalfExtents()
        {
            Vector2 contentHalfExtents = GetBoardContentHalfExtents();
            return contentHalfExtents + new Vector2(BoardPresentationDefinition.FramePadding, BoardPresentationDefinition.FramePadding);
        }

        public static Vector3 GetBoardRootLocalPosition()
        {
            return new Vector3(0f, BoardPresentationDefinition.BoardVerticalOffset, 0f);
        }

        public static Vector3 GetCameraTargetPosition(Vector3 boardRootLocalPosition)
        {
            return new Vector3(0f, boardRootLocalPosition.y, -BoardPresentationDefinition.CameraDistance);
        }

        public static float CalculateOrthographicSize(float aspect)
        {
            if (aspect <= 0f)
            {
                aspect = BoardPresentationDefinition.ReferenceAspect;
            }

            Vector2 framedHalfExtents = GetFramedBoardHalfExtents();
            float verticalNeed = framedHalfExtents.y
                + BoardPresentationDefinition.TopHudReserve
                + BoardPresentationDefinition.BottomHudReserve;
            float horizontalNeed = framedHalfExtents.x / aspect;
            return Mathf.Max(verticalNeed, horizontalNeed) + BoardPresentationDefinition.CameraPadding;
        }
    }
}
