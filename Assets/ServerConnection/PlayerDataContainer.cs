using System;
using System.Collections.Generic;

namespace ServerConnection
{
    public class PlayerDataContainer
    {
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