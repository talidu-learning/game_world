using System;
using System.Linq;
using UnityEngine;

namespace Shop
{
    public class ItemCreator : MonoBehaviour
    {
        [SerializeField] private ShopInventory _inventory;
        
        public GameObject CreateItem(string id, Guid uid)
        {
            GameObject prefab = _inventory.ShopItems.First(i => i.ItemID == id).Prefab;
            var go = Instantiate(prefab);
            go.AddComponent<ItemID>().id = id;
            go.GetComponent<ItemID>().uid = uid;
            return go;
        }
    }
}