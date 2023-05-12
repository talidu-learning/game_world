using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Interactables;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEditor;
using UnityEngine;

namespace Shop
{
    public static class Paths
    {
        public const string BackUpPath = "Assets/Shop/shopBackUp.txt";
        public const string PrefabPath = "Assets/Interactables/Prefabs/";
        public const string BasePrefabPath = "Assets/Interactables/Prefabs/NewPrefabs\\0InteractablePrefab.prefab";
    }
    public class ShopInventoryBackup : MonoBehaviour
    {
        [MenuItem("Assets/Save Back Up Shop Inventory", true)]
        private static bool GenerateBackUpFileValidation() {
            return Selection.activeObject is ShopInventory;
        }
        
        [MenuItem("Assets/Load Back Up Shop Inventory", true)]
        private static bool LoadBackUpFileValidation() {
            return Selection.activeObject is ShopInventory;
        }
        
        [MenuItem("Assets/Save Back Up Shop Inventory")]
        static void GenerateBackUpFile()
        {
            var shopInventory = GetShopInventory();

            var shopItems = GetJsonShopItems(shopInventory);

            string json = JsonConvert.SerializeObject(shopItems);
            WriteJsonBackUpFile(json);
            Debug.Log("Back Up completed");
        }

        private static void WriteJsonBackUpFile(string json)
        {
            StreamWriter writer = new StreamWriter(Paths.BackUpPath, false);
            writer.WriteLine(json);
            writer.Close();
        }

        private static List<JsonShopItem> GetJsonShopItems(ShopInventory shopInventory)
        {
            List<JsonShopItem> shopItems = new List<JsonShopItem>();

            Debug.Log("Number of items to save: " + shopInventory.ShopItems.Count);
            foreach (var shopItem in shopInventory.ShopItems)
            {
                JsonShopItem item = new JsonShopItem
                {
                    ItemId = shopItem.ItemID,
                    BaseItemId = shopItem.BaseItemID,
                    Value = shopItem.Value,
                    DefaultColor = GetFloatArray(shopItem.DefaultColor),
                    PrefabName = shopItem.Prefab.name
                };

                if (shopItem.Prefab == null)
                {
                    Debug.LogWarning($"{item.ItemId} has no prefab!");
                }

                foreach (var variant in shopItem.ItemVariants)
                {
                    if (variant.Prefab == null)
                    {
                        Debug.LogWarning($"{variant.ItemID} (ItemVariant of {item.ItemId}) has no prefab!");
                    }

                    item.ItemVariants.Add(new JsonItemVariant
                    {
                        ItemId = variant.ItemID,
                        PrefabName = variant.Prefab != null ? variant.Prefab.name : null,
                        Color = GetFloatArray(variant.Color)
                    });
                }

                foreach (var attribute in shopItem.Attributes)
                {
                    item.Attributes.Add(attribute);
                }

                shopItems.Add(item);
            }

            return shopItems;
        }

        public static ShopInventory GetShopInventory()
        {
            var path = AssetDatabase.GetAssetPath(Selection.objects[0].GetInstanceID());
            ShopInventory shopInventory = AssetDatabase.LoadAssetAtPath<ShopInventory>(path);
            return shopInventory;
        }

        [MenuItem("Assets/Load Back Up Shop Inventory")]
        static void LoadBackUpFile()
        {
            var shopInventory = GetShopInventory();
            
            shopInventory.ShopItems.Clear();
            
            var jsonShopItems = GetJsonShopItemsBackUp();
            
            Debug.Log("Number of found items: " + jsonShopItems.Count);

            Dictionary<string, GameObject> prefabs = PrefabGenerator.LoadPrefabs();

            GenerateShopInventory(jsonShopItems, prefabs, shopInventory);
            
            PrefabGenerator.AssignSpritesToShopInventoryItems();
            
            Debug.Log("Back Up loaded");
            
        }

        private static void GenerateShopInventory(List<JsonShopItem> jsonShopItems, Dictionary<string, GameObject> prefabs, ShopInventory shopInventory)
        {
            foreach (var item in jsonShopItems)
            {
                GameObject itemPrefab = prefabs.FirstOrDefault(p => p.Key == item.PrefabName).Value;

                ShopItemData shopItemData = new ShopItemData
                {
                    ItemID = item.ItemId,
                    BaseItemID = item.BaseItemId,
                    Value = item.Value,
                    DefaultColor = ConvertToColor(item.DefaultColor),
                    Prefab = itemPrefab
                };

                foreach (var variant in item.ItemVariants)
                {
                    GameObject prefab = prefabs.FirstOrDefault(p => p.Key == variant.PrefabName).Value;
                    shopItemData.ItemVariants.Add(new ItemVariant
                    {
                        ItemID = variant.ItemId,
                        Prefab = prefab,
                        Color = ConvertToColor(variant.Color)
                    });
                }

                foreach (var attribute in item.Attributes)
                {
                    shopItemData.Attributes.Add(attribute);
                }

                shopInventory.ShopItems.Add(shopItemData);
            }
        }

        private static List<JsonShopItem> GetJsonShopItemsBackUp()
        {
            var text = AssetDatabase.LoadAssetAtPath<TextAsset>(Paths.BackUpPath).text;
            List<JsonShopItem> jsonShopItems = JsonConvert.DeserializeObject<List<JsonShopItem>>(text);
            return jsonShopItems;
        }

        private static float[] GetFloatArray(Color color)
        {
            return new[] {color.r, color.g, color.b, color.a};
        }

        private static Color ConvertToColor(float[] values)
        {
            return new Color(values[0], values[1], values[2], values[3]);
        }
    }
    
    [Serializable]
    class JsonShopItem
    {
        public string ItemId;
        public string BaseItemId;
        public float[] DefaultColor = new []{0f,0f,0f,0f};
        public int Value;
        public string PrefabName;

        [JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
        public List<ItemAttribute> Attributes = new List<ItemAttribute>();

        public List<JsonItemVariant> ItemVariants = new List<JsonItemVariant>();
    }

    [Serializable]
    class JsonItemVariant
    {
        public string ItemId;
        public string PrefabName;
        public float[] Color = new []{0f,0f,0f,0f};
    }
}
