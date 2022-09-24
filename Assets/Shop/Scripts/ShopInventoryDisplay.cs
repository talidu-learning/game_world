using System;
using System.Collections.Generic;
using ServerConnection;
using UnityEngine;

namespace Shop
{
    public class ShopInventoryDisplay : MonoBehaviour
    {
        [SerializeField] private ShopInventory ShopInventory;
        [SerializeField] private GameObject ContentViewport;
        [SerializeField] private GameObject ItemPrefab;

        private Dictionary<string, ShopItem> _shopItems = new Dictionary<string, ShopItem>();

        public void OnPlacedItem(string id, bool wasValidPlacement)
        {
            if (_shopItems.TryGetValue(id, out ShopItem item))
            {
                if(wasValidPlacement) item.PlaceItem();
                else item.MarkItemAsBought();
            }
        }

        private void Awake()
        {
            BuildShopInventory();
        }

        private void Start()
        {
            SaveGame.LoadedPlayerData.AddListener(UpdateItemStates);
        }

        private void UpdateItemStates()
        {
            var ownedItems = LocalPlayerData.Instance.GetOwnedItems();
            var placedItems = LocalPlayerData.Instance.GetPlacedItems();
            
            Debug.Log(placedItems.Length);
            
            foreach (var item in ownedItems)
            {
                if (_shopItems.TryGetValue(item.id, out ShopItem shopItem))
                {
                    if (Array.IndexOf(placedItems, item) > -1)
                    {
                        shopItem.PlaceItem();
                    }
                    else
                    {
                        shopItem.MarkItemAsBought();
                    }
                }
            }
        }

        private void BuildShopInventory()
        {
            foreach (var item in ShopInventory.ShopItems)
            {
                var newItem = Instantiate(ItemPrefab, ContentViewport.transform);
                newItem.GetComponent<ShopItem>().Initialize(item);
                _shopItems.Add(item.ItemID, newItem.GetComponent<ShopItem>());
            }
        }
    }
}