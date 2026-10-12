using UnityEngine;
using Game.Core.Events;

namespace Game.Core.Managers
{
    public enum GameState
    {
        Initializing,
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; }

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
                return;
            }
        }

        private void Start()
        {
            ChangeState(GameState.Initializing);
            GameEvents.OnGameInitialized?.Invoke();
        }

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;
            
            CurrentState = newState;
            
            switch (CurrentState)
            {
                case GameState.Playing:
                    GameEvents.OnGameStart?.Invoke();
                    break;
                case GameState.Paused:
                    GameEvents.OnGamePause?.Invoke();
                    break;
                case GameState.GameOver:
                    GameEvents.OnGameOver?.Invoke();
                    break;
            }
        }
    }
}
