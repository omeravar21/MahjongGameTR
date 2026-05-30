using System;

namespace MahjongGame.Core.Save
{
    [Serializable]
    public sealed class ActiveLevelStateSaveData
    {
        public bool hasActiveSession;
        public int currentLevel;
        public int currentSeed;
        public float remainingTimer;
        public int score;
        public int currentCombo;
        public int highestComboInSession;
        public int shuffleUsed;
        public int undoUsed;
        public int hintUsed;
        public string boardStateJson = string.Empty;
        public string trayStateJson = string.Empty;
        public string closedTileStateJson = string.Empty;
        public string matchedTilesJson = string.Empty;
        public string remainingTilesJson = string.Empty;
    }
}