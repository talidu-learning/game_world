using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures.Base;
using UnityEngine;

namespace BuildingSystem
{
    /// <summary>
    /// Moves object on grid according to data from an TransformGesture.
    /// </summary>
    [AddComponentMenu("TouchScript/Behaviors/CustomTransformer")]
    public class CustomTransformer : MonoBehaviour
    {
        private Vector3 offset;

        private TransformGestureBase gesture;

        private Vector3 targetPosition, targetScale;
        private Quaternion targetRotation;

        private Vector3 lastPosition;

        private void OnEnable()
        {
            gesture = GetComponent<TransformGestureBase>();
            gesture.StateChanged += StateChangedHandler;
        }

        private void OnDisable()
        {
            if (gesture != null) gesture.StateChanged -= StateChangedHandler;
        }

        private void StateChangedHandler(object sender, GestureStateChangeEventArgs gestureStateChangeEventArgs)
        {
            switch (gestureStateChangeEventArgs.State)
            {
                case Gesture.GestureState.Began:
                    offset = transform.position - GetPositionWorld(gesture.ActivePointers[0].Position);
                    break;
                case Gesture.GestureState.Possible:
                    break;
                case Gesture.GestureState.Changed:
                    transform.position = BuildingSystem.Current.SnapCoordinateToGrid(GetPositionWorld(gesture.ActivePointers[0].Position) + offset);
                    break;
                case Gesture.GestureState.Ended:
                    break;
                case Gesture.GestureState.Cancelled:
                    break;
                case Gesture.GestureState.Failed:
                case Gesture.GestureState.Idle:
                    
                    break;
            }
        }

        private static Vector3 GetPositionWorld(Vector2 pointerPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(pointerPosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                return raycastHit.point;
            }

            return Vector3.zero;
        }
    }
}