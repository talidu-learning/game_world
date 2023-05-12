using Enumerations;
using UnityEngine;
using UnityEngine.Events;

namespace GameModes
{
    public class GameModeUnityEvent : UnityEvent<GameMode>{}

    public class GameModeSwitcher : MonoBehaviour
    {
        public static GameMode currentGameMode = GameMode.Default;

        public static GameModeUnityEvent SwitchGameMode = new GameModeUnityEvent();
        public static GameModeUnityEvent OnSwitchedGameMode = new GameModeUnityEvent();

        private void Awake()
        {
            SwitchGameMode.AddListener(OnSwitchingGameModes);
        }

        private void OnSwitchingGameModes(GameMode newMode)
        {
            Debug.Log("gameMode: " + newMode);
            
            if(newMode != currentGameMode)
                OnSwitchedGameMode.Invoke(newMode);
            
            currentGameMode = newMode;
        }
    }
}