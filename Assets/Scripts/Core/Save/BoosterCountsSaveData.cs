using System;

namespace MahjongGame.Core.Save
{
    [Serializable]
    public sealed class BoosterCountsSaveData
    {
        public int shuffle;
        public int undo;
        public int hint;
    }
}