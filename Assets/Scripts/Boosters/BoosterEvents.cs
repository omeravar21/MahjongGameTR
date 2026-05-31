using System;

namespace MahjongGame.Boosters
{
    public sealed class BoosterCountsChangedContext
    {
        public BoosterCounts Counts { get; }

        public BoosterCountsChangedContext(BoosterCounts counts)
        {
            Counts = counts;
        }
    }

    public sealed class BoosterProgressionRewardGrantedContext
    {
        public int TotalLevelsCompleted { get; }

        public BoosterProgressionRewardGrantedContext(int totalLevelsCompleted)
        {
            TotalLevelsCompleted = totalLevelsCompleted;
        }
    }

    public sealed class ShuffleExecutedContext
    {
        public int ShuffledTileCount { get; }

        public ShuffleExecutedContext(int shuffledTileCount)
        {
            ShuffledTileCount = shuffledTileCount;
        }
    }

    public static class BoosterEvents
    {
        public static event Action<BoosterCountsChangedContext> BoosterCountsChanged;

        public static event Action<BoosterProgressionRewardGrantedContext> BoosterProgressionRewardGranted;

        public static event Action BoosterRuntimeReset;

        public static event Action<ShuffleExecutedContext> ShuffleExecuted;

        public static event Action<BoosterType> BoosterUsedInSession;

        internal static void RaiseBoosterCountsChanged(BoosterCountsChangedContext context)
        {
            if (context == null)
            {
                return;
            }

            BoosterCountsChanged?.Invoke(context);
        }

        internal static void RaiseBoosterProgressionRewardGranted(BoosterProgressionRewardGrantedContext context)
        {
            if (context == null)
            {
                return;
            }

            BoosterProgressionRewardGranted?.Invoke(context);
        }

        internal static void RaiseBoosterRuntimeReset()
        {
            BoosterRuntimeReset?.Invoke();
        }

        internal static void RaiseShuffleExecuted(ShuffleExecutedContext context)
        {
            if (context == null)
            {
                return;
            }

            ShuffleExecuted?.Invoke(context);
        }

        internal static void RaiseBoosterUsedInSession(BoosterType boosterType)
        {
            BoosterUsedInSession?.Invoke(boosterType);
        }
    }
}
