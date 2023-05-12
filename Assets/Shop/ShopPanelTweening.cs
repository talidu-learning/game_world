using UnityEngine;

namespace Shop
{
    public class ShopPanelTweening : MonoBehaviour
    {
        private bool isOpen = false;
        public RectTransform ShopPanel;
        public Canvas Canvas;

        private void Open()
        {
            LeanTween.moveX(gameObject, 0, 0.4f).setEaseOutExpo();
        }

        private void Close()
        {
            // mach das hier fenstergrößenabhänging
            LeanTween.moveX(gameObject,
                    -ShopPanel.transform.GetComponent<RectTransform>().sizeDelta.x * Canvas.scaleFactor, 0.1f)
                .setEase(LeanTweenType.easeInCirc);
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