using UnityEngine;
using System.Collections.Generic;

namespace Game.Gameplay.Tracing
{
    public class StrokeValidator : MonoBehaviour
    {
        [Header("Validation Settings")]
        [SerializeField] private float _acceptableRadius = 1.0f;
        
        [Header("Current Stroke Data")]
        [SerializeField] private List<Transform> _expectedPathPoints = new List<Transform>();
        
        private int _currentTargetIndex = 0;

        public void SetupStroke(List<Transform> pathPoints)
        {
            _expectedPathPoints = pathPoints;
            _currentTargetIndex = 0;
        }

        public bool ValidatePoint(Vector3 drawnPoint)
        {
            if (_expectedPathPoints == null || _expectedPathPoints.Count == 0 || _currentTargetIndex >= _expectedPathPoints.Count)
            {
                return false;
            }

            Vector3 targetPosition = _expectedPathPoints[_currentTargetIndex].position;
            float distanceToCurrentNode = Vector3.Distance(drawnPoint, targetPosition);

            // Reached the current target node
            if (distanceToCurrentNode <= _acceptableRadius)
            {
                _currentTargetIndex++;
                return true; 
            }

            // If the user hasn't reached the node, check if they wandered too far off the path entirely
            if (distanceToCurrentNode > _acceptableRadius * 2f) 
            {
                // Wrong path! They are too far away from the target
                return false; 
            }

            // Valid point, still tracing towards target
            return true; 
        }

        public bool IsStrokeComplete()
        {
            return _currentTargetIndex >= _expectedPathPoints.Count;
        }
    }
}
