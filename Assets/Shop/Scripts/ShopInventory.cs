using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopInevtory", menuName = "ScriptableObjects/Shop Inventory")]
public class ShopInventory : ScriptableObject
{
    public List<ItemData> ShopItems = new List<ItemData>();
}

[Serializable]
public class ItemData
{
    public string ItemID;
    public Sprite ItemSprite;
    public int Value;
}
