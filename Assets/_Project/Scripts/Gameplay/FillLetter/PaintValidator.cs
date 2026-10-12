using UnityEngine;
using System.Collections.Generic;

namespace Game.Gameplay.FillLetter
{
    public class PaintValidator : MonoBehaviour
    {
        [SerializeField]
        private Transform strokePointsContainer;
        [Header("Validation")]
        [SerializeField] private float targetFillLength = 250f;

        [SerializeField] private int minimumStrokeCount = 8;

        [SerializeField] private int minimumPointsPerStroke = 10;
        private Transform currentGuideRoot;

        public void SetValidationData(Transform guideRoot)
        {
            currentGuideRoot = guideRoot;
        }
        public float CalculateFillPercentage(List<LineRenderer> activeStrokes)
        {
            if (activeStrokes == null || activeStrokes.Count == 0)
                return 0f;

            if (activeStrokes.Count < minimumStrokeCount)
                return 0f;

            float totalLength = 0f;

            foreach (LineRenderer stroke in activeStrokes)
            {
                if (stroke == null)
                    continue;

                if (!stroke.gameObject.activeSelf)
                    continue;

                if (stroke.startColor == Color.clear)
                    continue;

                if (stroke.positionCount < minimumPointsPerStroke)
                    continue;

                Vector3 previous = stroke.GetPosition(0);

                for (int i = 1; i < stroke.positionCount; i++)
                {
                    Vector3 current = stroke.GetPosition(i);

                    totalLength += Vector3.Distance(previous, current);

                    previous = current;
                }
            }

            float percentage = totalLength / targetFillLength;

            return Mathf.Clamp01(percentage);
        }
        public void SetGuide(Transform prefab)
        {
            strokePointsContainer = prefab;
        }
    }

}