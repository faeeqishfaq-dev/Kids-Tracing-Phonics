using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Core.Managers;
using System;

namespace Game.Gameplay.FillLetter
{
    public class BrushPainter : MonoBehaviour
    {
        [Header("Brush Settings")]
        [SerializeField] 
        private Material _brushMaterial; 
        [SerializeField] 
        private Transform _strokesContainer; 
        [SerializeField] 
        private float _minPointDistance = 0.05f;

        public Color CurrentColor { get; set; } = Color.red;
        public float CurrentBrushSize { get; set; } = 0.2f;
        public bool IsEraser { get; set; } = false;

        public RectTransform drawArea;

        private Image currentLetterImage;
        private Texture2D currentTexture;
        private Sprite currentSprite;
        private List<LineRenderer> _undoStack = new List<LineRenderer>();
        private List<LineRenderer> _redoStack = new List<LineRenderer>();

        private LineRenderer _currentStroke;
        private List<Vector3> _currentPoints = new List<Vector3>();
        private Camera _mainCamera;
        private bool _isPainting = false;

        public Action<Vector3> OnPointAdded;
        public Action OnDragEnded;
        public Action OnStrokeCompleted;

        [Header("Paint Sound")]
        [SerializeField] private AudioSource _paintAudioSource;
        [SerializeField] private AudioClip _paintClip;

        private void Awake()
        {
            _mainCamera = Camera.main;

            if (_paintAudioSource != null)
            {
                _paintAudioSource.playOnAwake = false;
                _paintAudioSource.loop = true;
                _paintAudioSource.clip = _paintClip;
            }
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

        public void SetPaintingEnabled(bool enabled)
        {
            _isPainting = enabled;

            if (!enabled)
                StopPaintSound();
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var stroke = _undoStack[_undoStack.Count - 1];
                _undoStack.RemoveAt(_undoStack.Count - 1);
                stroke.gameObject.SetActive(false);
                _redoStack.Add(stroke);
                OnStrokeCompleted?.Invoke();
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var stroke = _redoStack[_redoStack.Count - 1];
                _redoStack.RemoveAt(_redoStack.Count - 1);
                stroke.gameObject.SetActive(true);
                _undoStack.Add(stroke);
                OnStrokeCompleted?.Invoke();
            }
        }

        public void ClearAll()
        {
            foreach (var stroke in _undoStack) Destroy(stroke.gameObject);
            foreach (var stroke in _redoStack) Destroy(stroke.gameObject);
            _undoStack.Clear();
            _redoStack.Clear();
            OnStrokeCompleted?.Invoke();
            StopPaintSound();
        }

        public List<LineRenderer> GetActiveStrokes()
        {
            return _undoStack;
        }

        private void HandleDragStart(Vector2 screenPosition)
        {
            if (!_isPainting) return;
            //if (!IsWithinBounds(screenPosition)) return;
            if (!IsWithinBounds(screenPosition))
            {
                StopPaintSound();
                return;
            }

            // Clear redo stack on new action
            foreach (var stroke in _redoStack) Destroy(stroke.gameObject);
            _redoStack.Clear();

            CreateNewStroke();
            AddPoint(screenPosition);
            PlayPaintSound();
        }

        private void HandleDrag(Vector2 screenPosition)
        {
            if (!_isPainting || _currentStroke == null) return;
            //if (!IsWithinBounds(screenPosition)) return;
            //if (!IsWithinBounds(screenPosition))
            //{
            //    StopPaintSound();
            //    return;
            //}
            if (!IsWithinBounds(screenPosition))
            {
                StopPaintSound();

                if (_currentStroke != null)
                {
                    _undoStack.Add(_currentStroke);
                    _currentStroke = null;
                    OnStrokeCompleted?.Invoke();
                }

                return;
            }

            Vector3 worldPos = GetWorldPosition(screenPosition);
            if (_currentPoints.Count > 0)
            {
                if (Vector3.Distance(_currentPoints[_currentPoints.Count - 1], worldPos) >= _minPointDistance)
                {
                    AddPoint(screenPosition);
                }
            }
        }

        private void HandleDragEnd(Vector2 screenPosition)
        {
            if (!_isPainting || _currentStroke == null) return;

            _undoStack.Add(_currentStroke);
            _currentStroke = null;

            OnDragEnded?.Invoke();
            OnStrokeCompleted?.Invoke();
            StopPaintSound();
        }

