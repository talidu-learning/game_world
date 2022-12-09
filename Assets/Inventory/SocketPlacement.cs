using System;
using System.Linq;
using Interactables;
using ServerConnection;
using Shop;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory
{
    public class SocketPlacement : MonoBehaviour
    {
        [SerializeField] private ShopInventory shopInventory;

        public static StringUnityEvent PlaceItemOnSocket = new StringUnityEvent();

        public static UnityEvent WithdrawItemOnSocket = new UnityEvent();

        [SerializeField] private GameObject SocketItem;

        private Socket currentSocket;

        private void Awake()
        {
            WithdrawItemOnSocket.AddListener(OnWithdrawItem);
        }

        private void OnWithdrawItem()
        {
            LocalPlayerData.Instance.OnWithdrewItem(currentSocket.Uid,currentSocket.transform.parent.parent.GetComponent<ItemID>().uid, currentSocket.transform.GetSiblingIndex());
            currentSocket.Withdraw();
        }

        private void Start()
        {
            SelectionManager.SELECT_SOCKET_EVENT.AddListener(OnSelectedSocket);
            PlaceItemOnSocket.AddListener(OnPlaceItemOnSocket);
        }

        private void OnPlaceItemOnSocket(string itemId)
        {
            if (!LocalPlayerData.Instance.IsItemPlaceable(itemId)) return;
            int uid = LocalPlayerData.Instance.GetUIDOfUnplacedItem(itemId);
            var go = CreateSocketItem(itemId, uid);

            currentSocket.Place(uid);

            LocalPlayerData.Instance.OnPlacedItem(uid, go.transform.position.x, go.transform.position.z,
                currentSocket.transform.parent.parent.GetComponent<ItemID>().uid,
                currentSocket.transform.parent.childCount, currentSocket.transform.GetSiblingIndex());
            
            SelectionManager.DESELECT_SOCKET_EVENT.Invoke(currentSocket);
        }

        private GameObject CreateSocketItem(string itemId, int uid)
        {
            var socketItem = Instantiate(SocketItem, currentSocket.gameObject.transform, false);

            var component = socketItem.AddComponent<ItemID>();
            component.id = itemId;
            component.uid = uid;
            component.ItemAttributes = shopInventory.ShopItems.FirstOrDefault(i => i.ItemID == itemId)?.Attributes;

            var spriteRenderer = socketItem.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = shopInventory.ShopItems.FirstOrDefault(i => i.ItemID == itemId)?.ItemSprite;

            return socketItem;
        }

        private void OnSelectedSocket(Socket socket)
        {
            currentSocket = socket;
        }
    }
}