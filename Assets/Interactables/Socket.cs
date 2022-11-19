using TouchScript.Gestures;
using UnityEngine;

namespace Interactables
{
    public class Socket : MonoBehaviour
    {
        public bool IsUsed;
        public int Uid;

        private void Awake()
        {
            GetComponent<PressGesture>().OnPress.AddListener(OnTap);
        }

        private void OnTap(Gesture gesture)
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