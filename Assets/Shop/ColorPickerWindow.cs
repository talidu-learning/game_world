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

            while (variantsArray.Length > variantObjects.Count)
            {
                variantObjects.Add(Instantiate(VariantPrefab, transform.GetChild(0)));
            }
            
            var gameObjects = variantObjects.ToArray();

            for (int i = 0; i < variantsArray.Length; i++)
            {
                gameObjects[i].SetActive(true);
                var cv = gameObjects[i].GetComponent<ColorVariant>();
                cv.VariantID = variants[i].ItemID;
                cv.ShopItem = shopItem;
                cv.SetColor(variants[i].Color);
            }

            for (int i = variantsArray.Length; i < gameObjects.Length; i++)
            {
                gameObjects[i].SetActive(false);
            }
            
        }
    }
}
