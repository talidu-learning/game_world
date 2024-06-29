using System;
using Enumerations;
using GameModes;
using Interactables;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using TouchScript.InputSources;
using UnityEngine;

namespace TouchScript.Examples.Camera
{
    /// <exclude />
    public class CameraController : MonoBehaviour
    {
        public ScreenTransformGesture TwoFingerMoveGesture;
        public ScreenTransformGesture ManipulationGesture;
        public float PanSpeed = 200f;

        [SerializeField]private Transform pivot;

        [SerializeField] private GameObject TouchManagerGameObject;
        private Transform cam;

        [SerializeField] private AnimationCurve rotation;
        [SerializeField] private AnimationCurve distance;

        [SerializeField] private Vector2 camZPosBorders = new Vector2(-50, 50);
        [SerializeField] private Vector2 camXPosBorders = new Vector2(-50, 50);

        public float ZoomSpeed = 10f;
        public float currentZoom;

        private bool isZoomEnabled = true;

        public void SetZoom(float amount)
        {
            currentZoom = amount;
            CalculateZoom();
        }
        
        private void Awake()
        {
            cam = UnityEngine.Camera.main.transform;
            currentZoom = 0.5f;
            CalculateZoom();
            ManipulationGesture.OnTransformComplete.AddListener(OnComplete);
        }

        private void Start()
        {
            GameModeSwitcher.OnSwitchedGameMode.AddListener(OnSwitchedGameModes);
        }

        private void OnSwitchedGameModes(GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.Default:
                    isZoomEnabled = true;
                    break;
                case GameMode.Placing:
                    isZoomEnabled = true;
                    break;
                case GameMode.Inventory:
                    isZoomEnabled = false;
                    break;
                case GameMode.Deco:
                    isZoomEnabled = true;
                    break;
                case GameMode.DecoInventory:
                    isZoomEnabled = false;
                    break;
                case GameMode.Shop:
                    isZoomEnabled = false;
                    break;
                case GameMode.SelectedSocket:
                    isZoomEnabled = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
            }
        }

        private void OnComplete(Gesture arg0)
        {
            var pointers = TouchManager.Instance.PressedPointers;

            foreach (var p in pointers)
            {
                TouchManager.Instance.CancelPointer(p.Id);
                TouchManager.Instance.RemoveInput(TouchManager.Instance.Inputs[0]);
                TouchManager.Instance.AddInput(TouchManagerGameObject.AddComponent<StandardInput>());
            }
        }

        private void Update()
        {
            if(!isZoomEnabled) return;
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                currentZoom += -Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
                currentZoom = Mathf.Clamp01(currentZoom);
                CalculateZoom();
            }
        }

        private void CalculateZoom()
        {
            if(!isZoomEnabled) return;
            var rotationX = rotation.Evaluate(currentZoom);
            var rotationY = cam.rotation.eulerAngles.y;
            Quaternion targetRot = Quaternion.Euler(rotationX, rotationY, 0.0f);
            
            cam.transform.rotation = targetRot;
            Vector3 offset = Vector3.forward * distance.Evaluate(currentZoom);
            Vector3 targetPos = pivot.position - targetRot * offset;
            cam.transform.position = targetPos;
        }

        private void OnEnable()
        {
            TwoFingerMoveGesture.Transformed += TwoFingerTransformHandler;
            ManipulationGesture.Transformed += ManipulationTransformedHandler;
        }

        private void OnDisable()
        {
            TwoFingerMoveGesture.Transformed -= TwoFingerTransformHandler;
            ManipulationGesture.Transformed -= ManipulationTransformedHandler;
        }

        private void ManipulationTransformedHandler(object sender, System.EventArgs e)
        {
            if(!isZoomEnabled) return;
            currentZoom += ManipulationGesture.DeltaScale - 1f;
            CalculateZoom();
        }

        private void TwoFingerTransformHandler(object sender, System.EventArgs e)
        {
            Vector3 newPos = pivot.rotation*TwoFingerMoveGesture.DeltaPosition*PanSpeed;
            newPos.z = -newPos.y;
            newPos.x = -newPos.x;
            newPos.y = 0;
            var newPosClamped = pivot.localPosition + newPos;
            newPosClamped.z = Mathf.Clamp(newPosClamped.z, camZPosBorders.x, camZPosBorders.y);
            newPosClamped.x = Mathf.Clamp(newPosClamped.x, camXPosBorders.x, camXPosBorders.y);
            pivot.localPosition = newPosClamped;
        }
    }
}