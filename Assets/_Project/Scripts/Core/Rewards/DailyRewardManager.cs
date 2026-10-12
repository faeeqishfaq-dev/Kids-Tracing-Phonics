using UnityEngine;
using System;
using Game.Core.Managers;

namespace Game.Core.Rewards
{
    public class DailyRewardManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _baseCoinReward = 10;
        [SerializeField] private int _streakMultiplier = 5;
        [SerializeField] private int _maxStreak = 7;

        public Action<int, int> OnDailyRewardGranted; // Coins granted, Current Streak

        private void Start()
        {
            // Delay check to ensure SaveManager is fully loaded
            Invoke(nameof(CheckDailyReward), 1.0f);
        }

        private void CheckDailyReward()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.CurrentSaveData == null) return;

            string lastLoginStr = SaveManager.Instance.CurrentSaveData.lastLoginDate;
            DateTime lastLogin;
            
            bool isFirstTime = string.IsNullOrEmpty(lastLoginStr) || !DateTime.TryParse(lastLoginStr, out lastLogin);
            
            DateTime today = DateTime.Now.Date;

            if (isFirstTime)
            {
                GrantReward(today, 1);
            }
            //else
            //{
            //    TimeSpan timeSinceLastLogin = today - lastLogin.Date;

            //    if (timeSinceLastLogin.Days == 1)
            //    {
            //        // Consecutive day
            //        int newStreak = SaveManager.Instance.CurrentSaveData.dailyStreak + 1;
            //        if (newStreak > _maxStreak) newStreak = _maxStreak;
            //        GrantReward(today, newStreak);
            //    }
            //    else if (timeSinceLastLogin.Days > 1)
            //    {
            //        // Streak broken
            //        GrantReward(today, 1);
            //    }
            //    else
            //    {
            //        // Already claimed today
            //        Debug.Log("[DailyReward] Reward already claimed for today.");
            //    }
            //}
        }

        private void GrantReward(DateTime date, int streak)
        {
            int rewardAmount = _baseCoinReward + (streak * _streakMultiplier);
            
            // Update Save Data
            SaveManager.Instance.CurrentSaveData.lastLoginDate = date.ToString("yyyy-MM-dd");
            SaveManager.Instance.CurrentSaveData.dailyStreak = streak;
            SaveManager.Instance.SaveGame();

            // Grant Coins
            if (RewardManager.Instance != null)
            {
                RewardManager.Instance.AddCoins(rewardAmount);
            }

            OnDailyRewardGranted?.Invoke(rewardAmount, streak);
            Debug.Log($"[DailyReward] Granted {rewardAmount} coins for day {streak} of streak!");
        }
    }
}
