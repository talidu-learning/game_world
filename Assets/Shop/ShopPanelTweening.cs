using UnityEngine;

namespace Shop
{
    public class ShopPanelTweening : MonoBehaviour
    {
        private bool isOpen = false;

        private void Open()
        {
            LeanTween.moveX(gameObject, 0, 0.8f).setEase(LeanTweenType.easeOutElastic);
        }

        private void Close()
        {
            LeanTween.moveX(gameObject, -840.0f, 0.8f).setEase(LeanTweenType.easeOutElastic);
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