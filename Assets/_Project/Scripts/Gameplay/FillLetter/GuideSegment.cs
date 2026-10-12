using UnityEngine;

namespace Game.Gameplay.FillLetter
{
    [RequireComponent(typeof(Collider2D))]
    public class GuideSegment : MonoBehaviour
    {
        [Header("Guide")]
        public int guideIndex;

        [Range(0f, 1f)]
        public float requiredCoverage = 0.90f;

        private bool completed;

        public bool IsCompleted => completed;

        public bool Contains(Vector3 worldPos)
        {
            Collider2D col = GetComponent<Collider2D>();

            if (col == null)
                return false;

            return col.OverlapPoint(worldPos);
        }

        public void Complete()
        {
            if (completed)
                return;

            completed = true;

            Debug.Log(name + " Completed");
        }

        public void ResetGuide()
        {
            completed = false;
        }
    }
}