using System;

namespace MahjongGame.Core.Save
{
    [Serializable]
    public sealed class DailyMissionSaveData
    {
        public const int SlotCount = 5;

        public int missionDayId;
        public int[] slotMissionTypes = new int[SlotCount];
        public int[] slotProgress = new int[SlotCount];
        public bool[] slotCompleted = new bool[SlotCount];
        public bool[] slotRewardClaimed = new bool[SlotCount];

        public void EnsureDefaults()
        {
            if (missionDayId < 0)
            {
                missionDayId = 0;
            }

            slotMissionTypes ??= new int[SlotCount];
            slotProgress ??= new int[SlotCount];
            slotCompleted ??= new bool[SlotCount];
            slotRewardClaimed ??= new bool[SlotCount];

            if (slotMissionTypes.Length != SlotCount)
            {
                slotMissionTypes = ResizeIntArray(slotMissionTypes, SlotCount);
            }

            if (slotProgress.Length != SlotCount)
            {
                slotProgress = ResizeIntArray(slotProgress, SlotCount);
            }

            if (slotCompleted.Length != SlotCount)
            {
                slotCompleted = ResizeBoolArray(slotCompleted, SlotCount);
            }

            if (slotRewardClaimed.Length != SlotCount)
            {
                slotRewardClaimed = ResizeBoolArray(slotRewardClaimed, SlotCount);
            }

            for (int i = 0; i < SlotCount; i++)
            {
                if (slotProgress[i] < 0)
                {
                    slotProgress[i] = 0;
                }
            }
        }

        private static int[] ResizeIntArray(int[] source, int length)
        {
            int[] resized = new int[length];
            if (source == null)
            {
                return resized;
            }

            int copyCount = Math.Min(source.Length, length);
            Array.Copy(source, resized, copyCount);
            return resized;
        }

        private static bool[] ResizeBoolArray(bool[] source, int length)
        {
            bool[] resized = new bool[length];
            if (source == null)
            {
                return resized;
            }

            int copyCount = Math.Min(source.Length, length);
            Array.Copy(source, resized, copyCount);
            return resized;
        }
    }
}
