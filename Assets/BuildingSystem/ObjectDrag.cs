using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 offset;

    private void Awake()
    {
        Input.simulateMouseWithTouches = true;
    }

    private void OnMouseDown()
    {
        offset = transform.position - BuildingSystem.GetMousePositionWorld();
    }

    private void OnMouseDrag()
    {
        Vector3 pos = BuildingSystem.GetMousePositionWorld() + offset;
        transform.position = BuildingSystem.Current.SnapCoordinateToGrid(pos);
    }
}
