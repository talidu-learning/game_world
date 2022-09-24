using System.Linq;
using Shop;
using UnityEngine;

namespace ServerConnection
{
    public class LocalPlayerData : MonoBehaviour
    {
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

        public string GetJSONData()
        {
            return JsonUtility.ToJson(_playerDataConatiner);
        }

        public void Initilize(PlayerDataContainer playerDataContainer)
        {
            _playerDataConatiner = playerDataContainer;
            StarCountUI.UpdateStarCount.Invoke(_playerDataConatiner.Stars.ToString());
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
            return _playerDataConatiner.Stars;
        }

        public bool TryBuyItem(string id, int itemValue)
        {
            if (_playerDataConatiner.Stars - itemValue >= 0)
            {
                _playerDataConatiner.Stars -= itemValue;
                StarCountUI.UpdateStarCount.Invoke(_playerDataConatiner.Stars.ToString());
                _playerDataConatiner._ownedItems.Add(new ItemData
                {
                    id = id
                });
                return true;
            }

            return false;
        }

        public void OnPlacedItem(string id, float x, float z)
        {
            _playerDataConatiner._ownedItems.Add(new ItemData
            {
                id = id,
                x = x,
                z = z
            });
        }

        public void OnWithdrewItem(string id)
        {
            var item = _playerDataConatiner._ownedItems.First(o => o.id == id);
            item.x = 0;
            item.z = 0;
        }
    }
}
