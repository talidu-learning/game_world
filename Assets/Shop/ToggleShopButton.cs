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
        }

        private void OnSwitchedGameMode(GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.Deco: 
                    gameObject.SetActive(false);
                    break;
                case GameMode.Default:
                    gameObject.SetActive(true);
                    break;
                case GameMode.Placing:
                    gameObject.SetActive(false);
                    break;
                case GameMode.Inventory:
                    if (!isOpen) return;
                    ShopPanel.Toggle();
                    isOpen = false;
                    GetComponent<Image>().sprite = ClosedPanel;
                    break;
                case GameMode.DecoInventory:
                    gameObject.SetActive(false);
                    break;
                case GameMode.Shop:
                    gameObject.SetActive(true);
                    break;
                case GameMode.SelectedSocket:
                    gameObject.SetActive(false);
                    break;
                default: 
                    gameObject.SetActive(true);
                    break;
            }
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