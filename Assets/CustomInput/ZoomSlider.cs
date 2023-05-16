using System;
using Enumerations;
using GameModes;
using TouchScript.Examples.CameraControl;
using UnityEngine;
using UnityEngine.UI;

namespace CustomInput
{
    public class ZoomSlider : MonoBehaviour
    {
        [SerializeField] private Slider Slider;
        [SerializeField] private Image ZoomBackground;

        [SerializeField] private CameraController CameraController;

        void Start()
        {
            Slider.onValueChanged.AddListener(ChangeZoom);
            GameModeSwitcher.OnSwitchedGameMode.AddListener(OnChangedGameMode);
        }

        private void OnChangedGameMode(GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.Default:
                    DisplayZoomSlider(true);
                    break;
                case GameMode.Placing:
                    DisplayZoomSlider(true);
                    break;
                case GameMode.Inventory:
                    DisplayZoomSlider(false);
                    break;
                case GameMode.Deco:
                    DisplayZoomSlider(true);
                    break;
                case GameMode.DecoInventory:
                    DisplayZoomSlider(false);
                    break;
                case GameMode.Shop:
                    DisplayZoomSlider(false);
                    break;
                case GameMode.SelectedSocket:
                    DisplayZoomSlider(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
            }
        }

        private void DisplayZoomSlider(bool active)
        {
            ZoomBackground.enabled = active;

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(active);
            }
        }

        private Color ChangeAlpha(float amount, Color color)
        {
            var tempColor = color;
            tempColor.a = amount;
            return tempColor;
        }

        private void ChangeZoom(float amount)
        {
            CameraController.SetZoom(amount);
        }

        private void LateUpdate()
        {
            Slider.value = CameraController.currentZoom;
        }
    }
}
