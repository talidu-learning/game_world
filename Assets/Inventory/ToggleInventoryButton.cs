using CustomInput;
using Enumerations;
using GameModes;
using Interactables;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class ToggleInventoryButton : TaliduButton
    {

        [SerializeField] private GameObject InventoryUI;

        private bool isOpen;
        
        protected override void OnClickedButton()
        {
            if(GameModeSwitcher.currentGameMode == GameMode.SelectedSocket)
                GameModeSwitcher.SwitchGameMode.Invoke(GameMode.DecoInventory);
            
            else if(GameModeSwitcher.currentGameMode == GameMode.Default)
                GameModeSwitcher.SwitchGameMode.Invoke(GameMode.Inventory);
            
            else if(GameModeSwitcher.currentGameMode == GameMode.Inventory)
                GameModeSwitcher.SwitchGameMode.Invoke(GameMode.Default);
            
            else if(GameModeSwitcher.currentGameMode == GameMode.DecoInventory)
                GameModeSwitcher.SwitchGameMode.Invoke(GameMode.Deco);
            
        }

        public void ToggleButton()
        {
            DoAction();
        }

        private void Start()
        {
            GameModeSwitcher.OnSwitchedGameMode.AddListener(OnSwitchedGameModes);
        }

        private void OnSwitchedGameModes(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.Deco:
                    if(isOpen)
                        CloseInventory(false);
                    else
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                case GameMode.Default:
                    if(isOpen)
                        CloseInventory(true);
                    else
                    {
                        gameObject.SetActive(true);
                    }
                    break;
                case GameMode.Placing:
                    if(isOpen)
                        CloseInventory(true);
                    gameObject.SetActive(false);
                    break;
                case GameMode.Inventory:
                    OpenInventory();
                    break;
                case GameMode.DecoInventory:
                    OpenInventory();
                    break;
                case GameMode.Shop:
                    gameObject.SetActive(false);
                    break;
                case GameMode.SelectedSocket:
                    ActivateInventoryButton();
                    break;
            }
        }

        private void CloseInventory(bool reactivateButton)
        {
            GameAudio.PlaySoundEvent.Invoke(SoundType.CloseInventory);
            Close(reactivateButton);
        }

        private void OpenInventory()
        {
            GameAudio.PlaySoundEvent.Invoke(SoundType.OpenInventory);
            if(GameModeSwitcher.currentGameMode == GameMode.Default)
                SelectionManager.DESELECT_OBJECT_EVENT.Invoke();
            Open();
        }

        private void Open()
        {
            isOpen = true;
            gameObject.GetComponent<Image>().enabled = false;
            LeanTween.moveLocalY(InventoryUI, 0, 0.8f).setEase(LeanTweenType.easeOutElastic).setOnComplete(()=> gameObject.SetActive(false));
        }

        private void ActivateInventoryButton()
        {
            gameObject.SetActive(true);
            gameObject.GetComponent<Image>().enabled = true;
        }
        
        private void Close(bool reactivateButton)
        {
            isOpen = false;
            gameObject.SetActive(true);
            LeanTween.move(InventoryUI, new Vector2(Screen.width / 2, -Screen.height - 10f), 0.8f)
                .setEase(LeanTweenType.easeOutElastic)
                .setOnComplete(() =>
                {
                    gameObject.GetComponent<Image>().enabled = true;
                    gameObject.SetActive(reactivateButton);
                });
        }
    }
}