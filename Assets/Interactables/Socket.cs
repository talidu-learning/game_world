using System;
using Enumerations;
using GameModes;
using TouchScript.Gestures;
using UnityEngine;

namespace Interactables
{
    public class Socket : MonoBehaviour
    {
        public bool IsUsed;
        public Guid Uid;

        private BoxCollider boxCollider;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.enabled = false;
            GameModeSwitcher.OnSwitchedGameMode.AddListener(OnSwitchedGameMode);
            GetComponent<PressGesture>().Pressed += OnTap;
        }

        private void OnSwitchedGameMode(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.Deco:
                    boxCollider.enabled = true;
                    break;
                default:
                    boxCollider.enabled = false;
                    break;
            }
        }

        private void OnTap(object sender, EventArgs e)
        {
            SelectionManager.SELECT_SOCKET_EVENT.Invoke(this);
        }

        public void Select()
        {
            GetComponent<SpriteRenderer>().color = Color.green;
        }

        public void Deselect()
        {
            GetComponent<SpriteRenderer>().color = IsUsed ? Color.red : Color.white;
        }

        public void Place(Guid uid)
        {
            IsUsed = true;
            Uid = uid;
            GetComponent<SpriteRenderer>().color = Color.red;
        }

        public void Delete()
        {
            Destroy(transform.GetChild(0).gameObject);
            IsUsed = false;
        }
    }
}