using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Shop;
using UnityEditor;
using UnityEngine;

public class PrefabGenerator : MonoBehaviour
{
    [MenuItem("AssetDatabase/GeneratePrefabs")]
    static void GeneratePrefabs()
    {
        var dirs = Directory.GetDirectories("Assets/Images/").ToList();
        dirs.Remove("Assets/Images/UI");
        dirs.Remove("Assets/Images/UITemplateSet");

        List<Sprite> sprites = new List<Sprite>();

        foreach (var d in dirs)
        {
            var filesPaths = Directory.GetFiles(d, "*.png", SearchOption.AllDirectories);
            foreach (var path in filesPaths)
            {
                if (string.IsNullOrEmpty(path)) return;
                sprites.Add((Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)));
            }
        }
            
        List<string> prefabPaths =
            Directory.GetFiles("Assets/Interactables/Prefabs/", "*.prefab", SearchOption.AllDirectories).ToList();
        List<GameObject> prefabs = new List<GameObject>();
        foreach (var path in prefabPaths)
        {
            if(path == "Assets/Interactables/Prefabs/NewPrefabs\\0InteractablePrefab.prefab" ) continue;
            prefabs.Add((GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)));
        }

        foreach (var s in sprites)
        {
            if (prefabs.Any(p => p.name == s.name)) continue;
            GeneratePrefabVariant(s.name, s);
        }
            
    }
    
    [MenuItem("Assets/Generate Interactable Prefab From Texture")]
    private static void GenerateInteractablePrefabFromTexture() {
        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            GeneratePrefabVariant(obj.name, (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)));
        }
    }

    [MenuItem("Assets/Generate Interactable Prefab From Texture", true)]
    private static bool GenerateInteractablePrefabFromTextureValidation() {
        return Selection.activeObject is Texture2D;
    }
    
    public static void GeneratePrefabVariant(string prefabName, Sprite sprite)
    {
        GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Interactables/Prefabs/0InteractablePrefab.prefab");
        GameObject objSource = (GameObject)PrefabUtility.InstantiatePrefab(source);
        // hier einfach Sachen mit dem prefab machen, bevor es gespeichert wird
        objSource.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = sprite;
        // hier wirds gespeichert und man kann dann nichts mehr am Prefab ändern
        PrefabUtility.SaveAsPrefabAsset(objSource, $"Assets/Interactables/Prefabs/GeneratedPrefabs/{prefabName}.prefab");
        DestroyImmediate(objSource.gameObject);
        Debug.Log("Generated: " + $"<a href=\"Assets/Interactables/Prefabs/GeneratedPrefabs/{prefabName}.prefab\" line=\"2\">Double Click here to get to: {prefabName}</a>");
    }
    
    [MenuItem("Assets/Assign Sprites To Shop Inventory Items")]
    private static void AssignSpritesToShopInventoryItems() {
        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            ShopInventory shopInventory = AssetDatabase.LoadAssetAtPath<ShopInventory>(path);

            foreach (var shopItem in shopInventory.ShopItems)
            {
                shopItem.ItemSprite = shopItem.Prefab.GetComponentInChildren<SpriteRenderer>().sprite;

                foreach (var variant in shopItem.ItemVariants)
                {
                    variant.ItemSprite = variant.Prefab.GetComponentInChildren<SpriteRenderer>().sprite;
                }
            }
        }
    }

    [MenuItem("Assets/Assign Sprites To Shop Inventory Items", true)]
    private static bool AssignSpritesToShopInventoryItemsValidation() {
        return Selection.activeObject is ShopInventory;
    }
    
    [MenuItem("Assets/Generate Backup")]
    private static void GenerateBackup() {
        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            ShopInventory shopInventory = AssetDatabase.LoadAssetAtPath<ShopInventory>(path);

            var json = JsonConvert.SerializeObject(shopInventory.ShopItems, Formatting.None, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            Debug.Log("Not implemented.");
        }
    }

    [MenuItem("Assets/Generate Backup", true)]
    private static bool GenerateBackupValidation() {
        return Selection.activeObject is ShopInventory;
    }

    class BackUpShopItem
    {
        
    }
}