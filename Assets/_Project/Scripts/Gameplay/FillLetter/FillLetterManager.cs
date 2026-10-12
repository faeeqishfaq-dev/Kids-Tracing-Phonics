using UnityEngine;
using System;
using Game.Core.Managers;

namespace Game.Gameplay.FillLetter
{
    public class FillLetterManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private BrushPainter _brushPainter;
        [SerializeField] private PaintValidator _paintValidator;
        //[SerializeField] private FillLetterStrokeGuide _strokeGuide;

        [Header("Settings")]
        [SerializeField] private float _completionThreshold = 0.9f; // 90%

        // Local Events
        public Action<float> OnFillPercentageUpdated;
        public Action OnLetterCompleted;

        private bool _isCompleted = false;

        private void Start()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
            {
                InitializePainting();
            }
            else
            {
                // Fallback for editor testing
                InitializePainting();
            }
        }

        public void InitializePainting()
        {
            _isCompleted = false;

            if (_brushPainter != null)
            {
                _brushPainter.SetPaintingEnabled(true);

                // Prevent duplicate event subscription
                _brushPainter.OnStrokeCompleted -= CheckCompletion;
                _brushPainter.OnStrokeCompleted += CheckCompletion;
            }

            //if (_strokeGuide != null)
            //{
            //    _strokeGuide.StartGuideAnimation();
            //}

            OnFillPercentageUpdated?.Invoke(0f);
        }

        private void OnDestroy()
        {
            if (_brushPainter != null)
            {
                _brushPainter.OnStrokeCompleted -= CheckCompletion;
            }
        }

        private void CheckCompletion()
        {
            if (_isCompleted) return;

            //if (_strokeGuide != null)
            //{
            //    _strokeGuide.AdvanceToNextStroke();
            //}

            float fillPercentage = 0f;
            if (_paintValidator != null && _brushPainter != null)
            {
                fillPercentage = _paintValidator.CalculateFillPercentage(_brushPainter.GetActiveStrokes());
            }

            OnFillPercentageUpdated?.Invoke(fillPercentage);

            if (fillPercentage >= _completionThreshold)
            {
                CompleteLetter();
            }
        }

        private void CompleteLetter()
        {
            if (_isCompleted)
                return;

            _isCompleted = true;

            if (_brushPainter != null)
            {
                _brushPainter.SetPaintingEnabled(false);
                _brushPainter.OnStrokeCompleted -= CheckCompletion;
            }

            //if (_strokeGuide != null)
            //{
            //    _strokeGuide.StopGuideAnimation();
            //}

            OnFillPercentageUpdated?.Invoke(1f);

            OnLetterCompleted?.Invoke();
        }

        public void ForceCompleteLetter()
        {
            if (!_isCompleted)
            {
                CompleteLetter();
            }
        }

    }
}
