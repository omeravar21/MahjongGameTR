namespace MahjongGame.Tray
{
    public sealed class TrayCapacityOverflowContext
    {
        public int Capacity { get; }

        public int ReservedCount { get; }

        public TrayCapacityOverflowContext(int capacity, int reservedCount)
        {
            Capacity = capacity;
            ReservedCount = reservedCount;
        }
    }
}
