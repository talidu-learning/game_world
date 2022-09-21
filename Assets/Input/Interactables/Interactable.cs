using System;
using BuildingSystem;
using Interactables;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private LongPressGesture LongPressGesture;
    private PressGesture PressGesture;
    private TransformGesture TransformGesture;
    private bool isSelected = false;
    private ObjectDrag objectDrag;

    private void Awake()
    {
        objectDrag = gameObject.AddComponent<ObjectDrag>();
        objectDrag.enabled = false;
        
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
            SelectionManager.SELECT_OBJECT_EVENT.Invoke(this);
        }
        else if (e.State == Gesture.GestureState.Failed)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    public void EnableDragging()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        isSelected = true;
        LongPressGesture.enabled = false;
        PressGesture.enabled = false;
        objectDrag.enabled = true;
        //TransformGesture.enabled = true;
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
        gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        isSelected = false;
        LongPressGesture.enabled = true;
        PressGesture.enabled = true;
        objectDrag.enabled = false;
        //TransformGesture.enabled = false;
    }
    
}