        public void CancelCurrentStroke()
        {
            if (_currentStroke != null)
            {
                Destroy(_currentStroke.gameObject);
                _currentStroke = null;
                StopPaintSound();
            }
        }

        private void CreateNewStroke()
        {
            GameObject strokeGO = new GameObject($"Stroke_{_undoStack.Count}");
            strokeGO.transform.SetParent(_strokesContainer != null ? _strokesContainer : transform, false);
            strokeGO.transform.localPosition = Vector3.zero;
            strokeGO.transform.localScale = Vector3.one;

            _currentStroke = strokeGO.AddComponent<LineRenderer>();
            _currentStroke.material = _brushMaterial;
            _currentStroke.positionCount = 0;
            _currentStroke.startWidth = CurrentBrushSize;
            _currentStroke.endWidth = CurrentBrushSize;
            _currentStroke.numCapVertices = 5;
            _currentStroke.numCornerVertices = 5;
            _currentStroke.useWorldSpace = true;
            _currentStroke.sortingOrder = 100;

            // Try to assign the custom mask-aware shader at runtime
            _currentStroke.material = _brushMaterial;

            if (IsEraser)
            {
                // Simplistic Eraser logic without custom shaders: draw transparent over background
                // Alternatively, relies on a special blend mode material assigned via _brushMaterial
                _currentStroke.startColor = Color.clear;
                _currentStroke.endColor = Color.clear;
            }
            else
            {
                _currentStroke.startColor = CurrentColor;
                _currentStroke.endColor = CurrentColor;
            }

            _currentPoints.Clear();
        }

        private void AddPoint(Vector2 screenPosition)
        {
            if (_currentStroke == null) return;

            Vector3 worldPos = GetWorldPosition(screenPosition);
            _currentPoints.Add(worldPos);
            _currentStroke.positionCount = _currentPoints.Count;
            _currentStroke.SetPosition(_currentPoints.Count - 1, worldPos);

            OnPointAdded?.Invoke(worldPos);
        }

        private Vector3 GetWorldPosition(Vector2 screenPosition)
        {
            if (_mainCamera == null) _mainCamera = Camera.main;

            RectTransform rt = GetComponent<RectTransform>();
            if (rt != null)
            {
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, screenPosition, _mainCamera, out Vector3 worldPos))
                {
                    worldPos += _mainCamera.transform.forward * 0.01f;
                    return worldPos;
                }
            }

            // Fallback
            Vector3 fallbackPos = _mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));
            fallbackPos.z = transform.position.z - 0.1f;
            return fallbackPos;
        }

        private bool IsWithinBounds(Vector2 screenPosition)
        {
            if (currentLetterImage == null)
                return false;

            RectTransform rect = currentLetterImage.rectTransform;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rect,
                    screenPosition,
                    _mainCamera,
                    out Vector2 localPoint))
            {
                return false;
            }

            Rect r = rect.rect;

            float normalizedX = (localPoint.x - r.x) / r.width;
            float normalizedY = (localPoint.y - r.y) / r.height;

            if (normalizedX < 0 || normalizedX > 1 ||
                normalizedY < 0 || normalizedY > 1)
                return false;

            if (currentSprite == null)
                return false;

            Texture2D tex = currentSprite.texture;

            if (tex == null)
                return false;

            Rect texRect = currentSprite.textureRect;

            int pixelX = Mathf.FloorToInt(texRect.x + normalizedX * texRect.width);
            int pixelY = Mathf.FloorToInt(texRect.y + normalizedY * texRect.height);

            if (pixelX < 0 ||
                pixelX >= tex.width ||
                pixelY < 0 ||
                pixelY >= tex.height)
                return false;

            Color pixel = tex.GetPixel(pixelX, pixelY);

            return pixel.a < 0.15f;
        }
        private void PlayPaintSound()
        {
            if (_paintAudioSource == null || _paintClip == null)
                return;

            if (!_paintAudioSource.isPlaying)
                _paintAudioSource.Play();
        }

        private void StopPaintSound()
        {
            if (_paintAudioSource == null)
                return;

            if (_paintAudioSource.isPlaying)
                _paintAudioSource.Stop();
        }
        public void SetCurrentLetter(Image image)
        {
            currentLetterImage = image;

            if (image == null)
                return;

            currentSprite = image.sprite;

            if (currentSprite != null)
                currentTexture = currentSprite.texture;
        }
    }

}