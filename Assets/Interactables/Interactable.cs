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
            if (GetComponentInChildren<MeshRenderer>())
            {
                var mesh = GetComponentInChildren<MeshFilter>().mesh;
                FlipMesh(mesh);
                var itemId = GetComponent<ItemID>();
                itemId.IsFlipped = !itemId.IsFlipped;
                return;
            }

            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

            GetComponent<ItemID>().IsFlipped = !sprites[0].flipX;
            
            foreach (var sprite in sprites)
            {
                sprite.flipX = !sprite.flipX;
            }
        }

        private static void FlipMesh(Mesh mesh)
        {
            Vector3[] verts = mesh.vertices;
            for (int i = 0; i < verts.Length; i++)
            {
                Vector3 c = verts[i];
                c.x *= -1;
                verts[i] = c;
            }

            mesh.vertices = verts;
            FlipNormals(mesh);
        }

        private static void FlipNormals(Mesh mesh)
        {
            int[] tris = mesh.triangles;
            for (int i = 0; i < tris.Length / 3; i++)
            {
                int a = tris[i * 3 + 0];
                int b = tris[i * 3 + 1];
                int c = tris[i * 3 + 2];
                tris[i * 3 + 0] = c;
                tris[i * 3 + 1] = b;
                tris[i * 3 + 2] = a;
            }
            mesh.triangles = tris;
        }
    }
}