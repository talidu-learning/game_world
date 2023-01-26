using System.Collections.Generic;
using System.Linq;
using Enumerations;
using Interactables;
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
                if (wasValidPlacement && !isOneCopyLeft) item.PlaceItem(false);
                else item.WasInvalidPlacement();
            }
        }

        private void Awake()
        {
            Debug.Log("BuildShop");
            BuildShopInventory();
        }

        private void Start()
        {
            SaveGame.TEXT_LOADED_PLAYER_DATA.AddListener(UpdateItemStates);
            UIManager.FILTER_EVENT.AddListener(OnFilterToggled);

            LocalPlayerData.ChangedItemDataEvent.AddListener(UpdateItemState);
        }

        private void UpdateItemState(string id)
        {
            var go = _shopItems.FirstOrDefault(o => o.Key == id).Value;
            var shopItem = go.GetComponent<ShopItem>();
            var itemdata = ShopInventory.ShopItems.FirstOrDefault(i => i.ItemID == id);
            shopItem.Initialize(itemdata);
        }

        private void OnFilterToggled(UIType uiType, List<ItemAttribute> attributes, bool isActive)
        {
            if (uiType == UIType.Inventory) return;

            if (attributes.Contains(ItemAttribute.Bought))
            {
                foreach (var item in _shopItems)
                {
                    if (LocalPlayerData.Instance.GetCountOfOwnedItems(item.Key) == 0)
                        item.Value.gameObject.SetActive(!isActive);
                }

                return;
            }


            foreach (var attribute in attributes)
            {
                foreach (var item in _shopItems)
                {
                    if (item.Value.GetComponent<ShopItem>().attributes.Contains(attribute))
                        item.Value.gameObject.SetActive(!isActive);
                }
            }
        }

        private void UpdateItemStates()
        {
            var ownedItems = LocalPlayerData.Instance.GetOwnedItems();

            foreach (var item in _shopItems)
            {
                if (ownedItems.Count(i => i.id == item.Key) > 0 || !LocalPlayerData.Instance.IsItemPlaceable(item.Key))
                {
                    item.Value.PlaceItem(false);
                }
            }
        }

        private void BuildShopInventory()
        {
            Debug.Log("ShopItems: " + ShopInventory.ShopItems.Count);
            foreach (var item in ShopInventory.ShopItems)
            {
                var newItem = Instantiate(ShopItemPrefab, ContentViewport.transform);
                newItem.GetComponent<ShopItem>().Initialize(item);
                _shopItems.Add(item.ItemID, newItem.GetComponent<ShopItem>());
            }
        }
    }
}