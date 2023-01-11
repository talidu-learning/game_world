using UnityEngine;

public class RotateSpriteToCamera : MonoBehaviour
{
    private Transform main;
    
    private void Start()
    {
        main = Camera.main.transform;
    }

    void Update()
    {
        // Vector3 camera = main.position;
        //
        // var direction = camera - transform.position;
        //
        // float angle = Vector3.Angle(direction, transform.position);
        //
        // float clampedAngle = Mathf.Clamp(angle, 0, 45);
        //
        // Quaternion rotation = Quaternion.Euler(clampedAngle, 0, 0);
        //
        // transform.SetPositionAndRotation(transform.position, rotation);

        var cameraRot = main.rotation;

        // float angle = Vector3.Angle(direction, transform.position);
        //
        // float clampedAngle = Mathf.Clamp(angle, 0, 90);
        //
        // Quaternion rotation = Quaternion.Euler(clampedAngle, 0, 0);
        //
        // transform.SetPositionAndRotation(transform.position, rotation);
    }
}
