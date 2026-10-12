using UnityEngine;
using System;
using Game.Core.Managers;

namespace Game.Core.Rewards
{
    public class RewardManager : MonoBehaviour
    {
        public static RewardManager Instance { get; private set; }

        public Action<int> OnStarsChanged;
        public Action<int> OnCoinsChanged;

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

        public void AddStars(int amount)
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData != null)
            {
                SaveManager.Instance.CurrentSaveData.totalStars += amount;
                SaveManager.Instance.SaveGame();
                OnStarsChanged?.Invoke(SaveManager.Instance.CurrentSaveData.totalStars);
            }
        }

        public void AddCoins(int amount)
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData != null)
            {
                SaveManager.Instance.CurrentSaveData.totalCoins += amount;
                SaveManager.Instance.SaveGame();
                OnCoinsChanged?.Invoke(SaveManager.Instance.CurrentSaveData.totalCoins);
            }
        }

        public bool SpendCoins(int amount)
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData != null)
            {
                if (SaveManager.Instance.CurrentSaveData.totalCoins >= amount)
                {
                    SaveManager.Instance.CurrentSaveData.totalCoins -= amount;
                    SaveManager.Instance.SaveGame();
                    OnCoinsChanged?.Invoke(SaveManager.Instance.CurrentSaveData.totalCoins);
                    return true;
                }
            }
            return false;
        }

        public int GetTotalStars()
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData != null)
                return SaveManager.Instance.CurrentSaveData.totalStars;
            return 0;
        }

        public int GetTotalCoins()
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData != null)
                return SaveManager.Instance.CurrentSaveData.totalCoins;
            return 0;
        }
    }
}
