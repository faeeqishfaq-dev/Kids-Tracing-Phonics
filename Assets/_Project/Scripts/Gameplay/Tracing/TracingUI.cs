using UnityEngine;
using UnityEngine.UI;

namespace Game.Gameplay.Tracing
{
    public class TracingUI : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private TracingManager _tracingManager;
        
        [Header("UI Elements")]
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private GameObject _successPanel;
        [SerializeField] private GameObject[] _stars;

        private void Awake()
        {
            if (_successPanel != null)
            {
                _successPanel.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (_tracingManager != null)
            {
                _tracingManager.OnProgressUpdated += UpdateProgress;
                _tracingManager.OnLetterCompleted += ShowSuccess;
                _tracingManager.OnWrongPath += ShowErrorFeedback;
            }
        }

        private void OnDisable()
        {
            if (_tracingManager != null)
            {
                _tracingManager.OnProgressUpdated -= UpdateProgress;
                _tracingManager.OnLetterCompleted -= ShowSuccess;
                _tracingManager.OnWrongPath -= ShowErrorFeedback;
            }
        }

        private void UpdateProgress(int current, int total)
        {
            if (_progressSlider != null && total > 0)
            {
                _progressSlider.value = (float)current / total;
            }
        }

        private void ShowErrorFeedback()
        {
            // Could trigger a red flash animation or screen shake
            Debug.Log("[TracingUI] Wrong Path Detected! Displaying visual feedback.");
        }

        private void ShowSuccess(int starsEarned)
        {
            if (_successPanel != null)
            {
                _successPanel.SetActive(true);
                
                // Activate the correct number of stars
                for (int i = 0; i < _stars.Length; i++)
                {
                    if (_stars[i] != null)
                    {
                        _stars[i].SetActive(i < starsEarned);
                    }
                }
            }
        }
    }
}
