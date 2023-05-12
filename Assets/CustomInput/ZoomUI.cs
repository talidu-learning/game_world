using System;
using Enumerations;
using GameModes;
using UnityEngine;

public class ZoomUI : MonoBehaviour
{
    void Start()
    {
        GameModeSwitcher.OnSwitchedGameMode.AddListener(OnSwitchedGameMode);
    }

    private void OnSwitchedGameMode(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.Default:
                gameObject.SetActive(true);
                break;
            case GameMode.Deco:
                gameObject.SetActive(true);
                break;
            case GameMode.DecoInventory:
                gameObject.SetActive(false);
                break;
            case GameMode.Shop:
                gameObject.SetActive(true);
                break;
            case GameMode.Placing:
                gameObject.SetActive(false);
                break;
            case GameMode.Inventory:
                gameObject.SetActive(false);
                break;
            case GameMode.SelectedSocket:
                gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
        }
    }
}
