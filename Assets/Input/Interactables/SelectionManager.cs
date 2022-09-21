using System.Collections;
using BuildingSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class BoolInteractableUnityEvent : UnityEvent<Interactable> { }

    public class SelectionManager : MonoBehaviour
    {
        public static readonly BoolInteractableUnityEvent SELECT_OBJECT_EVENT = new BoolInteractableUnityEvent();
        public static readonly UnityEvent DESELECT_OBJECT_EVENT = new UnityEvent();
        public static readonly UnityEvent WITHDRAW_OBJECT_EVENT = new UnityEvent();

        private Interactable selectedObject;

        private bool preventNewObjectFromSpawning = false;

        private void Awake()
        {
            SELECT_OBJECT_EVENT.AddListener(SelectObject);
            DESELECT_OBJECT_EVENT.AddListener(DeselectObject);
            WITHDRAW_OBJECT_EVENT.AddListener(WithdrawObject);
        }

        private void WithdrawObject()
        {
            selectedObject = null;
            BuildingSystem.BuildingSystem.Current.WithdrawSelectedObject();
        }

        private void DeselectObject()
        {
            // can happen when withdraw button is pressed. Interference with OnTap from Interactable?
            if (!selectedObject) return;
            selectedObject.DisableDragging();
            selectedObject = null;
            BuildingSystem.BuildingSystem.Current.PlaceLastObjectOnGrid();
        }

        private void SelectObject(Interactable interactable)
        {
            Debug.Log(preventNewObjectFromSpawning);
            if(preventNewObjectFromSpawning) return;

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
                preventNewObjectFromSpawning = true;
                StartCoroutine(Wait());
                var go = BuildingSystem.BuildingSystem.Current.StartPlacingObjectOnGrid(interactable.gameObject);
                selectedObject = go.GetComponent<Interactable>();
            }
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(0.5f);
            preventNewObjectFromSpawning = false;
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