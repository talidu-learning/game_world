using System.Collections.Generic;
using Enumerations;
using Interactables;
using Shop;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Filter
{
    public class Filter : MonoBehaviour
    {
        [SerializeField] private UIType UIType;
        [SerializeField] private List<ItemAttribute> SelectedItemAttributes;
        [SerializeField] private Image FilterImage;

        private bool isActivated = false;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(ToggleFilter);
        }

        private void ToggleFilter()
        {
            GameAudio.PlaySoundEvent.Invoke(SoundType.Filter);
            isActivated = !isActivated;

            gameObject.GetComponent<Image>().color = isActivated ? Color.grey : Color.white;
            FilterImage.color = isActivated ? new Color(1, 1, 1, 0.2f) : Color.white;

            UIManager.FILTER_EVENT.Invoke(UIType, SelectedItemAttributes, isActivated);
        }
    }
}