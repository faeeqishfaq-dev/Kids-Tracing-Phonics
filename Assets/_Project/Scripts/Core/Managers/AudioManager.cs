using UnityEngine;
using Game.Core.Events;

namespace Game.Core.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

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

        private void OnEnable()
        {
            GameEvents.OnMusicVolumeChanged += SetMusicVolume;
            GameEvents.OnSFXVolumeChanged += SetSFXVolume;
        }

        private void OnDisable()
        {
            GameEvents.OnMusicVolumeChanged -= SetMusicVolume;
            GameEvents.OnSFXVolumeChanged -= SetSFXVolume;
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (_musicSource == null || clip == null) return;
            
            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.Play();
        }

        public void PlaySFX(AudioClip clip)
        {
            if (_sfxSource == null || clip == null) return;
            
            _sfxSource.PlayOneShot(clip);
        }

        private void SetMusicVolume(float volume)
        {
            if (_musicSource != null) _musicSource.volume = Mathf.Clamp01(volume);
        }

        private void SetSFXVolume(float volume)
        {
            if (_sfxSource != null) _sfxSource.volume = Mathf.Clamp01(volume);
        }
    }
}
