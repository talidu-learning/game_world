using System.Collections.Generic;
using Interactables;
using UnityEngine;

namespace Shop
{
    public class ItemID : MonoBehaviour
    {
        public string id;
        public int uid;

        public List<ItemAttribute> ItemAttributes = new List<ItemAttribute>();
    }
}