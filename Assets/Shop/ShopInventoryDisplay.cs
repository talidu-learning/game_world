using UnityEngine;

namespace Shop
{
    public class ShopInventoryDisplay : MonoBehaviour
    {

        [SerializeField] private ShopInventory ShopInventory;
        [SerializeField] private GameObject ContentViewport;
        [SerializeField] private GameObject ItemPrefab;
        
        private void Awake()
        {
            foreach (var item in ShopInventory.ShopItems)
            {
                var newItem = Instantiate(ItemPrefab, ContentViewport.transform);
                newItem.GetComponent<ShopItem>().Initialize(item);
            }
        }
    }
}