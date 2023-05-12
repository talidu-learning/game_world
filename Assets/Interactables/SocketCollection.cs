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
            switch (gameMode)
            {
                case GameMode.Deco:
                    ActivateSockets();
                    break;
                case GameMode.Default:
                    DeactivateSockets();
                    break;
                case GameMode.Placing:
                    break;
                case GameMode.Inventory:
                    break;
                case GameMode.DecoInventory:
                    break;
                case GameMode.Shop:
                    break;
                case GameMode.SelectedSocket:
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