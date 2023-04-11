using System.Collections.Generic;
using Interactables;
using TouchScript.Gestures;
using UnityEngine;
using TouchScript.Gestures.TransformGestures;
using TouchScript.InputSources;
using TouchScript.Pointers;

namespace TouchScript.Examples.CameraControl
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

        private IList<Pointer> twoFingerPointers = new List<Pointer>();
        
        [SerializeField] private AnimationCurve rotation;
        [SerializeField] private AnimationCurve distance;
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
            cam = Camera.main.transform;
            currentZoom = 0.5f;
            CalculateZoom();
            ManipulationGesture.OnTransformComplete.AddListener(OnComplete);
        }

        private void Start()
        {
            // SelectionManager.SELECT_OBJECT_EVENT.AddListener(OnSelectedObject);
            // SelectionManager.DESELECT_OBJECT_EVENT.AddListener(OnDeselectedObject);
            // SelectionManager.DELETE_OBJECT_EVENT.AddListener(OnDeselectedObject);
        }

        private void OnDeselectedObject()
        {
            isZoomEnabled = true;
        }

        private void OnSelectedObject(Interactable interactable)
        {
            ZoomOut();
            isZoomEnabled = false;
        }

        private void ZoomOut()
        {
            currentZoom = 1;
            CalculateZoom();
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
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                // Vector3 newZoom = cam.transform.localPosition + Vector3.up * Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
                // newZoom.y = Mathf.Clamp(newZoom.y, 5.0f, 40.5f);
                // cam.transform.localPosition = newZoom;
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
            // var zoomFactor = Vector3.up * (ManipulationGesture.DeltaScale - 1f) * ZoomSpeed;
            // Vector3 localPosition = cam.transform.localPosition + zoomFactor;
            // localPosition.y = Mathf.Clamp(localPosition.y, 5.0f, 40.5f);
            // cam.transform.localPosition = localPosition;

            currentZoom += ManipulationGesture.DeltaScale - 1f;
            CalculateZoom();
        }

        private void TwoFingerTransformHandler(object sender, System.EventArgs e)
        {
            Vector3 newPos = pivot.rotation*TwoFingerMoveGesture.DeltaPosition*PanSpeed;
            newPos.z = -newPos.y;
            newPos.x = -newPos.x;
            newPos.y = 0;
            pivot.localPosition += newPos;
        }
    }
}