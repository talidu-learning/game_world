using TouchScript.Gestures;
using UnityEngine;
using TouchScript.Gestures.TransformGestures;

namespace TouchScript.Examples.CameraControl
{
    /// <exclude />
    public class CameraController : MonoBehaviour
    {
        public ScreenTransformGesture TwoFingerMoveGesture;
        public ScreenTransformGesture ManipulationGesture;
        public float PanSpeed = 200f;
        public float ZoomSpeed = 10f;

        [SerializeField]private Transform pivot;
        private Transform cam;

        private void Awake()
        {
            cam = Camera.main.transform;
            
            ManipulationGesture.OnStateChange.AddListener(OnStateChanged);
        }


        private void OnStateChanged(Gesture gesture)
        {
            if (ManipulationGesture.State == Gesture.GestureState.Idle)
            {
                ManipulationGesture.Cancel();
            }

            if (ManipulationGesture.State == Gesture.GestureState.Ended)
            {
                ManipulationGesture.ActivePointers.Clear();
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
            newPos.z = newPos.y;
            newPos.x = -newPos.x;
            newPos.y = 0;
            pivot.localPosition += newPos;
        }
    }
}