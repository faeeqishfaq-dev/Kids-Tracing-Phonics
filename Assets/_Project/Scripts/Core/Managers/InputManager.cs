using UnityEngine;
using System;

namespace Game.Core.Managers
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        // Modular events for input
        public Action<Vector2> OnTap;
        public Action<Vector2> OnDragStart;
        public Action<Vector2> OnDrag;
        public Action<Vector2> OnDragEnd;

        private bool _isDragging = false;
        private Vector2 _lastPointerPosition;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // typically input might be scene-specific or global, depending on design
                // Making it global for easier access
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            HandlePointerInput();
        }

        private void HandlePointerInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isDragging = true;
                _lastPointerPosition = Input.mousePosition;
                OnDragStart?.Invoke(_lastPointerPosition);
                OnTap?.Invoke(_lastPointerPosition);
            }
            else if (Input.GetMouseButton(0) && _isDragging)
            {
                Vector2 currentPosition = Input.mousePosition;
                if (currentPosition != _lastPointerPosition)
                {
                    OnDrag?.Invoke(currentPosition);
                    _lastPointerPosition = currentPosition;
                }
            }
            else if (Input.GetMouseButtonUp(0) && _isDragging)
            {
                _isDragging = false;
                OnDragEnd?.Invoke(Input.mousePosition);
            }
        }
    }
}
