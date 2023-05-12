using System;
using System.Collections.Generic;
using Interactables;
using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "ShopInventory", menuName = "ScriptableObjects/Shop Inventory")]
    public class ShopInventory : ScriptableObject
    {
        public List<ShopItemData> ShopItems = new List<ShopItemData>();
    }

    [Serializable]
    public class ShopItemData
    {
        public string ItemID;
        [Tooltip("ItemID of default item variant, if there is one")]
        public string BaseItemID;
        public Sprite ItemSprite;
        [Tooltip("Color for displaying the default variant of this item")]
        public Color DefaultColor;
        public int Value;
        public GameObject Prefab;
        public List<ItemAttribute> Attributes = new List<ItemAttribute>();
        public List<ItemVariant> ItemVariants = new List<ItemVariant>();
    }
}