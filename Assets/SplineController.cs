using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineController : MonoBehaviour
{
    private SplineContainer spline;
    void Start()
    {
        spline = GetComponent<SplineContainer>();

        Debug.Log(spline.EvaluatePosition(1));
        Debug.Log(spline.EvaluatePosition(2));
        Debug.Log(spline.EvaluatePosition(0.5f));
        Debug.Log(spline.EvaluatePosition(0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
