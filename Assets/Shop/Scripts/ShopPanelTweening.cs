using UnityEngine;

namespace Shop
{
    public class ShopPanelTweening : MonoBehaviour
    {
        private bool isOpen = false;
        
        public void Open()
        {
            LeanTween.moveX(gameObject, 0, 0.8f).setEase(LeanTweenType.easeOutElastic);
        }

        public void Close()
        {
            LeanTween.moveX(gameObject, -700.0f, 0.8f).setEase(LeanTweenType.easeOutElastic);
        }

        public void Toggle()
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }

            isOpen = !isOpen;
        }
    }
}