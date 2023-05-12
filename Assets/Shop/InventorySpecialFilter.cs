using System.Collections.Generic;
using Enumerations;
using Interactables;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class InventorySpecialFilter : MonoBehaviour
    {
        [SerializeField] private UIType UIType;
        [SerializeField] private Image FilterImage;
        [SerializeField] private Sprite FilterImageDefault;
        [SerializeField] private Sprite FilterImageBought;
        [SerializeField] private Sprite FilterImageNotBought;
        [SerializeField] private Image FilterImageShadow;
        [SerializeField] private Sprite FilterImageShadowDefault;
        [SerializeField] private Sprite FilterImageShadowBought;
        [SerializeField] private Sprite FilterImageShadowNotBought;

        private bool isActivated = false;

        private ButtonStates currentState;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(ToggleFilter);
        }

        private void ToggleFilter()
        {
            GameAudio.PlaySoundEvent.Invoke(SoundType.Filter);
            isActivated = !isActivated;

            switch (currentState)
            {
                case ButtonStates.Default:
                    UIManager.FILTER_EVENT.Invoke(UIType, new List<ItemAttribute> { ItemAttribute.Bought }, true);
                    FilterImage.sprite = FilterImageBought;
                    FilterImageShadow.sprite = FilterImageShadowBought;
                    currentState = ButtonStates.Bought;
                    break;
                case ButtonStates.Bought:
                    UIManager.FILTER_EVENT.Invoke(UIType, new List<ItemAttribute> { ItemAttribute.Bought }, false);
                    UIManager.FILTER_EVENT.Invoke(UIType, new List<ItemAttribute> { ItemAttribute.NotBought }, true);
                    FilterImage.sprite = FilterImageNotBought;
                    FilterImageShadow.sprite = FilterImageShadowNotBought;
                    currentState = ButtonStates.NotBought;
                    break;
                case ButtonStates.NotBought:
                    UIManager.FILTER_EVENT.Invoke(UIType, new List<ItemAttribute> { ItemAttribute.NotBought }, false);
                    FilterImage.sprite = FilterImageDefault;
                    FilterImageShadow.sprite = FilterImageShadowDefault;
                    currentState = ButtonStates.Default;
                    break;
            }
        }

        private enum ButtonStates
        {
            Default,
            Bought,
            NotBought
        }
    }
}
