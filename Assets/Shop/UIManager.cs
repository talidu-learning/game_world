using System.Collections;
using System.Collections.Generic;
using Enumerations;
using Interactables;
using ServerConnection;
using UnityEngine;
using UnityEngine.Events;

namespace Shop
{
    public class FilterEvent : UnityEvent<UIType, List<ItemAttribute>, bool> { }
    public class ColorPickerEvent : UnityEvent<List<ItemVariant>, ShopItem> { }

    public class UIManager : MonoBehaviour
    {
        public static readonly FilterEvent FILTER_EVENT = new FilterEvent();

        public static readonly ColorPickerEvent ColorPickerEvent = new ColorPickerEvent();
        public static readonly UnityEvent CloseColorPickerEvent = new UnityEvent();
        
        [SerializeField] private Animation LoadingAnimation;

        [SerializeField] private GameObject DeleteButton;

        [SerializeField] private ColorPickerWindow ColorPickerMenu;
        [SerializeField] private GameObject ColorPickerGO;

        private void Start()
        {
            SaveGame.TEXT_LOADED_PLAYER_DATA.AddListener(() => StartCoroutine(OnLoadedGame()));

            SelectionManager.SELECT_OBJECT_EVENT.AddListener(EnableDelete);
            SelectionManager.SELECT_SOCKET_EVENT.AddListener(OnSelectedSocket);
            SelectionManager.DESELECT_SOCKET_EVENT.AddListener(OnDeselectedSocket);
            SelectionManager.DESELECT_OBJECT_EVENT.AddListener(DisableDelete);
            SelectionManager.DELETE_OBJECT_EVENT.AddListener(DisableDelete);
            SelectionManager.DELETE_SOCKET_EVENT.AddListener(OnDeleteSocketItem);
            
            ColorPickerEvent.AddListener(OnOpenColorPickerMenu);
            CloseColorPickerEvent.AddListener(OnCloseColorPicker);
            
            OnCloseColorPicker();
        }

        private void OnCloseColorPicker()
        {
            ColorPickerGO.SetActive(false);
        }

        private void OnOpenColorPickerMenu(List<ItemVariant> variants, ShopItem shopItem)
        {
            ColorPickerGO.SetActive(true);
            ColorPickerMenu.InitializeMenu(variants, shopItem);
        }

        private void OnDeleteSocketItem(Socket socket)
        {
            DisableDelete();
        }

        private void OnDeselectedSocket(Socket socket)
        {
            DisableDelete();
        }

        private void OnSelectedSocket(Socket socket)
        {
            if (socket.IsUsed) EnableDelete(null);
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