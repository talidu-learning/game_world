using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ItemButton : MonoBehaviour
    {
        private Image image;

        void Awake()
        {
            image = GetComponent<Image>();
        }

        public void ChangeState(ShopItemButtonState state)
        {
            image = GetComponent<Image>();
            
            switch (state)
            {
                case ShopItemButtonState.Bought:
                    image.color = Color.green;
                    break;
                case ShopItemButtonState.Placed:
                    image.color = Color.gray;
                    break;
                default:
                    image.color = Color.red;
                    break;
            }
        }
    }
}
