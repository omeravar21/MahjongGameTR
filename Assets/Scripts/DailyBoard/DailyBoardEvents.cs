using System;

namespace MahjongGame.DailyBoard
{
    public sealed class DailyBoardRefreshedContext
    {
        public DailyBoardIdentity Identity { get; }

        public DailyBoardRefreshedContext(DailyBoardIdentity identity)
        {
            Identity = identity ?? DailyBoardIdentity.Empty;
        }
    }

    public static class DailyBoardEvents
    {
        public static event Action<DailyBoardRefreshedContext> DailyBoardRefreshed;

        public static void RaiseDailyBoardRefreshed(DailyBoardRefreshedContext context)
        {
            DailyBoardRefreshed?.Invoke(context);
        }
    }
}
