using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Core.Events;

namespace Game.Core.Managers
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadScene(string sceneName)
        {
            CheckCpAd.instance.CallAd();
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            GameEvents.OnSceneLoading?.Invoke(sceneName);

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                // Progress can be clamped and sent to UI here if needed
                // float progress = Mathf.Clamp01(operation.progress / 0.9f);
                
                if (operation.progress >= 0.9f)
                {
                    // Allow activation or wait for UI transition
                    operation.allowSceneActivation = true;
                }
                
                yield return null;
            }

            GameEvents.OnSceneLoaded?.Invoke(sceneName);
        }
    }
}
