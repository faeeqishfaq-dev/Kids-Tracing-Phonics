using UnityEngine;
using Game.Core.Events;

namespace Game.Core.Managers
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

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

        private void Start()
        {
            ApplySettings();
        }

        public void ApplySettings()
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData != null)
            {
                SetMusicVolume(SaveManager.Instance.CurrentSaveData.musicVolume, false);
                SetSFXVolume(SaveManager.Instance.CurrentSaveData.sfxVolume, false);
            }
        }

        public void SetMusicVolume(float volume, bool save = true)
        {
            GameEvents.OnMusicVolumeChanged?.Invoke(volume);
            
            if (save && SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData != null)
            {
                SaveManager.Instance.CurrentSaveData.musicVolume = volume;
                SaveManager.Instance.SaveGame();
            }
        }

        public void SetSFXVolume(float volume, bool save = true)
        {
            GameEvents.OnSFXVolumeChanged?.Invoke(volume);
            
            if (save && SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData != null)
            {
                SaveManager.Instance.CurrentSaveData.sfxVolume = volume;
                SaveManager.Instance.SaveGame();
            }
        }
    }
}
