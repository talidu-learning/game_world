﻿using System;
using System.Collections.Generic;
using System.Linq;
using Enumerations;
using Interactables;
using ServerConnection;
using Shop;
using UnityEngine;

namespace Inventory
{
    public class ItemInventoryUI : MonoBehaviour
    {
        public static StringUnityEvent OnBoughtItemEvent = new StringUnityEvent();
        
        [SerializeField] private ShopInventory ShopInventory;
        [SerializeField] private GameObject InventoryItemPrefab;
        [SerializeField] private GameObject InventoryUIContent;

        private Dictionary<string, GameObject> inventoryItems = new Dictionary<string, GameObject>();
        
        private void Awake()
        {
            OnBoughtItemEvent.AddListener(OnBoughtItem);
        }

        private void Start()
        {
            SaveGame.LoadedPlayerData.AddListener(UpdateItemStates);
            
            UIManager.FILTER_EVENT.AddListener(OnFilterToggled);
        }

        private void OnFilterToggled(UIType uiType, List<ItemAttribute> attributes, bool isActive)
        {
            if(uiType == UIType.Shop) return;

            foreach (var attribute in attributes)
            {
                foreach (var item in inventoryItems)
                {
                    if(item.Value.GetComponent<InventoryItem>().attributes.Contains(attribute))
                        item.Value.SetActive(!isActive);
                }
            }
        }

        private void UpdateItemStates()
        {
            var ownedItems = LocalPlayerData.Instance.GetOwnedItems();

            var uniqueItems =
                ownedItems.GroupBy(i => i.id, o => o.uid, (key, uid) => new {Id = key, UIDs = uid.ToList()});

            foreach (var item in uniqueItems)
            {
                CreateInventoryItem(item.Id);
            }
        }
        
        private void OnBoughtItem(string itemId)
        {
            if (inventoryItems.TryGetValue(itemId, out GameObject key))
            {
                key.GetComponent<InventoryItem>().UpdateUIOffByOne();
            }
            else
            {
                CreateInventoryItem(itemId);
            }
        }

        private void CreateInventoryItem(string itemId)
        {
            var itemGO = Instantiate(InventoryItemPrefab, InventoryUIContent.transform);
            var inventoryItem = itemGO.GetComponent<InventoryItem>();
            var itemData = ShopInventory.ShopItems.FirstOrDefault(i => i.ItemID == itemId);
            inventoryItem.Initialize(itemData);

            inventoryItems.Add(itemId, itemGO);
        }
    }
}