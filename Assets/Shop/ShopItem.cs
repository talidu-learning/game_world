using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI PriceTag;
        [SerializeField] private Image ItemImage;
        
        public void Initialize(ItemData itemData)
        {
            ItemImage.sprite = itemData.ItemSprite;
            PriceTag.text = itemData.Value.ToString();
        }
    }
}