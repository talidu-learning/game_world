using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Inventory
{
    public class ToggleInventoryButton : MonoBehaviour
    {
        public static UnityEvent CloseInventoryUnityEvent = new UnityEvent();
        public static UnityEvent ClosedInventoryUnityEvent = new UnityEvent();
        public static UnityEvent OpenedInventoryEvent = new UnityEvent();
    
        [SerializeField] private GameObject InventoryUI;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OpenInventory);
            CloseInventoryUnityEvent.AddListener(Close);
        }

        public void CloseInventory()
        {
            Close();
        }

        public void OpenInventory()
        {
            Open();
            OpenedInventoryEvent.Invoke();
        }
        
        private void Open()
        {
            LeanTween.moveLocalY(InventoryUI, 0, 0.8f).setEase(LeanTweenType.easeOutElastic);
        }

        private void Close()
        {
            LeanTween.moveLocalY(InventoryUI, -1079.0f, 1.0f).setEase(LeanTweenType.easeOutElastic);
            ClosedInventoryUnityEvent.Invoke();
        }
    }
}
