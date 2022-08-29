using System;
using Plugins.WebGL;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.Events;

    public class DeselectOnTap : MonoBehaviour
    {
        public static UnityEvent OnTapOnBackground = new UnityEvent();

        void Awake()
        {
            GetComponent<TapGesture>().Tapped += OnTapped;
            
            TapObjectsLayer.IsObjectSelectedEvent.AddListener(OnObjectSelected);
        }

        private void OnObjectSelected(bool isSelected, Interactable interactable)
        {
            if (isSelected)
            {
                gameObject.layer = LayerMask.NameToLayer("Interactables");
            }
        }

        private void OnTapped(object sender, EventArgs e)
        {
            InvokeOnTap();
        }

        private void InvokeOnTap()
        {
            Debug.Log("tap");
            WebGLPluginJS.Browser_Log("Tap");
            gameObject.layer = LayerMask.NameToLayer("Default");
            OnTapOnBackground.Invoke();
        }
    }
