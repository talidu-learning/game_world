using System;
using TouchScript.Gestures;
using UnityEngine;

namespace Interactables
{
    public class Socket : MonoBehaviour
    {
        public Socket Neighbour;
        public bool IsUsed;
        public int Uid;

        private void Awake()
        {
            GetComponent<PressGesture>().Pressed+=OnTap;
        }

        private void OnTap(object sender, EventArgs e)
        {
            SelectionManager.SELECT_SOCKET_EVENT.Invoke(this);
        }

        public void Select()
        {
            GetComponent<SpriteRenderer>().color = Color.green;
        }

        public void Deselect()
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        public void Withdraw()
        {
            Destroy(transform.GetChild(0).gameObject);
            IsUsed = false;
        }
        
    }
}