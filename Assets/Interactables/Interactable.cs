using System;
using Enumerations;
using GameModes;
using Shop;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;

namespace Interactables
{
    public class Interactable : MonoBehaviour
    {
        private PressGesture PressGesture;
        private TransformGesture TransformGesture;
        private TapGesture tapGesture;
        private bool isSelected = false;

        private void Awake()
        {
            tapGesture = GetComponent<TapGesture>();
            tapGesture.Tapped += TapGestureOnTapped;
            TransformGesture = GetComponent<TransformGesture>();
            TransformGesture.enabled = false;
            PressGesture = GetComponent<PressGesture>();
            PressGesture.Pressed += PressGestureOnPressed;
            DeselectOnTap.OnTapOnBackground.AddListener(OnTap);
            GameModeSwitcher.OnSwitchedGameMode.AddListener(OnSwitchedGameMode);
        }

        private void TapGestureOnTapped(object sender, EventArgs e)
        {
            EnableDragging();
            SelectionManager.SELECT_OBJECT_EVENT.Invoke(this);
        }

        private void OnSwitchedGameMode(GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.Deco:
                    DisableMoving();
                    break;
                case GameMode.Default:
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
                default:
                    EnableMoving();
                    break;
            }
        }

        private void EnableMoving()
        {
            tapGesture.enabled = true;
            PressGesture.enabled = true;
            GetComponent<BoxCollider>().enabled = true;
        }

        private void DisableMoving()
        {
            tapGesture.enabled = false;
            PressGesture.enabled = false;
            GetComponent<BoxCollider>().enabled = false;
        }


        private void PressGestureOnPressed(object sender, EventArgs e)
        {
            GameAudio.PlaySoundEvent.Invoke(SoundType.Select);
        }

        // private void LongPressedHandler(object sender, GestureStateChangeEventArgs e)
        // {
        //     if (e.State == Gesture.GestureState.Recognized)
        //     {
        //         EnableDragging();
        //         SelectionManager.SELECT_OBJECT_EVENT.Invoke(this);
        //     }
        //     else if (e.State == Gesture.GestureState.Failed)
        //     {
        //         gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        //         GameAudio.StopPLaying.Invoke();
        //     }
        // }

        public void EnableDragging()
        {
            isSelected = true;
            //LongPressGesture.enabled = false;
            PressGesture.enabled = false;
            TransformGesture.enabled = true;
            tapGesture.enabled = false;

            LeanTween.moveLocalY(gameObject, 1, 1).setLoopPingPong().setEase(LeanTweenType.easeInOutSine).setOnComplete(
                () => { gameObject.transform.position = new Vector3(transform.position.x, 0, transform.position.z); });
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
            isSelected = false;
            tapGesture.enabled = true;
            PressGesture.enabled = true;
            TransformGesture.enabled = false;
            
            LeanTween.cancelAll(true);
            DustParticleSystem.PlayDustPartclesEvent.Invoke(transform.position);
        }

        public void Flip()
        {
            // TODO Flip 3D-Objects
            if( GetComponentsInChildren<SpriteRenderer>().Length <=0)return;
            
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

            GetComponent<ItemID>().IsFlipped = !sprites[0].flipX;
            
            foreach (var sprite in sprites)
            {
                sprite.flipX = !sprite.flipX;
            }
        }
    }
}