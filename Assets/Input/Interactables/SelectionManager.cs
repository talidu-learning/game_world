using UnityEngine;

namespace Interactables
{
    public class SelectionManager : MonoBehaviour
    {
        public static BoolUnityEvent IsObjectSelectedEvent = new BoolUnityEvent();
    
        private bool isObjectSelected = false;

        private Interactable selectedObject;

        private void Awake()
        {
            IsObjectSelectedEvent.AddListener(IsObjectSelected);
        }

        public void IsObjectSelected(bool isSelected, Interactable gameObject)
        {
            if (isSelected)
            {
                isObjectSelected = isSelected;
                if(selectedObject != null && selectedObject !=gameObject)
                    selectedObject.DisableDragging();
                selectedObject = gameObject;
            }
            else
            {
                isObjectSelected = isSelected;
                selectedObject.DisableDragging();
            }
        }
    }
}