using System.Collections.Generic;
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
            SelectionManager.EnableDecoration.AddListener(ActivateSockets);
            SelectionManager.DisableDecoration.AddListener(DeactivateSockets);
        }

        public void ActivateSockets(){
            foreach (var socket in Sockets)
            {
                socket.GetComponent<PressGesture>().enabled = true;
                socket.GetComponent<SpriteRenderer>().enabled = true;
                if (socket.IsUsed) socket.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
        
        public void DeactivateSockets(){
            foreach (var socket in Sockets)
            {
                socket.GetComponent<PressGesture>().enabled = false;
                socket.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        
    }
    
}