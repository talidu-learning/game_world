using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enumerations;
using Interactables;
using Inventory;
using ServerConnection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI PriceTag;
        [SerializeField] private TextMeshProUGUI Owned;
        [SerializeField] private TextMeshProUGUI Placed;
        [SerializeField] private Image ItemImage;
        [SerializeField] private Button button;
        public string itemID { private set; get; }

        private int itemValue;
        
        public List<ItemAttribute> attributes{ private set; get; }

        private void Awake()
        {
            button.onClick.AddListener(OnBuyItemButtonClick);
        }

        private void Start()
        {
            SelectionManager.DESELECT_OBJECT_EVENT.AddListener(StartAsyncUpdate);
            SelectionManager.WITHDRAW_OBJECT_EVENT.AddListener(StartAsyncUpdate);
        }

        private void StartAsyncUpdate()
        {
            StartCoroutine(AsyncUpdate());
        }
        
        private IEnumerator AsyncUpdate()
        {
            yield return null;
            UpdateUI();
        }

        private async void OnBuyItemButtonClick()
        {
            var boughtItem = await BuyItem();
            if (boughtItem)
            {
                GameAudio.PlaySoundEvent.Invoke(SoundType.Buy);
                UpdateUI();
                var uitemID = LocalPlayerData.Instance.GetUIDOfUnplacedItem(itemID);
                ShopManager.InitilizePlaceObjectEvent.Invoke(itemID, uitemID);
                ItemInventoryUI.OnBoughtItemEvent.Invoke(itemID);
            }
        }

        private void UpdateUI()
        {
            int owned = LocalPlayerData.Instance.GetCountOfOwnedItems(itemID);
            int unplaced = LocalPlayerData.Instance.GetCountOfUnplacedItems(itemID);
            Owned.text = owned.ToString();
            Placed.text = unplaced.ToString();
        }
        
        public void Initialize(ShopItemData shopItemData)
        {
            itemID = shopItemData.ItemID;
            ItemImage.sprite = shopItemData.ItemSprite;
            PriceTag.text = shopItemData.Value.ToString();
            itemValue = shopItemData.Value;
            attributes = shopItemData.Attributes;
            
            UpdateUI();
        }

        public void WasInvalidPlacement()
        {
            UpdateUI();
        }

        private async Task<bool> BuyItem()
        {
            var tryBuyItem = await LocalPlayerData.Instance.TryBuyItem(itemID, itemValue);
            if (tryBuyItem)
            {
                return true;
            }

            return false;
        }

        public void PlaceItem(bool isCopyLeft)
        {
            if(isCopyLeft) return;
            // placeItem.gameObject.SetActive(false);
            StartAsyncUpdate();
        }
    }
}