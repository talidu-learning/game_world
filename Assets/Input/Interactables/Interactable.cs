using System;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private LongPressGesture LongPressGesture;
    private PressGesture PressGesture;
    private TransformGesture TransformGesture;
    private bool isSelected = false;

    private void Start()
    {
        TransformGesture = GetComponent<TransformGesture>();
        TransformGesture.enabled = false;
        
        LongPressGesture = GetComponent<LongPressGesture>();
        
        LongPressGesture.StateChanged += LongPressedHandler;

        PressGesture = GetComponent<PressGesture>();
        
        PressGesture.Pressed += PressGestureOnPressed;
        
        DeselectOnTap.OnTapOnBackground.AddListener(OnTap);
    }
    

    private void PressGestureOnPressed(object sender, EventArgs e)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
    }

    private void LongPressedHandler(object sender, GestureStateChangeEventArgs e)
    {
        if (e.State == Gesture.GestureState.Recognized)
        {
            EnableDragging();
            TapObjectsLayer.IsObjectSelectedEvent.Invoke(true, this);
        }
        else if (e.State == Gesture.GestureState.Failed)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    private void EnableDragging()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        isSelected = true;
        LongPressGesture.enabled = false;
        PressGesture.enabled = false;
        TransformGesture.enabled = true;
    }

    public void OnTap()
    {
        TapObjectsLayer.IsObjectSelectedEvent.Invoke(false, this);
        DisableDragging();
    }
    
    public void DisableDragging()
    {
        if (!isSelected) return;
        gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        isSelected = false;
        LongPressGesture.enabled = true;
        PressGesture.enabled = true;
        TransformGesture.enabled = false;
    }
    
}
