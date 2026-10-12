using UnityEngine;
using System.IO;
using System;

namespace Game.Core.Managers
{
    [Serializable]
    public class SaveData
    {
        // Example structure, will be expanded later
        public float musicVolume = 1.0f;
        public float sfxVolume = 1.0f;
        
        // Rewards System
        public int totalStars = 0;
        public int totalCoins = 0;
        
        // Daily Rewards
        public string lastLoginDate = "";
        public int dailyStreak = 0;
        
        // Collections & Achievements
        public System.Collections.Generic.List<string> unlockedAchievements = new System.Collections.Generic.List<string>();
        public System.Collections.Generic.List<string> unlockedStickers = new System.Collections.Generic.List<string>();
    }

    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private string _saveFilePath;
        public SaveData CurrentSaveData { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                _saveFilePath = Path.Combine(Application.persistentDataPath, "savedata.json");
                LoadGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SaveGame()
        {
            if (CurrentSaveData == null) return;

            try
            {
                string json = JsonUtility.ToJson(CurrentSaveData, true);
                File.WriteAllText(_saveFilePath, json);
                Debug.Log($"[SaveManager] Data saved to: {_saveFilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to save data: {e.Message}");
            }
        }

        public void LoadGame()
        {
            if (File.Exists(_saveFilePath))
            {
                try
                {
                    string json = File.ReadAllText(_saveFilePath);
                    CurrentSaveData = JsonUtility.FromJson<SaveData>(json);
                    Debug.Log("[SaveManager] Data loaded successfully.");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SaveManager] Failed to load data: {e.Message}");
                    CreateNewSave();
                }
            }
            else
            {
                CreateNewSave();
            }
        }

        private void CreateNewSave()
        {
            CurrentSaveData = new SaveData();
            SaveGame();
            Debug.Log("[SaveManager] Created new save data.");
        }
    }
}
