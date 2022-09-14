using System.Collections.Generic;
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
        public float ZoomSpeed = 10f;

        [SerializeField] private float negativeXAxisBound = -10f;
        [SerializeField] private float positiveXAxisBound = 10f;
        [SerializeField] private float negativeZAxisBound = -10f;
        [SerializeField] private float positiveZAxisBound = 10f;

        [SerializeField]private Transform pivot;

        [SerializeField] private GameObject TouchManagerGameObject;
        private Transform cam;

        private IList<Pointer> twoFingerPointers = new List<Pointer>();

        private void Awake()
        {
            cam = Camera.main.transform;
            
            ManipulationGesture.OnTransformComplete.AddListener(OnComplete);
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
                Vector3 newZoom = cam.transform.localPosition + Vector3.up * Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
                newZoom.y = Mathf.Clamp(newZoom.y, 5.0f, 11.5f);
                cam.transform.localPosition = newZoom;
            }
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
            Vector3 newZoom = cam.transform.localPosition + Vector3.up * (ManipulationGesture.DeltaScale - 1f) * ZoomSpeed;
            newZoom.y = Mathf.Clamp(newZoom.y, 5.0f, 11.5f);
            cam.transform.localPosition = newZoom;
        }

        private void TwoFingerTransformHandler(object sender, System.EventArgs e)
        {
            Vector3 newPos = pivot.rotation*TwoFingerMoveGesture.DeltaPosition*PanSpeed;
            newPos.z = Mathf.Clamp(-newPos.y, negativeZAxisBound, positiveZAxisBound);
            newPos.x = Mathf.Clamp(-newPos.x, negativeXAxisBound, positiveXAxisBound);
            newPos.y = 0;
            pivot.localPosition += newPos;
        }
    }
}