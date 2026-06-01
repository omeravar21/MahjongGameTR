using System;
using System.IO;
using MahjongGame.Core.Save;
using UnityEngine;

namespace MahjongGame.Core
{
    public sealed class SaveSystem : MonoBehaviour
    {
        private const string SaveFolderName = "MahjongGame";
        private const string SaveFileName = "player_save.json";

        private static SaveSystem _instance;

        private PlayerSaveData _data;

        public static SaveSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[SaveSystem] Instance is not available.");
                }

                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        public PlayerSaveData Data => _data;

        public bool HasSaveFile { get; private set; }

        public string SaveFilePath { get; private set; }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            SaveFilePath = Path.Combine(Application.persistentDataPath, SaveFolderName, SaveFileName);
            Load();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                Save();
            }
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public void Load()
        {
            _data = PlayerSaveData.CreateDefault();
            HasSaveFile = false;

            if (!File.Exists(SaveFilePath))
            {
                _data.EnsureDefaults();
                Save();
                return;
            }

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                PlayerSaveData loaded = JsonUtility.FromJson<PlayerSaveData>(json);

                if (loaded == null)
                {
                    Debug.LogWarning("[SaveSystem] Save file was empty. Creating defaults.");
                    _data.EnsureDefaults();
                    Save();
                    return;
                }

                _data = loaded;
                _data.EnsureDefaults();
                HasSaveFile = true;
            }
            catch (Exception exception)
            {
                Debug.LogError("[SaveSystem] Failed to load save file. Creating defaults. " + exception.Message);
                _data = PlayerSaveData.CreateDefault();
                _data.EnsureDefaults();
                Save();
            }
        }

        public void Save()
        {
            if (_data == null)
            {
                _data = PlayerSaveData.CreateDefault();
            }

            _data.EnsureDefaults();
            _data.saveVersion = PlayerSaveData.CurrentSaveVersion;

            try
            {
                string directory = Path.GetDirectoryName(SaveFilePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonUtility.ToJson(_data, true);
                File.WriteAllText(SaveFilePath, json);
                HasSaveFile = true;
            }
            catch (Exception exception)
            {
                Debug.LogError("[SaveSystem] Failed to save data. " + exception.Message);
            }
        }

        public void ResetToDefaults()
        {
            _data = PlayerSaveData.CreateDefault();
            _data.EnsureDefaults();
            Save();
        }

        internal void EnsureValidationInstance()
        {
            if (_instance == null)
            {
                _instance = this;
            }

            if (_data == null)
            {
                _data = PlayerSaveData.CreateDefault();
                _data.EnsureDefaults();
            }
        }
    }
}