using System;
using TouchScript;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Gestures.TransformGestures.Base;
using TouchScript.Utils.Attributes;
using UnityEngine;

namespace BuildingSystem
{
    /// <summary>
    /// Component which transforms an object according to events from transform gestures: <see cref="TransformGesture"/>, <see cref="ScreenTransformGesture"/>, <see cref="PinnedTransformGesture"/> and others.
    /// </summary>
    [AddComponentMenu("TouchScript/Behaviors/CustomTransformer")]
    public class CustomTransformer : MonoBehaviour
    {
        // Here's how it works.
        //
        // If smoothing is not enabled, the component just gets gesture events in stateChangedHandler(), passes Changed event to manualUpdate() which calls applyValues() to sett updated values.
        // The value of transformMask is used to only set values which were changed not to interfere with scripts changing this values.
        //
        // If smoothing is enabled — targetPosition, targetScale, targetRotation are cached and a lerp from current position to these target positions is applied every frame in update() method. It also checks transformMask to change only needed values.
        // If none of the delta values pass the threshold, the component transitions to idle state.

        #region Consts

        /// <summary>
        /// State for internal Transformer state machine.
        /// </summary>
        private enum TransformerState
        {
            /// <summary>
            /// Nothing is happening.
            /// </summary>
            Idle,

            /// <summary>
            /// The object is under manual control, i.e. user is transforming it.
            /// </summary>
            Manual,

            /// <summary>
            /// The object is under automatic control, i.e. it's being smoothly moved into target position when user lifted all fingers off.
            /// </summary>
            Automatic
        }

        #endregion

        #region Private variables

        [SerializeField] [ToggleLeft] private bool enableSmoothing = false;

        [SerializeField] private float smoothingFactor = 1f / 100000f;

        [SerializeField] private float positionThreshold = 0.01f;

        private TransformerState state;

        private TransformGestureBase gesture;
        private Transform cachedTransform;

        private TransformGesture.TransformType transformMask;
        private Vector3 targetPosition, targetScale;
        private Quaternion targetRotation;

        // last* variables are needed to detect when Transform's properties were changed outside of this script
        private Vector3 lastPosition, lastScale;
        private Quaternion lastRotation;

        #endregion

        #region Unity methods

        private void Awake()
        {
            cachedTransform = transform;
        }

        private void OnEnable()
        {
            gesture = GetComponent<TransformGestureBase>();
            gesture.StateChanged += stateChangedHandler;
            TouchManager.Instance.FrameFinished += frameFinishedHandler;

            stateIdle();
        }

        private void OnDisable()
        {
            if (gesture != null) gesture.StateChanged -= stateChangedHandler;
            if (TouchManager.Instance != null)
                TouchManager.Instance.FrameFinished -= frameFinishedHandler;

            stateIdle();
        }

        #endregion

        #region States

        private void stateIdle()
        {
            var prevState = state;
            setState(TransformerState.Idle);

            if (enableSmoothing && prevState == TransformerState.Automatic)
            {
                transform.position = lastPosition = targetPosition;
                var newLocalScale = lastScale = targetScale;
                // prevent recalculating colliders when no scale occurs
                if (newLocalScale != transform.localScale) transform.localScale = newLocalScale;
                transform.rotation = lastRotation = targetRotation;
            }

            transformMask = TransformGesture.TransformType.None;
        }

        private void stateManual()
        {
            setState(TransformerState.Manual);

            targetPosition = lastPosition = cachedTransform.position;
            targetRotation = lastRotation = cachedTransform.rotation;
            targetScale = lastScale = cachedTransform.localScale;
            transformMask = TransformGesture.TransformType.None;
        }

        private void stateAutomatic()
        {
            setState(TransformerState.Automatic);

            if (!enableSmoothing || transformMask == TransformGesture.TransformType.None) stateIdle();
        }

        private void setState(TransformerState newState)
        {
            state = newState;
        }

        #endregion

        #region Private functions

        private void update()
        {
            if (state == TransformerState.Idle) return;

            var fraction = 1 - Mathf.Pow(smoothingFactor, Time.unscaledDeltaTime);
            var changed = false;

            var pos = transform.position;

            transform.position = Vector3.Lerp(pos, targetPosition, fraction);
            // Something might have adjusted our position (most likely Unity UI).
            lastPosition = transform.position;

            if (state == TransformerState.Automatic && !changed &&
                (targetPosition - lastPosition).sqrMagnitude > positionThreshold) changed = true;

            if (state == TransformerState.Automatic && !changed) stateIdle();
        }

        private void manualUpdate()
        {
            if (state != TransformerState.Manual) stateManual();

            var mask = gesture.TransformMask;
            
            targetPosition += gesture.DeltaPosition;
            transformMask |= mask;

            gesture.OverrideTargetPosition(targetPosition);

            if (!enableSmoothing) applyValues();
        }

        private Vector3 offset;
        
        private void applyValues()
        {

        }

        #endregion

        #region Event handlers
        
        public static Vector3 GetPositionWorld(Vector2 pointerPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(pointerPosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                return raycastHit.point;
            }

            return Vector3.zero;
        }

        private void stateChangedHandler(object sender, GestureStateChangeEventArgs gestureStateChangeEventArgs)
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

        private void frameFinishedHandler(object sender, EventArgs eventArgs)
        {
            update();
        }

        #endregion
    }
}