using Interactables;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ToggleShopButton : MonoBehaviour
    {
        [SerializeField] private ShopPanelTweening ShopPanel;
        [SerializeField] private Sprite ClosedPanel;
        [SerializeField] private Sprite OpenedPanel;

        private bool isOpen = false;
        
        void Start()
        {
            var button = GetComponent<Button>();
            
            button.onClick.AddListener(OnButtonClick);
            
            DecorationModeButton.ToggledDecoModeButtonEvent.AddListener(OnToggledDecoMode);
        }

        private void OnToggledDecoMode()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
        
        private void OnButtonClick()
        {
            //ShopPanel.gameObject.SetActive(!ShopPanel.gameObject.activeSelf);

            ShopPanel.Toggle();

            isOpen = !isOpen;

            if (isOpen)
            {
                GetComponent<Image>().sprite = OpenedPanel;
            }
            else
            {
                GetComponent<Image>().sprite = ClosedPanel;
            }
        }
    }
}
