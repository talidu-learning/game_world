using System;
using System.Collections.Generic;

namespace ServerConnection
{
    public class PlayerDataContainer
    {
        public string _playerName = "Test";
        public string _credential = "0000";
        public int Stars = 10;
        public List<ItemData> _ownedItems = new List<ItemData>();
    }

    [Serializable]
    public class ItemData
    {
        public string id;
        public int uid;
        public float x;
        public float z;
    }
}