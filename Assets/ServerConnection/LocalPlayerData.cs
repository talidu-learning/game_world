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
        }

        public string[] GetOwnedItems()
        {
            return _playerDataConatiner._ownedItems.Split(",");
        }
        
        public string[] GetPlacedItems()
        {
            return _playerDataConatiner._placedItems.ToArray();
        }

        public int GetStarCount()
        {
            return _playerDataConatiner.Stars;
        }

        public bool TryBuyItem(int itemValue)
        {
            if (_playerDataConatiner.Stars - itemValue >= 0)
            {
                _playerDataConatiner.Stars -= itemValue;
                StarCountUI.UpdateStarCount.Invoke(_playerDataConatiner.Stars.ToString());
                return true;
            }

            return false;
        }
    }
}
