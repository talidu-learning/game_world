using System;
using System.Linq;
using System.Threading.Tasks;
using Shop;
using UnityEngine;

namespace ServerConnection
{
    public class LocalPlayerData : MonoBehaviour
    {
        [SerializeField]private Game.ServerConnection ServerConnection;
        
        public static StringUnityEvent ChangedItemDataEvent = new StringUnityEvent();
        
        private int _stars = 500;
        
        private PlayerDataContainer _playerDataConatiner = new PlayerDataContainer();

        private static LocalPlayerData _instance;

        public static LocalPlayerData Instance
        {
            get { return _instance; }
        }


        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        public string GetJsonData()
        {
            return JsonUtility.ToJson(_playerDataConatiner);
        }

        public void Initialize(PlayerDataContainer playerDataContainer)
        {
            _playerDataConatiner = playerDataContainer;
            StarCountUI.UpdateStarCount.Invoke(_stars.ToString());
        }

        public ItemData[] GetOwnedItems()
        {
            return _playerDataConatiner._ownedItems.ToArray();
        }
        
        public ItemData[] GetPlacedItems()
        {
            return _playerDataConatiner._ownedItems.Where(o=> o.x != 0 && o.z != 0).ToArray();
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
                if (updateStarCount && newItem != null)
                {
                    _stars -= itemValue;
                    StarCountUI.UpdateStarCount.Invoke(_stars.ToString());
                    _playerDataConatiner._ownedItems.Add(newItem);
                
                    ChangedItemDataEvent.Invoke(id);
                    return true;   
                }
            }

            return false;
        }

        public void OnPlacedItem(Guid uid, float x, float z)
        {
            var item = _playerDataConatiner._ownedItems.First(i => i.uid == uid);
            
            item.x = x;
            item.z = z;
            
            ChangedItemDataEvent.Invoke(item.id);
        }
        
        public void OnPlacedItem(Guid uid, float x, float z, Guid socketcolectionuid, int socketcount, int socketindex)
        {
            var item = _playerDataConatiner._ownedItems.First(i => i.uid == uid);

            var socketItem = _playerDataConatiner._ownedItems.First(i => i.uid == socketcolectionuid);
            
            
            if(socketItem.itemsPlacedOnSockets == null) socketItem.itemsPlacedOnSockets = new Guid[socketcount];
            socketItem.itemsPlacedOnSockets[socketindex] = uid;

            item.x = x;
            item.z = z;
            
            ChangedItemDataEvent.Invoke(item.id);
        }

        public void OnWithdrewItem(Guid uid)
        {
            var item = _playerDataConatiner._ownedItems.FirstOrDefault(o => o.uid == uid && o.x != 0 && o.z!=0);

            item.x = 0;
            item.z = 0;
            
            ChangedItemDataEvent.Invoke(item.id);

        }
        
        public void OnWithdrewItem(Guid uid, Guid socketcollectionuid, int socketindex)
        {
            var item = _playerDataConatiner._ownedItems.FirstOrDefault(o => o.uid == uid && o.x != 0 && o.z!=0);
            
            var socketcollection = _playerDataConatiner._ownedItems.First(o => o.uid == socketcollectionuid)
                .itemsPlacedOnSockets;
            
            socketcollection[socketindex] = Guid.Empty;
            item.x = 0;
            item.z = 0;
            
            ChangedItemDataEvent.Invoke(item.id);
        }

        public bool IsItemPlaceable(string itemId)
        {
            return _playerDataConatiner._ownedItems.Any(o => o.id == itemId && o.x == 0 && o.z == 0);
        }
        
        public int GetCountOfOwnedItems(string itemId)
        {
            return _playerDataConatiner._ownedItems.Count(i => i.id == itemId);
        }

        public int GetCountOfUnplacedItems(string itemId)
        {
            return _playerDataConatiner._ownedItems.Count(i => i.id == itemId && i.x == 0 && i.z == 0);
        }

        public Guid GetUIDOfUnplacedItem(string itemID)
        {
            return _playerDataConatiner._ownedItems.First(i=> i.id == itemID && i.x == 0 && i.z == 0).uid;
        }
    }
}
