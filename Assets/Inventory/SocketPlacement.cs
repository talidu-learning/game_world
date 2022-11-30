using System.Linq;
using Interactables;
using ServerConnection;
using Shop;
using UnityEngine;

namespace Inventory
{
    public class SocketPlacement : MonoBehaviour
    {
        [SerializeField] private ShopInventory shopInventory;

        public static StringUnityEvent PlaceItemOnSocket = new StringUnityEvent();

        [SerializeField] private GameObject SocketItem;

        private Socket currentSocket;

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

            // var attributes = go.GetComponent<ItemData>().Attributes;
            // if (attributes.Contains(ItemAttribute.TwoSocketDeco))
            // {
            //     currentSocket.Neighbour.Place(uid);
            //     go.transform.position += new Vector3(-0.5f, 0, 0); // place in between sockets
            // }
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