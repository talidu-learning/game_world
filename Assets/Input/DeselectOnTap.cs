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
        }

        private void OnTapped(object sender, EventArgs e)
        {
            InvokeOnTap();
        }

        private void InvokeOnTap()
        {
            Debug.Log("tap");
            WebGLPluginJS.Browser_Log("Tap");
            OnTapOnBackground.Invoke();
        }
    }
