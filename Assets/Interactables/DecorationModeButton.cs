using System;
using CustomInput;
using Enumerations;
using GameModes;
using UnityEngine;
using UnityEngine.UI;

namespace Interactables
{
    public class DecorationModeButton : TaliduButton
    {
        [SerializeField] private Sprite DecoModeOn;
        [SerializeField] private Sprite DecoModeOff;

        private bool isModeEnabled = false;

        protected override void OnClickedButton()
        {
            ToggleDecoMode();
        }

        private void Start()
        {
            SelectionManager.SELECT_OBJECT_EVENT.AddListener(OnSelectedObject);
            SelectionManager.DESELECT_OBJECT_EVENT.AddListener(OnDeselectedObject);
            SelectionManager.DELETE_OBJECT_EVENT.AddListener(OnDeselectedObject);
            
            GameModeSwitcher.OnSwitchedGameMode.AddListener(OnSwitchedGameMode);
        }

        private void OnSwitchedGameMode(GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.Default:
                    gameObject.SetActive(true);
                    GetComponent<Image>().sprite = DecoModeOff;
                    break;
                case GameMode.Placing:
                    gameObject.SetActive(false);
                    break;
                case GameMode.Inventory:
                    gameObject.SetActive(false);
                    break;
                case GameMode.Deco:
                    gameObject.SetActive(true);
                    GetComponent<Image>().sprite = DecoModeOn;
                    break;
                case GameMode.DecoInventory:
                    gameObject.SetActive(false);
                    break;
                case GameMode.Shop:
                    gameObject.SetActive(false);
                    break;
                case GameMode.SelectedSocket:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
            }
        }

        private void OnDeselectedObject()
        {
            gameObject.SetActive(true);
        }

        private void OnSelectedObject(Interactable arg0)
        {
            gameObject.SetActive(false);
        }

        private void ToggleDecoMode()
        {
            if (isModeEnabled)
            {
                GameModeSwitcher.SwitchGameMode.Invoke(GameMode.Default);
            }
            else
            {
                GameModeSwitcher.SwitchGameMode.Invoke(GameMode.Deco);
            }

            isModeEnabled = !isModeEnabled;
        }
    }
}