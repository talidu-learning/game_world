using System.Linq;
using Shop;
using UnityEngine;

namespace ServerConnection
{
    public class LocalPlayerData : MonoBehaviour
    {
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

        public void Initilize(PlayerDataContainer playerDataContainer)
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

        public bool TryBuyItem(string id, int itemValue)
        {
            if (_stars - itemValue >= 0)
            {
                _stars -= itemValue;
                StarCountUI.UpdateStarCount.Invoke(_stars.ToString());
                _playerDataConatiner._ownedItems.Add(new ItemData
                {
                    id = id,
                    uid = _playerDataConatiner._ownedItems.Count + 1
                });
                return true;
            }

            return false;
        }

        public void OnPlacedItem(int uid, float x, float z)
        {
            var item = _playerDataConatiner._ownedItems.First(i => i.uid == uid);
            item.x = x;
            item.z = z;
        }

        public void OnWithdrewItem(int uid)
        {
            var item = _playerDataConatiner._ownedItems.FirstOrDefault(o => o.uid == uid && o.x != 0 && o.z!=0);
            if (item == null) return;
            item.x = 0;
            item.z = 0;
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

        public int GetUIDOfUnplacedItem(string itemID)
        {
            return _playerDataConatiner._ownedItems.First(i=> i.x == 0 && i.z == 0).uid;
        }
    }
}
