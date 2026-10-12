using UnityEngine;
using UnityEngine.UI;

namespace Game.Core.Rewards
{
    public class CompletionScreenUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject _panel;
        [SerializeField] private Text _starsEarnedText;
        [SerializeField] private Text _coinsEarnedText;
        [SerializeField] private GameObject[] _stars;
        
        [Header("Sticker Reward Visuals")]
        [SerializeField] private GameObject _stickerRewardGroup;
        [SerializeField] private Image _stickerImage;
        [SerializeField] private Text _stickerNameText;

        private void Awake()
        {
            if (_panel != null) _panel.SetActive(false);
        }

        private void OnEnable()
        {
            if (StickerCollectionManager.Instance != null)
            {
                StickerCollectionManager.Instance.OnStickerUnlocked += ShowStickerReward;
            }
        }

        private void OnDisable()
        {
            if (StickerCollectionManager.Instance != null)
            {
                StickerCollectionManager.Instance.OnStickerUnlocked -= ShowStickerReward;
            }
        }

        public void ShowCompletionScreen(int starsEarned, int coinsEarned)
        {
            if (_panel != null) _panel.SetActive(true);

            if (_starsEarnedText != null) _starsEarnedText.text = $"+{starsEarned}";
            if (_coinsEarnedText != null) _coinsEarnedText.text = $"+{coinsEarned}";

            for (int i = 0; i < _stars.Length; i++)
            {
                if (_stars[i] != null) _stars[i].SetActive(i < starsEarned);
            }

            if (_stickerRewardGroup != null) _stickerRewardGroup.SetActive(false);
            
            // Deposit the rewards globally
            if (RewardManager.Instance != null)
            {
                RewardManager.Instance.AddStars(starsEarned);
                RewardManager.Instance.AddCoins(coinsEarned);
            }
        }

        private void ShowStickerReward(StickerData sticker)
        {
            if (_stickerRewardGroup != null) _stickerRewardGroup.SetActive(true);
            if (_stickerImage != null) _stickerImage.sprite = sticker.Artwork;
            if (_stickerNameText != null) _stickerNameText.text = $"New Sticker:\n{sticker.Name}";
        }

        public void CloseScreen()
        {
            if (_panel != null) _panel.SetActive(false);
        }
    }
}
