using System;
using UnityEngine;
using UnityEngine.UI;

namespace Interactables
{
    public class DecorationModeButton : MonoBehaviour
    {
        private bool isModeEnabled = false;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(ToggleDecoMode);
        }

        public void ToggleDecoMode()
        {
            if (isModeEnabled)
            {
                SelectionManager.DisableDecoration.Invoke();
            }
            else
            {
                SelectionManager.EnableDecoration.Invoke();
            }

            isModeEnabled = !isModeEnabled;
        }
    }
}