using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shop;
using UnityEditor;
using UnityEngine;

public class PrefabGenerator : MonoBehaviour
{
    public static Dictionary<string, GameObject> LoadPrefabs()
    {
        List<string> prefabPaths =
            Directory.GetFiles(Paths.PrefabPath, "*.prefab", SearchOption.AllDirectories).ToList();
        Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
        foreach (var assetPath in prefabPaths)
        {
            if (assetPath == Paths.BasePrefabPath) continue;
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath).gameObject;
            prefabs.Add(obj.name, obj);
        }

        return prefabs;
    }
    
    [MenuItem("AssetDatabase/GeneratePrefabs")]
    private static void GeneratePrefabs()
    {
        var dirs = GetRelevantDirectories();

        if (GetSprites(dirs, out var sprites)) return;

        Dictionary<string, GameObject> prefabs = LoadPrefabs();

        foreach (var s in sprites)
        {
            if (prefabs.Any(p => p.Key == s.name)) continue;
            GeneratePrefabVariant(s.name, s);
        }
    }
    
    [MenuItem("AssetDatabase/Get CSV File")]
    private static void GetCSVFile()
    {
        var dirs = GetRelevantDirectories();

        if (GetSprites(dirs, out var sprites)) return;

        string fileNames = "Name;Item-Wert;Preis;\n";
        foreach (var s in sprites)
        {
            fileNames += s.name + ";\n";
        }

        StreamWriter writer = new StreamWriter("Assets/itemDatabase.csv", false);
        writer.WriteLine(fileNames);
        writer.Close();
    }

    private static bool GetSprites(List<string> dirs, out List<Sprite> sprites)
    {
        sprites = new List<Sprite>();

        foreach (var d in dirs)
        {
            var filePaths = Directory.GetFiles(d, "*.png", SearchOption.AllDirectories);
            foreach (var path in filePaths)
            {
                if (string.IsNullOrEmpty(path)) return true;
                sprites.Add((Sprite) AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)));
            }
        }

        return false;
    }

    private static List<string> GetRelevantDirectories()
    {
        var dirs = Directory.GetDirectories("Assets/Images/").ToList();
        dirs.Remove("Assets/Images/UI");
        dirs.Remove("Assets/Images/UITemplateSet");
        return dirs;
    }

    [MenuItem("Assets/Generate Interactable Prefab From Texture")]
    private static void GenerateInteractablePrefabFromTexture()
    {
        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            GeneratePrefabVariant(obj.name, (Sprite) AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)));
        }
    }

    [MenuItem("Assets/Generate Interactable Prefab From Texture", true)]
    private static bool GenerateInteractablePrefabFromTextureValidation()
    {
        return Selection.activeObject is Texture2D;
    }

    private static void GeneratePrefabVariant(string prefabName, Sprite sprite)
    {
        GameObject source =
            AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Interactables/Prefabs/0InteractablePrefab.prefab");
        GameObject objSource = (GameObject) PrefabUtility.InstantiatePrefab(source);
        // hier einfach Sachen mit dem prefab machen, bevor es gespeichert wird
        objSource.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = sprite;
        // hier wirds gespeichert und man kann dann nichts mehr am Prefab ändern
        PrefabUtility.SaveAsPrefabAsset(objSource,
            $"Assets/Interactables/Prefabs/GeneratedPrefabs/{prefabName}.prefab");
        DestroyImmediate(objSource.gameObject);
        Debug.Log("Generated: " +
                  $"<a href=\"Assets/Interactables/Prefabs/GeneratedPrefabs/{prefabName}.prefab\" line=\"2\">Double Click here to get to: {prefabName}</a>");
    }

    [MenuItem("Assets/Assign Sprites To Shop Inventory Items")]
    public static void AssignSpritesToShopInventoryItems()
    {
        ShopInventory shopInventory = ShopInventoryBackup.GetShopInventory();

        foreach (var shopItem in shopInventory.ShopItems)
        {
            shopItem.ItemSprite = shopItem.Prefab.GetComponentInChildren<SpriteRenderer>()?.sprite;

            foreach (var variant in shopItem.ItemVariants)
            {
                variant.ItemSprite = variant.Prefab.GetComponentInChildren<SpriteRenderer>()?.sprite;
            }
        }
    }

    [MenuItem("Assets/Assign Sprites To Shop Inventory Items", true)]
    private static bool AssignSpritesToShopInventoryItemsValidation()
    {
        return Selection.activeObject is ShopInventory;
    }
}