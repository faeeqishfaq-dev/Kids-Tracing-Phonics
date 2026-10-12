using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.Gameplay.FillLetter
{
    public class FillLetterStrokeGuide : MonoBehaviour
    {
        [Header("Guide Settings")]
        public GameObject guidePrefab;
        public float speed = 2f;
        public float scale = 1f;
        public float rotation = 0f;
        
        [Header("Path Settings")]
        public Transform strokePointsContainer;

        private GameObject _currentGuide;
        private Coroutine _guideAnimationCoroutine;
        private List<Transform> _pathPoints = new List<Transform>();
        private int _currentStrokeIndex = 0;

        public void StartGuideAnimation()
        {
            StopGuideAnimation();
            
            // Populate path points dynamically if container is assigned
            if (strokePointsContainer != null)
            {
                _pathPoints.Clear();
                foreach (Transform child in strokePointsContainer)
                {
                    _pathPoints.Add(child);
                }
            }

            if (guidePrefab == null || _pathPoints == null || _pathPoints.Count == 0) return;

            if (_currentGuide == null)
            {
                _currentGuide = Instantiate(guidePrefab, transform);
            }

            _currentGuide.transform.localScale = Vector3.one * scale;
            _currentGuide.transform.localRotation = Quaternion.Euler(0, 0, rotation);
            _currentGuide.SetActive(true);

            _currentStrokeIndex = 0;
            _guideAnimationCoroutine = StartCoroutine(AnimateGuide(_pathPoints));
        }

        public void StopGuideAnimation()
        {
            if (_guideAnimationCoroutine != null)
            {
                StopCoroutine(_guideAnimationCoroutine);
                _guideAnimationCoroutine = null;
            }

            if (_currentGuide != null)
            {
                _currentGuide.SetActive(false);
            }
        }

        public void AdvanceToNextStroke()
        {
            // The prompt says ""After Stroke 1: Automatically move to Stroke 2""
            // In a Fill Letter context, we just move to the next point or restart if completed.
            _currentStrokeIndex++;
            if (_currentStrokeIndex >= _pathPoints.Count - 1)
            {
                // Completed all strokes, optionally hide or restart
                StopGuideAnimation();
            }
        }

        private IEnumerator AnimateGuide(List<Transform> pathPoints)
        {
            while (true) // Loop until externally stopped or advanced
            {
                if (pathPoints.Count < 2) yield break;

                // Animate from current stroke index to the next one
                int startIndex = _currentStrokeIndex;
                int endIndex = _currentStrokeIndex + 1;
                
                if (endIndex >= pathPoints.Count)
                {
                    // Loop back to start if we reached the end of the predefined points
                    startIndex = 0;
                    endIndex = 1;
                }

                Vector3 startPos = pathPoints[startIndex].position;
                Vector3 endPos = pathPoints[endIndex].position;

                _currentGuide.transform.position = startPos;
                
                float distance = Vector3.Distance(startPos, endPos);
                float duration = distance / speed;
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    _currentGuide.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                    yield return null;
                }

                // Snap to end
                _currentGuide.transform.position = endPos;
                
                // Small pause before looping the current stroke animation (to show user what to do)
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
