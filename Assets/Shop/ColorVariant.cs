using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ColorVariant : MonoBehaviour
    {
        public string VariantID;
        public ShopItem ShopItem;

        private void Awake()
        {
            GetComponentInChildren<ButtonPressAnimations>().OnClick.AddListener(OnSelectedVariant);
        }

        private void OnSelectedVariant()
        {
            ShopItem.SwitchItemVariant(VariantID);
        }

        public void SetColor(Color color)
        {
            GetComponentInChildren<Image>().color=color;
        }
    }
}