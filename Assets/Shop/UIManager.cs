using System.Collections;
using System.Collections.Generic;
using Enumerations;
using Interactables;
using ServerConnection;
using UnityEngine;
using UnityEngine.Events;

namespace Shop
{
    public class FilterEvent : UnityEvent<UIType, List<ItemAttribute>, bool>{}
    
    public class UIManager : MonoBehaviour
    {
        public readonly static FilterEvent FILTER_EVENT = new FilterEvent();
        
        
        [SerializeField]private Animation LoadingAnimation;

        [SerializeField] private GameObject DeleteButton;
    
        void Start()
        {
            SaveGame.LoadedPlayerData.AddListener(()=> StartCoroutine(OnLoadedGame()));

            SelectionManager.SELECT_OBJECT_EVENT.AddListener(EnableDelete);
            SelectionManager.SELECT_SOCKET_EVENT.AddListener(OnSelectedSocket);
            SelectionManager.DESELECT_SOCKET_EVENT.AddListener(OnDeselectedSocket);
            SelectionManager.DESELECT_OBJECT_EVENT.AddListener(DisableDelete);
            SelectionManager.DELETE_OBJECT_EVENT.AddListener(DisableDelete);
            SelectionManager.DELETE_SOCKET_EVENT.AddListener(OnDeleteSocketItem);
        }

        private void OnDeleteSocketItem(Socket arg0)
        {
            DisableDelete();
        }

        private void OnDeselectedSocket(Socket arg0)
        {
            DisableDelete();
        }

        private void OnSelectedSocket(Socket socket)
        {
            if(socket.IsUsed) EnableDelete(null);
        }

        private void EnableDelete(Interactable interactable)
        {
            DeleteButton.SetActive(true);
        }
        
        private void DisableDelete()
        {
            DeleteButton.SetActive(false);
        }

        private IEnumerator OnLoadedGame()
        {
            LoadingAnimation.Play();

            yield return null;

            yield return new WaitWhile(() => LoadingAnimation.isPlaying);
        
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        
            LoadingAnimation.gameObject.SetActive(false);
        }
    }
}
