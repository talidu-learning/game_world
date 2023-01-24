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
        transform.rotation = main.rotation;
    }
}