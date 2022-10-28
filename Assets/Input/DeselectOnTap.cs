using System;
using Interactables;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.Events;

    public class DeselectOnTap : MonoBehaviour
    {
        public static readonly UnityEvent OnTapOnBackground = new UnityEvent();

        void Awake()
        {
            GetComponent<TapGesture>().Tapped += OnTapped;
            
            SelectionManager.SELECT_OBJECT_EVENT.AddListener(OnObjectSelected);
        }

        private void OnObjectSelected(Interactable interactable)
        {
            gameObject.GetComponent<MeshCollider>().enabled = true;
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
