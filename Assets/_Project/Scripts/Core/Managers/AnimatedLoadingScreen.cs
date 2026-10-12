using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Game.Core.Managers;

namespace Game.UI
{
    public class AnimatedLoadingScreen : MonoBehaviour
    {
        [Header("UI Elements")]
        public RectTransform fillRect;
        public Text loadingText;
        
        [Header("Settings")]
        public float loadTime = 3f;
        
        private void Start()
        {
            StartCoroutine(LoadAnimation());
        }
        
        private IEnumerator LoadAnimation()
        {
            float timer = 0f;
            while(timer < loadTime)
            {
                timer += Time.deltaTime;
                float progress = timer / loadTime;
                
                if(fillRect != null)
                {
                    fillRect.anchorMax = new Vector2(progress, 1);
                }
                
                if(loadingText != null)
                {
                    int dots = Mathf.FloorToInt(timer * 3f) % 4;
                    loadingText.text = "Loading" + new string('.', dots);
                }
                
                yield return null;
            }
            
            // Ensure full before jumping
            if(fillRect != null) fillRect.anchorMax = new Vector2(1, 1);
            if(loadingText != null) loadingText.text = "Ready!";
            yield return new WaitForSeconds(0.5f);

            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadScene("1_Gameplay");
            }
        }
    }
}
