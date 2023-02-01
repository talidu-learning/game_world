using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        [SerializeField] private Image ItemImage;
        [SerializeField] private Button Button;
        [SerializeField] private GameObject PaintBucket;
        public string ItemID { set; get; }
        public string BaseItemID { set; get; }
        private Sprite BaseItemSprite { set; get; }
        private int itemValue;
        
        public List<ItemAttribute> Attributes { private set; get; }
        public List<ItemVariant> ItemVariants { private set; get; }

        private void Awake()
        {
            Button.onClick.AddListener(OnBuyItemButtonClick);
        }

        private void Start()
        {
            SelectionManager.DESELECT_OBJECT_EVENT.AddListener(StartAsyncUpdate);
            SelectionManager.DELETE_OBJECT_EVENT.AddListener(StartAsyncUpdate);
        }
        
        #region public

        public void Initialize(ShopItemData shopItemData)
        {
            ItemID = shopItemData.ItemID;
            BaseItemID = shopItemData.BaseItemID;
            if (string.IsNullOrEmpty(BaseItemID))
                BaseItemID = shopItemData.ItemID;
            ItemImage.sprite = shopItemData.ItemSprite;
            BaseItemSprite = shopItemData.ItemSprite;
            PriceTag.text = shopItemData.Value.ToString();
            itemValue = shopItemData.Value;
            Attributes = shopItemData.Attributes;
            ItemVariants = shopItemData.ItemVariants;
            
            if(ItemVariants.Count==0)
                PaintBucket.SetActive(false);

            UpdateUI();
        }

        public void SwitchItemVariant(string variantID)
        {
            ItemID = variantID;
            ItemImage.sprite = variantID != BaseItemID ? ItemVariants.First(v => v.ItemID == variantID).ItemSprite : BaseItemSprite;
            UpdateUI();
        }

        public void OnClickedPaintBucket()
        {
            UIManager.ColorPickerEvent.Invoke(ItemVariants, this);
        }

        public void WasInvalidPlacement()
        {
            UpdateUI();
        }

        public void PlaceItem(bool isCopyLeft)
        {
            if (isCopyLeft) return;
            StartAsyncUpdate();
        }

        #endregion
        #region private

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
                var uitemID = LocalPlayerData.Instance.GetUidOfUnplacedItem(ItemID);
                ShopManager.InitilizePlaceObjectEvent.Invoke(ItemID, uitemID);
                ItemInventoryUI.OnBoughtItemEvent.Invoke(ItemID);
                UIManager.CloseColorPickerEvent.Invoke();
            }
        }

        public void UpdateUI()
        {
            int owned = LocalPlayerData.Instance.GetCountOfOwnedItems(ItemID);
            Owned.text = owned.ToString();
        }

        private async Task<bool> BuyItem()
        {
            var tryBuyItem = await LocalPlayerData.Instance.TryBuyItem(ItemID, itemValue);
            if (tryBuyItem)
            {
                return true;
            }

            return false;
        }

        #endregion

    }
}