using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shop;
using UnityEngine;

namespace ServerConnection
{
    public class LocalPlayerData : MonoBehaviour
    {
        private ServerConnection ServerConnection;

        public static StringUnityEvent ChangedItemDataEvent = new StringUnityEvent();

        private int _stars = 500;

        public List<ItemData> _ownedItems = new List<ItemData>();

        private static LocalPlayerData _instance;

        public static LocalPlayerData Instance
        {
            get { return _instance; }
        }


        private void Awake()
        {
            ServerConnection = FindObjectOfType<ServerConnection>();
            if (_instance != null && _instance != this)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        }

        public string GetJsonData()
        {
            return JsonUtility.ToJson(_ownedItems);
        }

        public void Initialize()
        {
            StarCountUI.UpdateStarCount.Invoke(_stars.ToString());
        }

        public ItemData[] GetOwnedItems()
        {
            return _ownedItems.ToArray();
        }

        public ItemData[] GetPlacedItems()
        {
            return _ownedItems.Where(o => o.x != 0 && o.z != 0 || o.isPlacedOnSocket == true).ToArray();
        }

        public int GetStarCount()
        {
            return _stars;
        }

        public void SetStarCount(int stars)
        {
            _stars = stars;
        }

        public async Task<bool> TryBuyItem(string id, int itemValue)
        {
            if (_stars - itemValue >= 0)
            {
                var updateStarCount = await ServerConnection.UpdateStarCount(_stars - itemValue);
                var newItem = await ServerConnection.CreateNewItemForCurrentPlayer(id);
                if (updateStarCount && newItem.id == id)
                {
                    _stars -= itemValue;
                    StarCountUI.UpdateStarCount.Invoke(_stars.ToString());
                    _ownedItems.Add(newItem);

                    ChangedItemDataEvent.Invoke(id);
                    return true;
                }

                if (updateStarCount) await ServerConnection.UpdateStarCount(_stars);
            }
            return false;
        }

        public void OnPlacedItem(Guid uid, float x, float z)
        {
            var item = _ownedItems.First(i => i.uid == uid);

            item.x = x;
            item.z = z;
            item.isPlacedOnSocket = false;

            ChangedItemDataEvent.Invoke(item.id);
        }

        public void OnPlacedItem(Guid uid, float x, float z, Guid socketcolectionuid, int socketcount, int socketindex)
        {
            var item = _ownedItems.First(i => i.uid == uid);

            var socketItem = _ownedItems.First(i => i.uid == socketcolectionuid);


            if (socketItem.itemsPlacedOnSockets == null) socketItem.itemsPlacedOnSockets = new Guid[socketcount];
            socketItem.itemsPlacedOnSockets[socketindex] = uid;

            item.x = x;
            item.z = z;
            item.isPlacedOnSocket = true;

            ChangedItemDataEvent.Invoke(item.id);
        }

        public void OnDeletedItem(Guid uid)
        {
            var item = _ownedItems.FirstOrDefault(o => o.uid == uid && o.x != 0 && o.z != 0);
            if (item == null) return;
            item.x = 0;
            item.z = 0;

            ChangedItemDataEvent.Invoke(item.id);
        }

        public void OnDeletedItem(Guid uid, Guid socketcollectionuid, int socketindex)
        {
            var item = _ownedItems.FirstOrDefault(o => o.uid == uid);

            var socketcollection = _ownedItems.First(o => o.uid == socketcollectionuid)
                .itemsPlacedOnSockets;

            socketcollection[socketindex] = Guid.Empty;
            item.x = 0;
            item.z = 0;
            item.isPlacedOnSocket = false;

            ChangedItemDataEvent.Invoke(item.id);
        }

        public bool IsItemPlaceable(string itemId)
        {
            return _ownedItems.Any(o => o.id == itemId && o.x == 0 && o.z == 0);
        }

        public int GetCountOfOwnedItems(string itemId)
        {
            return _ownedItems.Count(i => i.id == itemId);
        }

        public int GetCountOfUnplacedItems(string itemId)
        {
            return _ownedItems.Count(i => i.id == itemId && i.x == 0 && i.z == 0 && i.isPlacedOnSocket == false);
        }

        public Guid GetUidOfUnplacedItem(string itemID)
        {
            return _ownedItems.First(i => i.id == itemID && i.x == 0 && i.z == 0 && i.isPlacedOnSocket == false).uid;
        }
    }
}