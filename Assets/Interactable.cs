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
    }
    

    private void PressGestureOnPressed(object sender, EventArgs e)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
    }

    private void LongPressedHandler(object sender, GestureStateChangeEventArgs e)
    {
        Debug.Log("Pressed");
        if (e.State == Gesture.GestureState.Recognized)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            isSelected = true;
            LongPressGesture.enabled = false;
            PressGesture.enabled = false;
            TransformGesture.enabled = true;
        }
        else if (e.State == Gesture.GestureState.Failed)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }
    
}
