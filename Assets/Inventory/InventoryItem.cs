using System.Collections;
using Interactables;
using ServerConnection;
using Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Unplaced;
        [SerializeField] private Image ItemImage;
        public string itemID { private set; get; }

        public void Awake()
        {
            GetComponent<Button>().onClick.AddListener(PlaceItem);
        }

        private void Start()
        {
            SelectionManager.DESELECT_OBJECT_EVENT.AddListener(StartAsyncUpdate);
            SelectionManager.WITHDRAW_OBJECT_EVENT.AddListener(StartAsyncUpdate);
        }
        
        private void StartAsyncUpdate()
        {
            gameObject.SetActive(true);
            StartCoroutine(AsyncUpdate());
        }
        
        private IEnumerator AsyncUpdate()
        {
            yield return null;
            UpdateUI();
        }

        private void PlaceItem()
        {
            
            // TODO if(LocalPlayerData.Instance.GetCountOfUnplacedItems(itemID) == 1)
            var uitemID = LocalPlayerData.Instance.GetUIDOfUnplacedItem(itemID);
            ShopManager.InitilizePlaceObjectEvent.Invoke(itemID, uitemID);
            
            ToggleInventoryButton.CloseInventoryUnityEvent.Invoke();
            
            // TODO if(LocalPlayerData.Instance.GetCountOfUnplacedItems(itemID) <= 1)
            
            //UpdateUIOffByOne();
        }

        public void UpdateUIOffByOne()
        {
            int owned = LocalPlayerData.Instance.GetCountOfOwnedItems(itemID);
            int unplaced = LocalPlayerData.Instance.GetCountOfUnplacedItems(itemID);
            int offByOne = (owned - unplaced - 1);
            Unplaced.text = offByOne.ToString();
            // TODO if(offByOne == 0)
        }
        
        public void Initialize(ItemData itemData)
        {
            itemID = itemData.ItemID;
            ItemImage.sprite = itemData.ItemSprite;
            
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            int unplaced = LocalPlayerData.Instance.GetCountOfUnplacedItems(itemID);
            
            if(unplaced != 0) gameObject.SetActive(true);
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}