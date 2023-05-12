using System.Collections;
using System.Collections.Generic;
using Enumerations;
using GameModes;
using Interactables;
using ServerConnection;
using Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Unplaced;
        [SerializeField] private Image ItemImage;

        public List<ItemAttribute> attributes { private set; get; }

        public string ItemID { private set; get; }

        public void Awake()
        {
            GetComponent<Button>().onClick.AddListener(PlaceItem);
        }

        private void Start()
        {
            SelectionManager.DESELECT_OBJECT_EVENT.AddListener(StartAsyncUpdate);
            SelectionManager.DELETE_OBJECT_EVENT.AddListener(StartAsyncUpdate);

            SelectionManager.SELECT_SOCKET_EVENT.AddListener(OnSelectedSocket);
            SelectionManager.DESELECT_SOCKET_EVENT.AddListener(OnDeselectedSocket);
            SelectionManager.DELETE_SOCKET_EVENT.AddListener(OnDeselectedSocket);
            
            GameModeSwitcher.OnSwitchedGameMode.AddListener(OnSwitchedGameModes);
        }

        private void OnSwitchedGameModes(GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.Deco:
                    OnSelectedSocket();
                    break;
                case GameMode.Default:
                    OnDeselectedSocket();
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
                    OnDeselectedSocket();
                    break;
            }
        }

        private void OnDeselectedSocket(Socket socket)
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(PlaceItem);
        }

        private void OnDeselectedSocket()
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(PlaceItem);
        }

        private void OnSelectedSocket(Socket socket)
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(PlaceOnSocket);
        }

        private void OnSelectedSocket()
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(PlaceOnSocket);
        }

        private void StartAsyncUpdate()
        {
            gameObject.SetActive(true);
            StartCoroutine(AsyncUpdate());
        }

        private IEnumerator AsyncUpdate()
        {
            yield return null;
            UpdateUI();
        }

        private void PlaceItem()
        {
            var uitemID = LocalPlayerData.Instance.GetUidOfUnplacedItem(ItemID);
            ShopManager.InitilizePlaceObjectEvent.Invoke(ItemID, uitemID);
            GameModeSwitcher.SwitchGameMode.Invoke(GameMode.Placing);
        }

        private void PlaceOnSocket()
        {
            GameModeSwitcher.SwitchGameMode.Invoke(GameMode.Deco);
            SocketPlacement.PlaceItemOnSocket.Invoke(ItemID);
        }

        public void Initialize(ShopItemData shopItemData)
        {
            ItemID = shopItemData.ItemID;
            if (shopItemData.ItemSprite != null ||
                shopItemData.Prefab.GetComponentInChildren<SpriteRenderer>().sprite != null)
            {
                ItemImage.sprite = !shopItemData.ItemSprite ? shopItemData.Prefab.GetComponentInChildren<SpriteRenderer>().sprite : shopItemData.ItemSprite;
            }
            attributes = shopItemData.Attributes;
            UpdateUI();
        }

        public void UpdateUI()
        {
            int unplaced = LocalPlayerData.Instance.GetCountOfUnplacedItems(ItemID);

            Unplaced.text = unplaced.ToString();

            if (unplaced == 0) gameObject.SetActive(false);
            
        }
    }
}