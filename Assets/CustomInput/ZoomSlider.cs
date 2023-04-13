using System;
using Enumerations;
using GameModes;
using Interactables;
using TouchScript.Examples.CameraControl;
using UnityEngine;
using UnityEngine.UI;

public class ZoomSlider : MonoBehaviour
{
    private Slider slider;

    [SerializeField] private CameraController cameraController;
    [SerializeField] private Image handleImage;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(ChangeZoom);
        SelectionManager.SELECT_OBJECT_EVENT.AddListener(OnSelectedObject);
        SelectionManager.DESELECT_OBJECT_EVENT.AddListener(OnDeselectedObject);
        SelectionManager.DELETE_OBJECT_EVENT.AddListener(OnDeselectedObject);
        
        GameModeSwitcher.OnSwitchedGameMode.AddListener(OnChangedGameMode);
            
    }

    private void OnChangedGameMode(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.Default:
                break;
            case GameMode.Placing:
                break;
            case GameMode.Inventory:
                break;
            case GameMode.Deco:
                break;
            case GameMode.DecoInventory:
                break;
            case GameMode.Shop:
                break;
            case GameMode.SelectedSocket:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
        }
    }

    private void OnDeselectedObject()
    {
        slider.interactable = true;
        handleImage.color = ChangeAlpha(1f);
    }

    private Color ChangeAlpha(float amount)
    {
        var tempColor = handleImage.color;
        tempColor.a = amount;
        return tempColor;
    }

    private void OnSelectedObject(Interactable interactable)
    {
        slider.interactable = false;
        handleImage.color = ChangeAlpha(0.4f);
    }

    private void ChangeZoom(float amount)
    {
        cameraController.SetZoom(amount);
    }

    private void LateUpdate()
    {
        slider.value = cameraController.currentZoom;
    }
}
