using System;
using Enumerations;
using GameModes;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;

namespace Interactables
{
    public class Interactable : MonoBehaviour
    {
        private LongPressGesture LongPressGesture;
        private PressGesture PressGesture;
        private TransformGesture TransformGesture;
        private bool isSelected = false;

        private void Awake()
        {
            TransformGesture = GetComponent<TransformGesture>();
            TransformGesture.enabled = false;
            LongPressGesture = GetComponent<LongPressGesture>();
            LongPressGesture.StateChanged += LongPressedHandler;
            PressGesture = GetComponent<PressGesture>();
            PressGesture.Pressed += PressGestureOnPressed;
            DeselectOnTap.OnTapOnBackground.AddListener(OnTap);
            GameModeSwitcher.OnSwitchedGameMode.AddListener(OnSwitchedGameMode);
        }

        private void OnSwitchedGameMode(GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.Deco:
                    DisableMoving();
                    break;
                case GameMode.Terrain:
                    DisableMoving();
                    break;
                default:
                    EnableMoving();
                    break;
            }
        }

        private void EnableMoving()
        {
            LongPressGesture.enabled = true;
            PressGesture.enabled = true;
            GetComponent<BoxCollider>().enabled = true;
        }

        private void DisableMoving()
        {
            LongPressGesture.enabled = false;
            PressGesture.enabled = false;
            GetComponent<BoxCollider>().enabled = false;
        }


        private void PressGestureOnPressed(object sender, EventArgs e)
        {
            gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.grey;
            GameAudio.PlaySoundEvent.Invoke(SoundType.Select);
        }

        private void LongPressedHandler(object sender, GestureStateChangeEventArgs e)
        {
            if (e.State == Gesture.GestureState.Recognized)
            {
                EnableDragging();
                SelectionManager.SELECT_OBJECT_EVENT.Invoke(this);
            }
            else if (e.State == Gesture.GestureState.Failed)
            {
                gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
                GameAudio.StopPLaying.Invoke();
            }
        }

        public void EnableDragging()
        {
            gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.green;
            isSelected = true;
            LongPressGesture.enabled = false;
            PressGesture.enabled = false;
            TransformGesture.enabled = true;
        }

        private void OnTap()
        {
            if (!isSelected) return;
            SelectionManager.DESELECT_OBJECT_EVENT.Invoke();
            DisableDragging();
        }

        public void DisableDragging()
        {
            if (!isSelected) return;
            gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            isSelected = false;
            LongPressGesture.enabled = true;
            PressGesture.enabled = true;
            TransformGesture.enabled = false;
        }

        public void Flip()
        {
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

            if (sprites == null) return;
            foreach (var sprite in sprites)
            {
                sprite.flipX = !sprite.flipX;
            }
        }
    }
}