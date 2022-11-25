using System.Collections.Generic;
using System.Linq;
using ServerConnection;
using UnityEngine;

namespace Shop
{
    public class ShopInventoryDisplay : MonoBehaviour
    {
        [SerializeField] private ShopInventory ShopInventory;
        [SerializeField] private GameObject ContentViewport;
        [SerializeField] private GameObject ShopItemPrefab;

        private Dictionary<string, ShopItem> _shopItems = new();

        public void OnPlacedItem(string id, bool wasValidPlacement, bool isOneCopyLeft)
        {
            if (_shopItems.TryGetValue(id, out ShopItem item))
            {
                if(wasValidPlacement && !isOneCopyLeft) item.PlaceItem(false);
                else item.WasInvalidPlacement();
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
            
            foreach (var item in _shopItems)
            {
                    if (ownedItems.Count(i=> i.id == item.Key) > 0 || !LocalPlayerData.Instance.IsItemPlaceable(item.Key))
                    {
                        item.Value.PlaceItem(false);
                    }
            }
        }

        private void BuildShopInventory()
        {
            foreach (var item in ShopInventory.ShopItems)
            {
                var newItem = Instantiate(ShopItemPrefab, ContentViewport.transform);
                newItem.GetComponent<ShopItem>().Initialize(item);
                _shopItems.Add(item.ItemID, newItem.GetComponent<ShopItem>());
            }
        }
    }
}