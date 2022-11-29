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

        [SerializeField] private GameObject WithdrawButton;
    
        void Start()
        {
            SaveGame.LoadedPlayerData.AddListener(()=> StartCoroutine(OnLoadedGame()));

            SelectionManager.SELECT_OBJECT_EVENT.AddListener(EnableWithdraw);
            SelectionManager.DESELECT_OBJECT_EVENT.AddListener(DisableWithdraw);
            SelectionManager.WITHDRAW_OBJECT_EVENT.AddListener(DisableWithdraw);
        }

        private void EnableWithdraw(Interactable interactable)
        {
            WithdrawButton.SetActive(true);
        }
        
        private void DisableWithdraw()
        {
            WithdrawButton.SetActive(false);
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
