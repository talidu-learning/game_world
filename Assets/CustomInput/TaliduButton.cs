using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CustomInput
{
    /// <summary>
    /// Prevents automatic double tapping on mobile devices
    /// </summary>
    public abstract class TaliduButton : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            if(button != null)
                button.onClick.AddListener(DoAction);
        }

        private void DoAction()
        {
            button.interactable = false;
            OnClickedButton();
            StartCoroutine(BlockButton());
        }

        private IEnumerator BlockButton()
        {
            yield return new WaitForSeconds(0.3f);
            button.interactable = true;
        }

        protected abstract void OnClickedButton();

        // ShopItem has Button on child object
        protected virtual void InitializeButton(Button btn)
        {
            button = btn;
            button.onClick.AddListener(DoAction);
        }
    }
}