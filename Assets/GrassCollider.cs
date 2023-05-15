using UnityEngine;

public class GrassCollider : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        meshRenderer.enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        meshRenderer.enabled = true;
    }
}
