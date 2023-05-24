using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveEvent : MonoBehaviour
{
    public delegate void ECameraMove();
    public static event ECameraMove OnMoved;
    private Vector3 lastFramePos = new Vector3();

    void FixedUpdate()
    {
        if (transform.position != lastFramePos)
        {
            if(OnMoved != null)
            OnMoved();
        }
    }
}
