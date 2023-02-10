using CustomInput;
using Enumerations;
using GameModes;
using Inventory;
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
            ToggleInventoryButton.OpenedInventoryEvent.AddListener(() => gameObject.SetActive(false));
            ToggleInventoryButton.ClosedInventoryUnityEvent.AddListener(() => gameObject.SetActive(true));

            SelectionManager.SELECT_OBJECT_EVENT.AddListener(OnSelectedObject);
            SelectionManager.DESELECT_OBJECT_EVENT.AddListener(OnDeselectedObject);
            SelectionManager.DELETE_OBJECT_EVENT.AddListener(OnDeselectedObject);
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
                GetComponent<Image>().sprite = DecoModeOff;
                GameModeSwitcher.SwitchGameMode.Invoke(GameMode.DefaultPlacing);
            }
            else
            {
                GetComponent<Image>().sprite = DecoModeOn;
                GameModeSwitcher.SwitchGameMode.Invoke(GameMode.Deco);
            }

            isModeEnabled = !isModeEnabled;
        }
    }
}