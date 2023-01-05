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

        public static UnityEvent DeleteItemOnSocket = new UnityEvent();

        [SerializeField] private GameObject SocketItem;

        private Socket currentSocket;

        [SerializeField] private Game.ServerConnection ServerConnection;
        private Action<bool, string, Guid> serverCallbackPlacing;
        private Action<bool, Guid, Guid, int> serverCallbackDelete;

        private void Awake()
        {
            DeleteItemOnSocket.AddListener(OnDeleteItem);

            serverCallbackPlacing = ServerCallbackOnTriedPlacing;
            serverCallbackDelete = ServerCallbackOnTriedDeleting;
        }

        private void OnDeleteItem()
        {
            ServerConnection.OnDeletedItemOnSocket(currentSocket.Uid, currentSocket.transform.GetSiblingIndex(),
                currentSocket.transform.parent.parent.GetComponent<ItemID>().uid, serverCallbackDelete);
        }

        private void Start()
        {
            SelectionManager.SELECT_SOCKET_EVENT.AddListener(OnSelectedSocket);
            PlaceItemOnSocket.AddListener(OnPlaceItemOnSocket);
        }

        private void OnPlaceItemOnSocket(string itemId)
        {
            if (!LocalPlayerData.Instance.IsItemPlaceable(itemId)) return;
            Guid uid = LocalPlayerData.Instance.GetUIDOfUnplacedItem(itemId);

            ServerConnection.OnPlacedItemOnSocket(uid, currentSocket.transform.parent.childCount,
                currentSocket.transform.GetSiblingIndex(),
                currentSocket.transform.parent.parent.GetComponent<ItemID>().uid, itemId, serverCallbackPlacing);
        }

        private void ServerCallbackOnTriedDeleting(bool sucessfullyConnected, Guid socketItemUid,
            Guid socketcollectionuid, int siblingindex)
        {
            if (sucessfullyConnected)
                OnSuccessfulDeletion(socketItemUid, socketcollectionuid, siblingindex);
            else OnFailedDeletion();
        }

        private void OnSuccessfulDeletion(Guid socketItemUid, Guid uid, int siblingindex)
        {
            LocalPlayerData.Instance.OnDeletedItem(socketItemUid, uid, siblingindex);
            currentSocket.Delete();
        }

        private void OnFailedDeletion()
        {
            currentSocket.Delete();
        }

        private void ServerCallbackOnTriedPlacing(bool sucessfullyConnected, string itemID, Guid uid)
        {
            if (sucessfullyConnected)
                OnSuccessfulPlacement(itemID, uid);
            else OnFailedPlacement();
        }

        private void OnSuccessfulPlacement(string itemId, Guid uid)
        {
            var go = CreateSocketItem(itemId, uid);

            currentSocket.Place(uid);

            LocalPlayerData.Instance.OnPlacedItem(uid, go.transform.position.x, go.transform.position.z,
                currentSocket.transform.parent.parent.GetComponent<ItemID>().uid,
                currentSocket.transform.parent.childCount, currentSocket.transform.GetSiblingIndex());

            SelectionManager.DESELECT_SOCKET_EVENT.Invoke(currentSocket);
        }

        private void OnFailedPlacement()
        {
            SelectionManager.DESELECT_SOCKET_EVENT.Invoke(currentSocket);
        }

        private GameObject CreateSocketItem(string itemId, Guid uid)
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