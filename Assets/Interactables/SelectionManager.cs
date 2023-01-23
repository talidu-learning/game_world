using System;
using BuildingSystem;
using Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class InteractableUnityEvent : UnityEvent<Interactable>
    {
    }

    public class SocketUnityEvent : UnityEvent<Socket>
    {
    }

    public class SelectionManager : MonoBehaviour
    {
        public static readonly InteractableUnityEvent SELECT_OBJECT_EVENT = new InteractableUnityEvent();
        public static readonly UnityEvent DESELECT_OBJECT_EVENT = new UnityEvent();
        public static readonly UnityEvent DELETE_OBJECT_EVENT = new UnityEvent();

        private Interactable selectedObject;

        public static readonly UnityEvent EnableDecoration = new UnityEvent();
        public static readonly UnityEvent DisableDecoration = new UnityEvent();

        public static readonly SocketUnityEvent SELECT_SOCKET_EVENT = new SocketUnityEvent();
        public static readonly SocketUnityEvent DESELECT_SOCKET_EVENT = new SocketUnityEvent();
        public static readonly SocketUnityEvent DELETE_SOCKET_EVENT = new SocketUnityEvent();

        private Socket selectedSocket;

        private Action serverCallback;

        private void Awake()
        {
            SELECT_OBJECT_EVENT.AddListener(SelectObject);
            DESELECT_OBJECT_EVENT.AddListener(DeselectObject);
            DELETE_OBJECT_EVENT.AddListener(DeleteObject);

            SELECT_SOCKET_EVENT.AddListener(SelectSocket);
            DESELECT_SOCKET_EVENT.AddListener(DeselectSocket);
            DELETE_SOCKET_EVENT.AddListener(DeleteSocket);
            DisableDecoration.AddListener(OnDisableDecoMode);
        }

        private void OnDisableDecoMode()
        {
            DeselectSocket(selectedSocket);
        }

        private void DeleteSocket(Socket socket)
        {
            socket.Deselect();
            selectedSocket = null;
        }

        private void DeselectSocket(Socket socket)
        {
            if (selectedSocket == null) return;
            Debug.Log("DeselectSocket");
            selectedSocket.Deselect();
            selectedSocket = null;
        }

        private void SelectSocket(Socket socket)
        {
            if (selectedSocket != null) selectedSocket.Deselect();
            selectedSocket = socket;
            selectedSocket.Select();
        }

        private void DeleteObject()
        {
            if (selectedSocket != null)
            {
                SocketPlacement.DeleteItemOnSocket.Invoke();
            }
            else
            {
                selectedObject = null;
                BuildingSystem.BuildingSystem.Current.DeleteSelectedObject();
            }
        }

        private void DeselectObject()
        {
            // can happen when delete button is pressed. Interference with OnTap from Interactable?
            if (!selectedObject) return;
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