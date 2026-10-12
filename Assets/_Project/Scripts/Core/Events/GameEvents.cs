using System;

namespace Game.Core.Events
{
    public static class GameEvents
    {
        // General Game Flow
        public static Action OnGameInitialized;
        public static Action OnGameStart;
        public static Action OnGamePause;
        public static Action OnGameResume;
        public static Action OnGameOver;

        // Scene Management
        public static Action<string> OnSceneLoading;
        public static Action<string> OnSceneLoaded;

        // UI & Settings
        public static Action<float> OnMusicVolumeChanged;
        public static Action<float> OnSFXVolumeChanged;

        // Add modular events here as needed
    }
}
