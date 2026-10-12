using UnityEngine;
using System;
using Game.Core.Managers;
using System.Collections.Generic;

namespace Game.Core.Rewards
{
    [Serializable]
    public struct AchievementData
    {
        public string ID;
        public string Title;
        public string Description;
        public int CoinReward;
        public Sprite Icon;
    }

    [CreateAssetMenu(fileName = "AchievementDatabase", menuName = "Game Data/Achievement Database")]
    public class AchievementDatabase : ScriptableObject
    {
        public List<AchievementData> Achievements = new List<AchievementData>();
    }

    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }

        [SerializeField] private AchievementDatabase _database;

        public Action<AchievementData> OnAchievementUnlocked;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void UnlockAchievement(string id)
        {
            if (_database == null || SaveManager.Instance == null || SaveManager.Instance.CurrentSaveData == null) return;

            var unlockedList = SaveManager.Instance.CurrentSaveData.unlockedAchievements;
            
            if (unlockedList.Contains(id)) return; // Already unlocked

            // Find in database
            AchievementData? dataToUnlock = null;
            foreach (var achievement in _database.Achievements)
            {
                if (achievement.ID == id)
                {
                    dataToUnlock = achievement;
                    break;
                }
            }

            if (dataToUnlock.HasValue)
            {
                unlockedList.Add(id);
                SaveManager.Instance.SaveGame();

                if (RewardManager.Instance != null)
                {
                    RewardManager.Instance.AddCoins(dataToUnlock.Value.CoinReward);
                }

                OnAchievementUnlocked?.Invoke(dataToUnlock.Value);
                Debug.Log($"[AchievementManager] Unlocked: {dataToUnlock.Value.Title}");
            }
        }
        
        public bool IsUnlocked(string id)
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData != null)
            {
                return SaveManager.Instance.CurrentSaveData.unlockedAchievements.Contains(id);
            }
            return false;
        }
    }
}
