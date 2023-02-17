using UnityEngine;
using UnityEngine.Events;

namespace ServerConnection
{
    public class ServerConnectionErrorUI : MonoBehaviour
    {
        [SerializeField] private GameObject ServerConnectionErrorUIWrapper;
        [SerializeField] private Animation Animation;

        public static UnityEvent ServerErrorOccuredEvent = new UnityEvent();

        private void Awake()
        {
            ServerErrorOccuredEvent.AddListener(OnErrorOccured);
        }

        private void OnErrorOccured()
        {
            ServerConnectionErrorUIWrapper.SetActive(true);
            Animation.Play("Flicker");
        }
    }
}