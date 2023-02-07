using Enumerations;
using GameModes;
using Interactables;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory
{
    public class ToggleInventoryButton : TaliduButton
    {
        public static UnityEvent CloseInventoryUnityEvent = new UnityEvent();
        public static UnityEvent ClosedInventoryUnityEvent = new UnityEvent();
        public static UnityEvent OpenedInventoryEvent = new UnityEvent();

        [SerializeField] private GameObject InventoryUI;

        private void Awake()
        {
            CloseInventoryUnityEvent.AddListener(Close);
        }

        protected override void OnClickedButton()
        {
            OpenInventory();
        }

        private void Start()
        {
            GameModeSwitcher.OnSwitchedGameMode.AddListener(OnSwitchedGameModes);
        }

        private void OnSwitchedGameModes(GameMode mode)
        {
            gameObject.SetActive(mode is not (GameMode.Deco or GameMode.Terrain));
        }

        public void CloseInventory()
        {
            GameAudio.PlaySoundEvent.Invoke(SoundType.CloseInventory);
            Close();
        }

        public void OpenInventory()
        {
            GameAudio.PlaySoundEvent.Invoke(SoundType.OpenInventory);
            if(GameModeSwitcher.currentGameMode == GameMode.DefaultPlacing)
                SelectionManager.DESELECT_OBJECT_EVENT.Invoke();
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