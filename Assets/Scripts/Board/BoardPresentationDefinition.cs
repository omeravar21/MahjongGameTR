using UnityEngine;

namespace MahjongGame.Board
{
    public static class BoardPresentationDefinition
    {
        public const string FrameRootName = "BoardFrame";
        public const string FrameBackgroundName = "FrameBackground";
        public const string FrameTrimName = "FrameTrim";

        public const float FramePadding = 0.35f;
        public const float FrameBorderThickness = 0.08f;
        public const float BoardVerticalOffset = -0.85f;
        public const float CameraPadding = 0.6f;
        public const float TopHudReserve = 1.1f;
        public const float BottomHudReserve = 0.9f;
        public const float ReferenceAspect = 9f / 16f;
        public const float CameraDistance = 10f;

        public static readonly Color FrameBackgroundColor = new Color(0.42f, 0.30f, 0.22f, 1f);
        public static readonly Color FrameTrimColor = new Color(0.75f, 0.65f, 0.45f, 0.9f);
        public static readonly Color GameplayBackgroundColor = new Color(0.18f, 0.14f, 0.11f, 1f);

        public const int FrameBackgroundSortingOrder = -10;
        public const int FrameTrimSortingOrder = -9;
    }
}
