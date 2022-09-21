using BuildingSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class BoolInteractableUnityEvent : UnityEvent<Interactable>
    {
    }

    public class SelectionManager : MonoBehaviour
    {
        public static readonly BoolInteractableUnityEvent SELECT_OBJECT_EVENT = new BoolInteractableUnityEvent();
        public static readonly UnityEvent DESELECT_OBJECT_EVENT = new UnityEvent();

        private Interactable selectedObject;

        private void Awake()
        {
            SELECT_OBJECT_EVENT.AddListener(SelectObject);
            DESELECT_OBJECT_EVENT.AddListener(DeselectObject);
        }

        private void DeselectObject()
        {
            selectedObject.DisableDragging();
            selectedObject = null;
            BuildingSystem.BuildingSystem.Current.PlaceLastObjectOnGrid();
        }

        private void SelectObject(Interactable interactable)
        {
            if (AnotherObjectIsSelected(interactable))
            {
                Debug.Log("SelectAnotherObject");
                DeselectObject();
            }


            if (ObjectWasPlacedBefore(interactable))
            {
                Debug.Log("Replace");
                BuildingSystem.BuildingSystem.Current.ReplaceObjectOnGrid(interactable.gameObject);
                selectedObject = interactable;
            }
            else
            {
                Debug.Log("New Object");
                var go = BuildingSystem.BuildingSystem.Current.StartPlacingObjectOnGrid(interactable.gameObject);
                selectedObject = go.GetComponent<Interactable>();
            }
        }

        private static bool ObjectWasPlacedBefore(Interactable interactable)
        {
            return interactable.gameObject.GetComponent<PlaceableObject>() &&
                   interactable.gameObject.GetComponent<PlaceableObject>().WasPlacedBefore;
        }

        private bool AnotherObjectIsSelected(Interactable interactable)
        {
            return selectedObject != null && selectedObject != interactable;
        }
    }
}