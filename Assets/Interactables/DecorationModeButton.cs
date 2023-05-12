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
            GameModeSwitcher.SwitchGameMode.Invoke(isModeEnabled ? GameMode.Default : GameMode.Deco);
            isModeEnabled = !isModeEnabled;
        }

        private void Start()
        {
            GameModeSwitcher.OnSwitchedGameMode.AddListener(OnSwitchedGameMode);
        }

        private void OnSwitchedGameMode(GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.Default:
                    if(FindObjectOfType<Socket>())
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
        
    }
}