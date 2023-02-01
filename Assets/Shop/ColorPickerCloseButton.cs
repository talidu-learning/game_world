using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ColorPickerCloseButton : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClickedButton);
        }

        private void OnClickedButton()
        {
            UIManager.CloseColorPickerEvent.Invoke();
        }
    }
}