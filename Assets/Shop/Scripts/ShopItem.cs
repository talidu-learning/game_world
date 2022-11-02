using System.Collections;
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
        [SerializeField] private TextMeshProUGUI Owned;
        [SerializeField] private TextMeshProUGUI Placed;
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

        private void OnBuyItemButtonClick()
        {
            if (BuyItem())
            {
                UpdateUI();
                placeItem.gameObject.SetActive(true);
            }
        }

        private void UpdateUI()
        {
            int owned = LocalPlayerData.Instance.GetCountOfOwnedItems(itemID);
            int unplaced = LocalPlayerData.Instance.GetCountOfUnplacedItems(itemID);
            Owned.text = owned.ToString();
            Placed.text = unplaced.ToString();
            
            if(unplaced != 0) placeItem.gameObject.SetActive(true);
            else
            {
                placeItem.gameObject.SetActive(false);
            }
        }
        
        public void UpdateUIOffByOne()
        {
            int owned = LocalPlayerData.Instance.GetCountOfOwnedItems(itemID);
            int unplaced = LocalPlayerData.Instance.GetCountOfUnplacedItems(itemID);
            int offByOne = (owned - unplaced - 1);
            Owned.text = owned.ToString();
            Placed.text = offByOne.ToString();
            
            if(offByOne == 0) placeItem.gameObject.SetActive(false);
        }

        private void OnPlaceItemButtonClick()
        {
            if(LocalPlayerData.Instance.GetCountOfUnplacedItems(itemID) == 1)placeItem.gameObject.SetActive(false);
            ShopManager.InitilizePlaceObjectEvent.Invoke(itemID, uitemID);
            
            if(LocalPlayerData.Instance.GetCountOfUnplacedItems(itemID) <= 1) placeItem.gameObject.SetActive(false);
            
            UpdateUIOffByOne();
        }

        public void Initialize(ItemData itemData)
        {
            itemID = itemData.ItemID;
            ItemImage.sprite = itemData.ItemSprite;
            PriceTag.text = itemData.Value.ToString();
            itemValue = itemData.Value;
            
            UpdateUI();
        }

        public void WasInvalidPlacement()
        {
            UpdateUI();
        }

        private bool BuyItem()
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
            StartAsyncUpdate();
        }
    }
}