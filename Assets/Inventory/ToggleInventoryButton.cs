using Enumerations;
using Interactables;
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

        private void Start()
        {
            DecorationModeButton.ToggledDecoModeButtonEvent.AddListener(OnToggledDecoMode);
        }

        private void OnToggledDecoMode()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        public void CloseInventory()
        {
            GameAudio.PlaySoundEvent.Invoke(SoundType.CloseInventory);
            Close();
        }

        public void OpenInventory()
        {
            GameAudio.PlaySoundEvent.Invoke(SoundType.OpenInventory);
            Open();
            OpenedInventoryEvent.Invoke();
        }

        private void Open()
        {
            LeanTween.moveLocalY(InventoryUI, 0, 0.8f).setEase(LeanTweenType.easeOutElastic);
        }

        private void Close()
        {
            LeanTween.move(InventoryUI, new Vector2(Screen.width / 2, -Screen.height - 10f), 0.8f)
                .setEase(LeanTweenType.easeOutElastic);
            ClosedInventoryUnityEvent.Invoke();
        }
    }
}