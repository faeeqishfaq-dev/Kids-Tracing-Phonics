using UnityEngine;
using System;
using System.Collections.Generic;
using Game.Core.Managers;

namespace Game.Gameplay.Tracing
{
    [Serializable]
    public class StrokeData
    {
        public List<Transform> PathPoints;
        public Sprite GuideSprite;
    }

    public class TracingManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private DrawingController _drawingController;
        [SerializeField] private StrokeValidator _strokeValidator;
        
        [Header("Level Data")]
        [SerializeField] private List<StrokeData> _letterStrokes;
        
        private int _currentStrokeIndex = 0;
        private int _failures = 0;

        // Local Events
        public Action<int, int> OnProgressUpdated; // current, total
        public Action OnWrongPath;
        public Action OnStrokeCompleted;
        public Action<int> OnLetterCompleted; // passes stars earned

        private void Start()
        {
            // Auto initialize if we are in playing state, or call InitializeTracing manually
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
            {
                InitializeTracing();
            }
            else
            {
                // Fallback for isolated scene testing
                InitializeTracing();
            }
        }

        public void InitializeTracing()
        {
            if (_letterStrokes == null || _letterStrokes.Count == 0)
            {
                Debug.LogWarning("[TracingManager] No strokes defined for this letter.");
                return;
            }

            _currentStrokeIndex = 0;
            _failures = 0;
            
            if (_drawingController != null)
            {
                _drawingController.SetDrawingEnabled(true);
                _drawingController.OnPointAdded += HandlePointAdded;
                _drawingController.OnDrawEnd += HandleDrawEnd;
            }
            
            LoadStroke(_currentStrokeIndex);
            OnProgressUpdated?.Invoke(_currentStrokeIndex, _letterStrokes.Count);
        }

        private void OnDestroy()
        {
            if (_drawingController != null)
            {
                _drawingController.OnPointAdded -= HandlePointAdded;
                _drawingController.OnDrawEnd -= HandleDrawEnd;
            }
        }

        private void LoadStroke(int index)
        {
            if (index < _letterStrokes.Count)
            {
                _strokeValidator.SetupStroke(_letterStrokes[index].PathPoints);
            }
        }

        private void HandlePointAdded(Vector3 point)
        {
            bool isValid = _strokeValidator.ValidatePoint(point);
            
            if (!isValid)
            {
                _failures++;
                OnWrongPath?.Invoke();
                
                if (_drawingController != null)
                {
                    _drawingController.ClearLine();
                }
                
                // Force user to restart the current stroke
                LoadStroke(_currentStrokeIndex);
            }
        }

        private void HandleDrawEnd()
        {
            if (_strokeValidator.IsStrokeComplete())
            {
                _currentStrokeIndex++;
                OnStrokeCompleted?.Invoke();
                OnProgressUpdated?.Invoke(_currentStrokeIndex, _letterStrokes.Count);
                
                // Optional: keep the successfully drawn line visible by detaching or generating a permanent stroke visual
                if (_drawingController != null)
                {
                    _drawingController.ClearLine();
                }

                if (_currentStrokeIndex >= _letterStrokes.Count)
                {
                    CompleteLetter();
                }
                else
                {
                    LoadStroke(_currentStrokeIndex);
                }
            }
            else
            {
                // They lifted finger before completing the entire stroke
                if (_drawingController != null)
                {
                    _drawingController.ClearLine();
                }
                LoadStroke(_currentStrokeIndex);
            }
        }

        private void CompleteLetter()
        {
            if (_drawingController != null)
            {
                _drawingController.SetDrawingEnabled(false);
            }
            
            int stars = 3;
            if (_failures > 2) stars = 1;
            else if (_failures > 0) stars = 2;
            
            OnLetterCompleted?.Invoke(stars);
        }
    }
}
