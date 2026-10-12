using UnityEngine;
using UnityEngine.UI;

namespace Game.Gameplay.FillLetter
{
    public class BrushSettingsUI : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private BrushPainter _brushPainter;
        [SerializeField] private FillLetterManager _fillManager;

        [Header("UI Feedback")]
        [SerializeField] private Slider _fillProgressSlider;
        [SerializeField] private GameObject _successPanel;

        private void Awake()
        {
            if (_successPanel != null) _successPanel.SetActive(false);
        }

        private void OnEnable()
        {
            if (_fillManager != null)
            {
                _fillManager.OnFillPercentageUpdated += UpdateProgress;
                _fillManager.OnLetterCompleted += ShowSuccess;
            }
        }

        private void OnDisable()
        {
            if (_fillManager != null)
            {
                _fillManager.OnFillPercentageUpdated -= UpdateProgress;
                _fillManager.OnLetterCompleted -= ShowSuccess;
            }
        }

        // Methods to be called via Unity UI Buttons
        public void SetColorRed() { SetColor(Color.red); }
        public void SetColorBlue() { SetColor(Color.blue); }
        public void SetColorGreen() { SetColor(Color.green); }

        public void SetColor(Color color)
        {
            if (_brushPainter != null)
            {
                _brushPainter.IsEraser = false;
                _brushPainter.CurrentColor = color;
            }
        }

        // Methods to be called via Unity UI Slider
        public void SetBrushSize(float size)
        {
            if (_brushPainter != null)
            {
                _brushPainter.CurrentBrushSize = size;
            }
        }

        public void ToggleEraser()
        {
            if (_brushPainter != null)
            {
                _brushPainter.IsEraser = !_brushPainter.IsEraser;
            }
        }

        public void ActionUndo()
        {
            if (_brushPainter != null) _brushPainter.Undo();
        }

        public void ActionRedo()
        {
            if (_brushPainter != null) _brushPainter.Redo();
        }

        public void ActionClear()
        {
            if (_brushPainter != null) _brushPainter.ClearAll();
        }

        private void UpdateProgress(float percentage)
        {
            if (_fillProgressSlider != null)
            {
                _fillProgressSlider.value = percentage;
            }
        }

        private void ShowSuccess()
        {
            if (_successPanel != null)
            {
                _successPanel.SetActive(true);
            }
        }
    }
}
