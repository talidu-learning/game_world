using System;
using System.Collections.Generic;
using Interactables;
using UnityEngine;

namespace Shop
{
    public class ItemID : MonoBehaviour
    {
        public string id;
        public Guid uid;
        public bool IsFlipped;

        public List<ItemAttribute> ItemAttributes = new List<ItemAttribute>();
    }
}