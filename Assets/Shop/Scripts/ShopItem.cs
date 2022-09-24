using Interactables;
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
        [SerializeField] private ItemButton itemButton;
        [SerializeField] private Button button;
        public string itemID { private set; get; }

        private ShopItemButtonState _currentState = ShopItemButtonState.Default;

        private int itemValue;

        private void Awake()
        {
            button.onClick.AddListener(OnItemButtonClick);
        }

        private void OnItemButtonClick()
        {
            switch (_currentState)
            {
                case ShopItemButtonState.Default:
                    BuyItem();
                    break;
                case ShopItemButtonState.Bought:
                    ShopManager.PlaceObjectEvent.Invoke(itemID);
                    itemButton.ChangeState(ShopItemButtonState.Placed);
                    _currentState = ShopItemButtonState.Placed;
                    break;
            }
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
            itemButton.ChangeState(ShopItemButtonState.Bought);
            _currentState = ShopItemButtonState.Bought;
        }

        public void BuyItem()
        {
            if (LocalPlayerData.Instance.TryBuyItem(itemValue))
            {
                itemButton.ChangeState(ShopItemButtonState.Bought);
                _currentState = ShopItemButtonState.Bought;
            }
        }
        
        public void PlaceItem()
        {
            itemButton.ChangeState(ShopItemButtonState.Placed);
            _currentState = ShopItemButtonState.Placed;
        }
    }
}