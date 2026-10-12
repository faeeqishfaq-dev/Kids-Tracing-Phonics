using UnityEngine;
using System;
using Game.Core.Managers;

namespace Game.Gameplay.ABCLearning
{
    public class LetterManager : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private AlphabetDatabase _alphabetDatabase;

        private int _currentIndex = 0;

        // Local Events
        public Action<LetterData> OnLetterChanged;
        public Action<bool> OnCanNavigatePrevious;
        public Action<bool> OnCanNavigateNext;

        private void Start()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
            {
                InitializeModule();
            }
            else
            {
                // Fallback for isolated scene testing
                InitializeModule();
            }
        }

        public void InitializeModule()
        {
            _currentIndex = 0;
            UpdateLetter();
        }

        public void NextLetter()
        {
            if (_alphabetDatabase == null) return;
            
            if (_currentIndex < _alphabetDatabase.GetTotalLetters() - 1)
            {
                _currentIndex++;
                UpdateLetter();
            }
        }

        public void PreviousLetter()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                UpdateLetter();
            }
        }

        private void UpdateLetter()
        {
            if (_alphabetDatabase == null || _alphabetDatabase.GetTotalLetters() == 0)
            {
                Debug.LogWarning("[LetterManager] AlphabetDatabase is empty or missing.");
                return;
            }

            LetterData currentData = _alphabetDatabase.GetLetterData(_currentIndex);
            
            // Broadcast new letter
            OnLetterChanged?.Invoke(currentData);
            
            // Broadcast navigation states
            OnCanNavigatePrevious?.Invoke(_currentIndex > 0);
            OnCanNavigateNext?.Invoke(_currentIndex < _alphabetDatabase.GetTotalLetters() - 1);

            // Play automatic pronunciation
            if (AudioManager.Instance != null && currentData.PronunciationAudio != null)
            {
                AudioManager.Instance.PlaySFX(currentData.PronunciationAudio);
            }
        }

        public void PlayExampleWordAudio()
        {
            if (_alphabetDatabase == null) return;
            LetterData currentData = _alphabetDatabase.GetLetterData(_currentIndex);

            if (AudioManager.Instance != null && currentData.ExampleWordAudio != null)
            {
                AudioManager.Instance.PlaySFX(currentData.ExampleWordAudio);
            }
        }
    }
}
