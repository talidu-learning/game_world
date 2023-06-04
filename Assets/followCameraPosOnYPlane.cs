using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followCameraPosOnYPlane : MonoBehaviour
{

    public float Drag = 1f;
    public Vector3 Offset = new Vector3();
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }
    void OnEnable()
    {
        CameraMoveEvent.OnMoved += UpdateCamRelatedPosition;
    }
    void OnDisable()
    {
        CameraMoveEvent.OnMoved -= UpdateCamRelatedPosition;
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        while (time < duration)
        {
            float t = time / duration;
            t = t * t * (3f - 2f * t);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
    

    public void UpdateCamRelatedPosition()
    {
        StopCoroutine("LerpPosition");
        StartCoroutine(LerpPosition(new Vector3(mainCam.transform.position.x + Offset.x, 0 + Offset.y, mainCam.transform.position.z + Offset.z),Drag));
    }
}
