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

        public void IsObjectSelected(bool isSelected, Interactable interactable)
        {
            if (isSelected)
            {
                isObjectSelected = isSelected;
                if (selectedObject != null && selectedObject != interactable)
                {
                    selectedObject.DisableDragging();
                    BuildingSystem.BuildingSystem.Current.PlaceObjectOnGrid();
                }
                selectedObject = interactable;

                if (interactable.gameObject.GetComponent<PlaceableObject>().Placed)
                {
                    var placeable = interactable.gameObject.GetComponent<PlaceableObject>();
                    BuildingSystem.BuildingSystem.Current.RemoveArea(placeable.placedPosition, placeable.Size);
                    BuildingSystem.BuildingSystem.Current.SetPlaceableObject(placeable);
                }
            }
            else
            {
                isObjectSelected = isSelected;
                selectedObject.DisableDragging();
                BuildingSystem.BuildingSystem.Current.PlaceObjectOnGrid();
            }
        }
    }
}