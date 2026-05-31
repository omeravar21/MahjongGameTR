using MahjongGame.Tray;

namespace MahjongGame.Matching
{
    public sealed class MatchDetectionContext
    {
        public TrayTileStoredContext TriggerContext { get; }

        public MatchDetectionContext(TrayTileStoredContext triggerContext)
        {
            TriggerContext = triggerContext;
        }
    }
}
