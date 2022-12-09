using BuildingSystem;
using Inventory;
using ServerConnection;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class InteractableUnityEvent : UnityEvent<Interactable> { }
    public class SocketUnityEvent : UnityEvent<Socket> { }

    public class SelectionManager : MonoBehaviour
    {
        public static readonly InteractableUnityEvent SELECT_OBJECT_EVENT = new InteractableUnityEvent();
        public static readonly UnityEvent DESELECT_OBJECT_EVENT = new UnityEvent();
        public static readonly UnityEvent WITHDRAW_OBJECT_EVENT = new UnityEvent();

        private Interactable selectedObject;

        public static readonly UnityEvent EnableDecoration = new UnityEvent();
        public static readonly UnityEvent DisableDecoration = new UnityEvent();
        
        public static readonly SocketUnityEvent SELECT_SOCKET_EVENT = new SocketUnityEvent();
        public static readonly SocketUnityEvent DESELECT_SOCKET_EVENT = new SocketUnityEvent();
        public static readonly SocketUnityEvent WITHDRAW_SOCKET_EVENT = new SocketUnityEvent();
        
        private Socket selectedSocket;

        private void Awake()
        {
            SELECT_OBJECT_EVENT.AddListener(SelectObject);
            DESELECT_OBJECT_EVENT.AddListener(DeselectObject);
            WITHDRAW_OBJECT_EVENT.AddListener(WithdrawObject);
            
            SELECT_SOCKET_EVENT.AddListener(SelectSocket);
            DESELECT_SOCKET_EVENT.AddListener(DeselectSocket);
            WITHDRAW_SOCKET_EVENT.AddListener(WithdrawSocket);
        }

        private void WithdrawSocket(Socket socket)
        {
           socket.Deselect(); 
           selectedSocket = null;
        }

        private void DeselectSocket(Socket socket)
        {
            if(selectedSocket == null) return;
            selectedSocket.Deselect();
            selectedSocket = null;
        }

        private void SelectSocket(Socket socket)
        {
            if(selectedSocket != null) selectedSocket.Deselect();
            selectedSocket = socket;
            selectedSocket.Select();
            
        }

        private void WithdrawObject()
        {
            if (selectedSocket != null)
            {
                SocketPlacement.WithdrawItemOnSocket.Invoke();
            }
            else
            {
                selectedObject = null;
                BuildingSystem.BuildingSystem.Current.WithdrawSelectedObject();   
            }
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