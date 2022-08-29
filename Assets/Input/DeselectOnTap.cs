using System;
using Interactables;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.Events;

    public class DeselectOnTap : MonoBehaviour
    {
        public static UnityEvent OnTapOnBackground = new UnityEvent();

        void Awake()
        {
            GetComponent<TapGesture>().Tapped += OnTapped;
            
            SelectionManager.IsObjectSelectedEvent.AddListener(OnObjectSelected);
        }

        private void OnObjectSelected(bool isSelected, Interactable interactable)
        {
            if (isSelected)
            {
                gameObject.GetComponent<MeshCollider>().enabled = true;
            }
        }

        private void OnTapped(object sender, EventArgs e)
        {
            InvokeOnTap();
        }

        private void InvokeOnTap()
        {
            gameObject.GetComponent<MeshCollider>().enabled = false;
            OnTapOnBackground.Invoke();
        }
    }
