using UnityEngine;

namespace BuildingSystem
{
    public class PlaceableObject : MonoBehaviour
    {
        public bool WasPlacedBefore { get; private set; }
        public Vector3Int Size { get; private set; }
        private Vector3[] vertices;
        public Vector3Int placedPosition{ get; private set; }

        public Vector3 placePosition{ get; private set; }

        private void Awake()
        {
            WasPlacedBefore = false;
        }

        private void GetColliderVertexPositionsLocal()
        {
            BoxCollider b = gameObject.GetComponent<BoxCollider>();
            vertices = new Vector3[4];
            vertices[0] = b.center + new Vector3(-b.size.x, -b.size.y, -b.size.z) * 0.5f;
            vertices[1]= b.center + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f;
            vertices[2]= b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f;
            vertices[3]= b.center + new Vector3(-b.size.x, -b.size.y, b.size.z) * 0.5f;
        }

        private void CalculateSizeInCells()
        {
            Vector3Int[] vector3Ints = new Vector3Int[vertices.Length];

            for (int i = 0; i < vector3Ints.Length; i++)
            {
                Vector3 worldPos = transform.TransformPoint(vertices[i]);
                vector3Ints[i] = BuildingSystem.Current.GridLayout.WorldToCell(worldPos);
            }

            Size = new Vector3Int(Mathf.Abs((vector3Ints[0] - vector3Ints[1]).x), Mathf.Abs((vector3Ints[0] - vector3Ints[3]).y), 1);
        }

        public Vector3 GetStartPosition()
        {
            return transform.TransformPoint(vertices[0]);
        }

        private void Start()
        {
            GetColliderVertexPositionsLocal();
            CalculateSizeInCells();
        }

        public virtual void Place(Vector3Int position)
        {

            placePosition = transform.position;
        
            placedPosition = position;
            WasPlacedBefore = true;
        
            // invoke events of placement
        }
    }
}
