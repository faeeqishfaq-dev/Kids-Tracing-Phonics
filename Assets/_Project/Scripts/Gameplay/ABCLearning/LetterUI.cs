using UnityEngine;
using UnityEngine.UI;

namespace Game.Gameplay.ABCLearning
{
    public class LetterUI : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private LetterManager _letterManager;

        [Header("UI Elements")]
        [SerializeField] private Image _uppercaseImage;
        [SerializeField] private Image _lowercaseImage;
        [SerializeField] private Text _exampleWordText;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _prevButton;
        
        [Header("Animated Object")]
        [SerializeField] private Transform _animatedObjectContainer;
        private GameObject _currentAnimatedObject;

        private void OnEnable()
        {
            if (_letterManager != null)
            {
                _letterManager.OnLetterChanged += HandleLetterChanged;
                _letterManager.OnCanNavigateNext += HandleCanNavigateNext;
                _letterManager.OnCanNavigatePrevious += HandleCanNavigatePrevious;
            }
        }

        private void OnDisable()
        {
            if (_letterManager != null)
            {
                _letterManager.OnLetterChanged -= HandleLetterChanged;
                _letterManager.OnCanNavigateNext -= HandleCanNavigateNext;
                _letterManager.OnCanNavigatePrevious -= HandleCanNavigatePrevious;
            }
        }

        private void HandleLetterChanged(LetterData data)
        {
            // Update Text / Sprites
            if (_uppercaseImage != null)
            {
                _uppercaseImage.sprite = data.UppercaseSprite;
                _uppercaseImage.gameObject.SetActive(data.UppercaseSprite != null);
            }

            if (_lowercaseImage != null)
            {
                _lowercaseImage.sprite = data.LowercaseSprite;
                _lowercaseImage.gameObject.SetActive(data.LowercaseSprite != null);
            }

            if (_exampleWordText != null)
            {
                _exampleWordText.text = data.ExampleWord;
            }

            // Spawn Animated Object
            SpawnAnimatedObject(data.AnimatedObjectPrefab);
        }

        private void SpawnAnimatedObject(GameObject prefab)
        {
            // Clear previous
            if (_currentAnimatedObject != null)
            {
                Destroy(_currentAnimatedObject);
            }

            // Spawn new
            if (prefab != null && _animatedObjectContainer != null)
            {
                _currentAnimatedObject = Instantiate(prefab, _animatedObjectContainer);
                _currentAnimatedObject.transform.localPosition = Vector3.zero;
                
                // If it's a UI element, reset scale and rect
                RectTransform rt = _currentAnimatedObject.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchoredPosition = Vector2.zero;
                    rt.localScale = Vector3.one;
                }
            }
        }

        private void HandleCanNavigateNext(bool canNavigate)
        {
            if (_nextButton != null) _nextButton.interactable = canNavigate;
        }

        private void HandleCanNavigatePrevious(bool canNavigate)
        {
            if (_prevButton != null) _prevButton.interactable = canNavigate;
        }

        // Methods to link to UI Buttons
        public void OnNextClicked()
        {
            if (_letterManager != null) _letterManager.NextLetter();
        }

        public void OnPreviousClicked()
        {
            if (_letterManager != null) _letterManager.PreviousLetter();
        }
        
        public void OnAudioClicked()
        {
            if (_letterManager != null) _letterManager.PlayExampleWordAudio();
        }
    }
}
