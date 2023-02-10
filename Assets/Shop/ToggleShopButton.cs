using CustomInput;
using Enumerations;
using GameModes;
using Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ToggleShopButton : TaliduButton
    {
        [SerializeField] private ShopPanelTweening ShopPanel;
        [SerializeField] private Sprite ClosedPanel;
        [SerializeField] private Sprite OpenedPanel;

        private bool isOpen = false;

        private void Start()
        {
            GameModeSwitcher.OnSwitchedGameMode.AddListener(OnSwitchedGameMode);
            ToggleInventoryButton.OpenedInventoryEvent.AddListener(OnOpenedInventory);
        }

        private void OnSwitchedGameMode(GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.Deco: 
                    gameObject.SetActive(false);
                    break;
                case GameMode.Terrain: break;
                default: 
                    gameObject.SetActive(true);
                    break;
            }
        }

        private void OnOpenedInventory()
        {
            if (!isOpen) return;

            ShopPanel.Toggle();

            isOpen = false;

            GetComponent<Image>().sprite = ClosedPanel;
        }

        private void OnButtonClick()
        {
            //ShopPanel.gameObject.SetActive(!ShopPanel.gameObject.activeSelf);

            ShopPanel.Toggle();

            isOpen = !isOpen;

            if (isOpen)
            {
                GameAudio.PlaySoundEvent.Invoke(SoundType.OpenShop);
                GetComponent<Image>().sprite = OpenedPanel;
            }
            else
            {
                GameAudio.PlaySoundEvent.Invoke(SoundType.CloseShop);
                GetComponent<Image>().sprite = ClosedPanel;
            }
        }

        protected override void OnClickedButton()
        {
            OnButtonClick();
        }
    }
}