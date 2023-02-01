using System.Collections.Generic;
using UnityEngine;

namespace Shop
{
    public class ColorPickerWindow : MonoBehaviour
    {
        [SerializeField] private GameObject VariantPrefab;

        private List<GameObject> variantObjects = new List<GameObject>();
        
        public void InitializeMenu(List<ItemVariant> variants, ShopItem shopItem)
        {
            var variantsArray = variants.ToArray();

            // +2 because of base item
            while (variantsArray.Length + 1 > variantObjects.Count)
            {
                variantObjects.Add(Instantiate(VariantPrefab, transform.GetChild(0)));
            }
            
            var gameObjects = variantObjects.ToArray();

            var baseItem = gameObjects[0].GetComponent<ColorVariant>();
            InitializeColorVariant(shopItem, baseItem, new ItemVariant
            {
                ItemID = shopItem.BaseItemID,
                Color = Color.white
            });

            for (int i = 0; i < variantsArray.Length; i++)
            {
                gameObjects[i+1].SetActive(true);
                var cv = gameObjects[i+1].GetComponent<ColorVariant>();
                InitializeColorVariant(shopItem, cv, variants[i]);
            }

            for (int i = variantsArray.Length + 1; i < gameObjects.Length; i++)
            {
                gameObjects[i].SetActive(false);
            }
            
        }

        private static void InitializeColorVariant(ShopItem shopItem, ColorVariant cv, ItemVariant itemVariant)
        {
            cv.VariantID = itemVariant.ItemID;
            cv.ShopItem = shopItem;
            cv.SetColor(itemVariant.Color);
        }
    }
}
