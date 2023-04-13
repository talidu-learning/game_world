using CustomInput;
using Enumerations;
using GameModes;
using Interactables;
using UnityEngine;

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
                GameModeSwitcher.SwitchGameMode.Invoke(GameMode.SelectedSocket);

            isOpen = !isOpen;
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
                    gameObject.SetActive(false);
                    break;
                case GameMode.Default:
                    if(isOpen)
                        CloseInventory();
                    gameObject.SetActive(true);
                    break;
                case GameMode.Placing:
                    if(isOpen)
                        CloseInventory();
                    break;
                case GameMode.Inventory:
                    OpenInventory();
                    break;
                case GameMode.DecoInventory:
                    gameObject.SetActive(false);
                    OpenInventory();
                    break;
                case GameMode.Shop:
                    gameObject.SetActive(false);
                    break;
                case GameMode.SelectedSocket:
                    gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        private void CloseInventory()
        {
            GameAudio.PlaySoundEvent.Invoke(SoundType.CloseInventory);
            Close();
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
            LeanTween.moveLocalY(InventoryUI, 0, 0.8f).setEase(LeanTweenType.easeOutElastic);
        }

        private void Close()
        {
            LeanTween.move(InventoryUI, new Vector2(Screen.width / 2, -Screen.height - 10f), 0.8f)
                .setEase(LeanTweenType.easeOutElastic);
        }
    }
}