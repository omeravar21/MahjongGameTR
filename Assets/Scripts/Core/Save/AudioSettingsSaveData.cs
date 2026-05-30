using System;

namespace MahjongGame.Core.Save
{
    [Serializable]
    public sealed class AudioSettingsSaveData
    {
        public float musicVolume = 1f;
        public float sfxVolume = 1f;
        public bool vibrationEnabled = true;
        public string languageCode = "en";
    }
}