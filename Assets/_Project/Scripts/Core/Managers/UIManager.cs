using UnityEngine;
using Game.Core.Events;

namespace Game.Core.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Panels")]
        [SerializeField] private GameObject _hudPanel;
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private GameObject _loadingPanel; // For scene transitions

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // Making it global so it persists or you can change to scene-specific
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            GameEvents.OnSceneLoading += ShowLoadingPanel;
            GameEvents.OnSceneLoaded += HideLoadingPanel;
        }

        private void OnDisable()
        {
            GameEvents.OnSceneLoading -= ShowLoadingPanel;
            GameEvents.OnSceneLoaded -= HideLoadingPanel;
        }

        public void ShowSettings()
        {
            if (_settingsPanel != null) _settingsPanel.SetActive(true);
        }

        public void HideSettings()
        {
            if (_settingsPanel != null) _settingsPanel.SetActive(false);
        }

        private void ShowLoadingPanel(string sceneName)
        {
            if (_loadingPanel != null) _loadingPanel.SetActive(true);
        }

        private void HideLoadingPanel(string sceneName)
        {
            if (_loadingPanel != null) _loadingPanel.SetActive(false);
        }
    }
}
