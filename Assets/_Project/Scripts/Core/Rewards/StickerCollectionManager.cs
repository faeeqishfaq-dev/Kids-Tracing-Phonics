using UnityEngine;
using System;
using System.Collections.Generic;
using Game.Core.Managers;

namespace Game.Core.Rewards
{
    [Serializable]
    public struct StickerData
    {
        public string ID;
        public string Name;
        public Sprite Artwork;
    }

    [CreateAssetMenu(fileName = "StickerDatabase", menuName = "Game Data/Sticker Database")]
    public class StickerDatabase : ScriptableObject
    {
        public List<StickerData> Stickers = new List<StickerData>();
    }

    public class StickerCollectionManager : MonoBehaviour
    {
        public static StickerCollectionManager Instance { get; private set; }

        [SerializeField] private StickerDatabase _database;

        public Action<StickerData> OnStickerUnlocked;

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

        public void UnlockRandomSticker()
        {
            if (_database == null || _database.Stickers.Count == 0) return;
            if (SaveManager.Instance == null || SaveManager.Instance.CurrentSaveData == null) return;

            var unlockedList = SaveManager.Instance.CurrentSaveData.unlockedStickers;
            
            // Find locked stickers
            List<StickerData> lockedStickers = new List<StickerData>();
            foreach (var sticker in _database.Stickers)
            {
                if (!unlockedList.Contains(sticker.ID))
                {
                    lockedStickers.Add(sticker);
                }
            }

            if (lockedStickers.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, lockedStickers.Count);
                StickerData awardedSticker = lockedStickers[randomIndex];
                
                unlockedList.Add(awardedSticker.ID);
                SaveManager.Instance.SaveGame();
                
                OnStickerUnlocked?.Invoke(awardedSticker);
                Debug.Log($"[StickerManager] Unlocked Sticker: {awardedSticker.Name}");
            }
            else
            {
                Debug.Log("[StickerManager] All stickers already unlocked.");
                // Fallback: Reward coins instead if collection is full
                if (RewardManager.Instance != null) RewardManager.Instance.AddCoins(50);
            }
        }
        
        public bool IsStickerUnlocked(string id)
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData != null)
            {
                return SaveManager.Instance.CurrentSaveData.unlockedStickers.Contains(id);
            }
            return false;
        }
    }
}
