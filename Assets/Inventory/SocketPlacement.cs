using System;
using System.Linq;
using Enumerations;
using GameModes;
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

        [SerializeField] private ServerConnection.ServerConnection ServerConnection;
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
            Transform parent = GetParentWithIDComponent();
            Debug.Log(currentSocket);
            ServerConnection.OnDeletedItemOnSocket(currentSocket.Uid, currentSocket.transform.GetSiblingIndex(),
                parent.GetComponent<ItemID>().uid, serverCallbackDelete);
        }

        private Transform GetParentWithIDComponent()
        {
            Transform parent;
            if (currentSocket.transform.parent.parent.parent)
                parent = currentSocket.transform.parent.parent.parent; // table has graphics pivot
            else parent = currentSocket.transform.parent.parent;
            return parent;
        }

        private void Start()
        {
            SelectionManager.SELECT_SOCKET_EVENT.AddListener(OnSelectedSocket);
            PlaceItemOnSocket.AddListener(OnPlaceItemOnSocket);
        }

        private void OnPlaceItemOnSocket(string itemId)
        {
            if (currentSocket == null) return;
            if (!LocalPlayerData.Instance.IsItemPlaceable(itemId)) return;
            Guid uid = LocalPlayerData.Instance.GetUidOfUnplacedItem(itemId);

            Transform parent = GetParentWithIDComponent();

            ServerConnection.OnPlacedItemOnSocket(uid, currentSocket.transform.parent.childCount,
                currentSocket.transform.GetSiblingIndex(),
                parent.GetComponent<ItemID>().uid, itemId, serverCallbackPlacing);
        }

        private void ServerCallbackOnTriedDeleting(bool sucessfullyConnected, Guid socketItemUid,
            Guid socketcollectionuid, int siblingindex)
        {
            if (sucessfullyConnected)
                OnSuccessfulDeletion(socketItemUid, socketcollectionuid, siblingindex);
            else OnFailedDeletion();
            GameModeSwitcher.SwitchGameMode.Invoke(GameMode.Deco);
        }

        private void OnSuccessfulDeletion(Guid socketItemUid, Guid uid, int siblingindex)
        {
            SelectionManager.DELETE_SOCKET_EVENT.Invoke(currentSocket);
            currentSocket.Delete();
            LocalPlayerData.Instance.OnDeletedItem(socketItemUid, uid, siblingindex);
            currentSocket = null;
        }

        private void OnFailedDeletion()
        {
            // currentSocket.Delete();
            currentSocket = null;
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

            var localScale = shopInventory.ShopItems.First(i => i.ItemID == itemId).Prefab.transform
                .GetChild(0).localScale;

            ScaleGameObjectForSocket(localScale, go);

            currentSocket.Place(uid);

            Transform parentWithIdComponent = GetParentWithIDComponent();

            LocalPlayerData.Instance.OnPlacedItem(uid, go.transform.position.x, go.transform.position.z,
                parentWithIdComponent.GetComponent<ItemID>().uid,
                currentSocket.transform.parent.childCount, currentSocket.transform.GetSiblingIndex());

            SelectionManager.DESELECT_SOCKET_EVENT.Invoke(currentSocket);
            currentSocket = null;
        }

        public static void ScaleGameObjectForSocket(Vector3 localScale, GameObject go)
        {
            go.transform.localScale = localScale;

            // go.transform.position += new Vector3(0, Mathf.Abs(localScale.y), 0);
            go.transform.localPosition = new Vector3(0, 0, 0);
        }

        private void OnFailedPlacement()
        {
            SelectionManager.DESELECT_SOCKET_EVENT.Invoke(currentSocket);
            currentSocket = null;
        }

        private GameObject CreateSocketItem(string itemId, Guid uid)
        {
            var socketItem = Instantiate(new GameObject(), currentSocket.gameObject.transform, false);

            var component = socketItem.AddComponent<ItemID>();
            component.id = itemId;
            component.uid = uid;
            component.ItemAttributes = shopInventory.ShopItems.FirstOrDefault(i => i.ItemID == itemId)?.Attributes;

            var spriteRenderer = socketItem.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = shopInventory.ShopItems.FirstOrDefault(i => i.ItemID == itemId)?.ItemSprite;
            socketItem.transform.position = Vector3.zero;
            return socketItem;
        }

        private void OnSelectedSocket(Socket socket)
        {
            currentSocket = socket;
        }
    }
}