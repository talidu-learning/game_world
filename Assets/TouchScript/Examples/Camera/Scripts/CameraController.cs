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
        public float RotationSpeed = 200f;
        public float ZoomSpeed = 10f;

        [SerializeField]private Transform pivot;
        private Transform cam;

        private void Awake()
        {
            cam = Camera.main.transform;
        }

        private void OnEnable()
        {
            TwoFingerMoveGesture.Transformed += twoFingerTransformHandler;
            ManipulationGesture.Transformed += manipulationTransformedHandler;
        }

        private void OnDisable()
        {
            TwoFingerMoveGesture.Transformed -= twoFingerTransformHandler;
            ManipulationGesture.Transformed -= manipulationTransformedHandler;
        }

        private void manipulationTransformedHandler(object sender, System.EventArgs e)
        {
            Debug.Log("transform");
            Vector3 newZoom = cam.transform.localPosition + Vector3.up * (ManipulationGesture.DeltaScale - 1f) * ZoomSpeed;
            newZoom.y = Mathf.Clamp(newZoom.y, 5.0f, 11.5f);
            cam.transform.localPosition = newZoom;
        }

        private void twoFingerTransformHandler(object sender, System.EventArgs e)
        {
            Vector3 newPos = pivot.rotation*TwoFingerMoveGesture.DeltaPosition*PanSpeed;
            newPos.z = newPos.y;
            newPos.x = -newPos.x;
            newPos.y = 0;
            pivot.localPosition += newPos;
        }
    }
}