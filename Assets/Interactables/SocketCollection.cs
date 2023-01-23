using System.Collections.Generic;
using Enumerations;
using GameModes;
using TouchScript.Gestures;
using UnityEngine;

namespace Interactables
{
    public class SocketCollection : MonoBehaviour
    {
        public List<Socket> Sockets = new List<Socket>();

        private void Awake()
        {
            DeactivateSockets();
        }

        private void Start()
        {
            GameModeSwitcher.OnSwitchedGameMode.AddListener(OnSwitchedGameModes);
        }

        private void OnSwitchedGameModes(GameMode gameMode)
        {
            Debug.Log(gameMode);
            switch (gameMode)
            {
                case GameMode.Deco:
                    ActivateSockets();
                    break;
                case GameMode.Terrain:
                    break;
                default:
                    DeactivateSockets();
                    break;
            }
        }

        private void ActivateSockets()
        {
            foreach (var socket in Sockets)
            {
                socket.GetComponent<PressGesture>().enabled = true;
                socket.GetComponent<SpriteRenderer>().enabled = true;
                if (socket.IsUsed) socket.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }

        private void DeactivateSockets()
        {
            foreach (var socket in Sockets)
            {
                socket.GetComponent<PressGesture>().enabled = false;
                socket.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }
}