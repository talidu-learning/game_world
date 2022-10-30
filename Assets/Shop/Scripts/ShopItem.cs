using ServerConnection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI PriceTag;
        [SerializeField] private Image ItemImage;
        [SerializeField] private Button placeItem;
        [SerializeField] private Button button;
        public string itemID { private set; get; }
        public int uitemID { private set; get; }

        private int itemValue;

        private void Awake()
        {
            button.onClick.AddListener(OnBuyItemButtonClick);
            placeItem.onClick.AddListener(OnPlaceItemButtonClick);
        }

        private void OnBuyItemButtonClick()
        {
            if (BuyItem())
            {
                placeItem.gameObject.SetActive(true);
            }
        }

        private void OnPlaceItemButtonClick()
        {
            if(LocalPlayerData.Instance.GetCountOfUnplacedItems(itemID) == 1)placeItem.gameObject.SetActive(false);
            ShopManager.InitilizePlaceObjectEvent.Invoke(itemID, uitemID);
        }

        public void Initialize(ItemData itemData)
        {
            itemID = itemData.ItemID;
            ItemImage.sprite = itemData.ItemSprite;
            PriceTag.text = itemData.Value.ToString();
            itemValue = itemData.Value;
        }

        public void MarkItemAsBought()
        {
            placeItem.gameObject.SetActive(true);
        }

        public bool BuyItem()
        {
            if (LocalPlayerData.Instance.TryBuyItem(itemID, itemValue))
            {
                return true;
            }

            return false;
        }

        public void PlaceItem(bool isCopyLeft)
        {
            if(isCopyLeft) return;
            placeItem.gameObject.SetActive(false);
        }
    }
}