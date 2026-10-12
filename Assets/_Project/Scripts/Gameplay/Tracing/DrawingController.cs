using UnityEngine;
using Game.Core.Managers;
using System.Collections.Generic;
using System;

namespace Game.Gameplay.Tracing
{
    [RequireComponent(typeof(LineRenderer))]
    public class DrawingController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _minDistanceBetweenPoints = 0.1f;
        
        private LineRenderer _lineRenderer;
        private List<Vector3> _currentPoints = new List<Vector3>();
        private Camera _mainCamera;
        
        public Action<Vector3> OnPointAdded;
        public Action OnDrawEnd;
        
        private bool _isDrawing = false;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 0;
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnDragStart += HandleDragStart;
                InputManager.Instance.OnDrag += HandleDrag;
                InputManager.Instance.OnDragEnd += HandleDragEnd;
            }
        }

        private void OnDisable()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnDragStart -= HandleDragStart;
                InputManager.Instance.OnDrag -= HandleDrag;
                InputManager.Instance.OnDragEnd -= HandleDragEnd;
            }
        }

        public void SetDrawingEnabled(bool enabled)
        {
            _isDrawing = enabled;
            if (!enabled)
            {
                ClearLine();
            }
        }

        public void ClearLine()
        {
            _currentPoints.Clear();
            _lineRenderer.positionCount = 0;
        }

        private void HandleDragStart(Vector2 screenPosition)
        {
            if (!_isDrawing) return;
            
            ClearLine();
            AddPoint(screenPosition);
        }

        private void HandleDrag(Vector2 screenPosition)
        {
            if (!_isDrawing) return;
            
            Vector3 worldPos = GetWorldPosition(screenPosition);
            if (_currentPoints.Count > 0)
            {
                float distance = Vector3.Distance(_currentPoints[_currentPoints.Count - 1], worldPos);
                if (distance >= _minDistanceBetweenPoints)
                {
                    AddPoint(screenPosition);
                }
            }
            else
            {
                AddPoint(screenPosition);
            }
        }

        private void HandleDragEnd(Vector2 screenPosition)
        {
            if (!_isDrawing) return;
            
            OnDrawEnd?.Invoke();
        }

        private void AddPoint(Vector2 screenPosition)
        {
            Vector3 worldPos = GetWorldPosition(screenPosition);
            _currentPoints.Add(worldPos);
            _lineRenderer.positionCount = _currentPoints.Count;
            _lineRenderer.SetPosition(_currentPoints.Count - 1, worldPos);
            
            OnPointAdded?.Invoke(worldPos);
        }

        private Vector3 GetWorldPosition(Vector2 screenPosition)
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
            if (_mainCamera == null) return Vector3.zero;

            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));
            worldPos.z = 0f;
            return worldPos;
        }
    }
}
