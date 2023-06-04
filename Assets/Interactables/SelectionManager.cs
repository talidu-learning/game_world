using System;
using BuildingSystem;
using Enumerations;
using GameModes;
using Inventory;
using UnityEngine;
using UnityEngine.Events;
using VFX;

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
        [SerializeField] private HighlightParticlesManager highlightParticlesManager;
        public static readonly InteractableUnityEvent SELECT_OBJECT_EVENT = new InteractableUnityEvent();
        public static readonly UnityEvent DESELECT_OBJECT_EVENT = new UnityEvent();
        public static readonly UnityEvent FLIP_OBJECT_EVENT = new UnityEvent();
        public static readonly UnityEvent DELETE_OBJECT_EVENT = new UnityEvent();

        private Interactable selectedObject;
        private Material selectionMaterial2D;
        private Material selectionMaterial3D;
        private Material lastSwappedMaterial;
        private SpriteRenderer currentRenderer;

        public static readonly SocketUnityEvent SELECT_SOCKET_EVENT = new SocketUnityEvent();
        public static readonly SocketUnityEvent DESELECT_SOCKET_EVENT = new SocketUnityEvent();
        public static readonly SocketUnityEvent DELETE_SOCKET_EVENT = new SocketUnityEvent();

        private Socket selectedSocket;

        private Action serverCallback;
        private GameMode currentMode = GameMode.Default;

        private void Awake()
        {
            SELECT_OBJECT_EVENT.AddListener(SelectObject);
            DESELECT_OBJECT_EVENT.AddListener(DeselectObject);
            DELETE_OBJECT_EVENT.AddListener(DeleteObject);
            FLIP_OBJECT_EVENT.AddListener(OnFlipObject);

            SELECT_SOCKET_EVENT.AddListener(SelectSocket);
            DESELECT_SOCKET_EVENT.AddListener(DeselectSocket);
            DELETE_SOCKET_EVENT.AddListener(DeleteSocket);
        }

        private void OnFlipObject()
        {
            selectedObject.Flip();
        }

        private void Start()
        {
            GameModeSwitcher.OnSwitchedGameMode.AddListener(OnSwitchedGameModes);
            selectionMaterial2D = Resources.Load<Material>("2DSelectionMaterial");
            selectionMaterial3D = Resources.Load<Material>("3DSelectionMaterial");
        }

        private void OnSwitchedGameModes(GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.Deco:
                    DeselectSocket(selectedSocket);
                    break;
                case GameMode.Default:
                    if(currentMode == GameMode.Deco) OnDisableDecoMode();
                    break;
                case GameMode.Placing:
                    break;
                case GameMode.Inventory:
                    break;
                case GameMode.DecoInventory:
                    break;
                case GameMode.Shop:
                    break;
                case GameMode.SelectedSocket:
                    break;
            }
            currentMode = gameMode;
        }

        private void OnDisableDecoMode()
        {
            DeselectSocket(selectedSocket);
        }

        private void DeleteSocket(Socket socket)
        {
            selectedSocket = null;
        }

        private void DeselectSocket(Socket socket)
        {
            if (selectedSocket == null) return;
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
            highlightParticlesManager.StopParticleSystem();
            
            if (currentMode != GameMode.Placing)
            {
                SocketPlacement.DeleteItemOnSocket.Invoke();
            }
            else
            {
                selectedObject = null;
                BuildingSystem.BuildingSystem.Current.DeleteSelectedObject();
                GameModeSwitcher.SwitchGameMode.Invoke(GameMode.Default);
            }
        }

        private void DeselectObject()
        {
            highlightParticlesManager.StopParticleSystem();

            if (currentRenderer) currentRenderer.material = lastSwappedMaterial;

            GameModeSwitcher.SwitchGameMode.Invoke(GameMode.Default);
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
            highlightParticlesManager.MoveParticleSystem(interactable.gameObject);


            currentRenderer = interactable.transform.GetComponentInChildren<SpriteRenderer>() ?? null ;
            lastSwappedMaterial = currentRenderer?.material;
            if(currentRenderer) currentRenderer.material = selectionMaterial2D;

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
            
            GameModeSwitcher.SwitchGameMode.Invoke(GameMode.Placing);
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