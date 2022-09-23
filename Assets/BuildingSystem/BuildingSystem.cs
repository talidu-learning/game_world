using UnityEngine;
using UnityEngine.Tilemaps;

namespace BuildingSystem
{
    /// <summary>
    /// Places GameObjects on a Tilemap. Singleton.
    /// </summary>
    public class BuildingSystem : MonoBehaviour
    {
        public static BuildingSystem Current;
        public GridLayout GridLayout;

        // Tilemaps
        [SerializeField] private Tilemap PlacingTilemap;
        [SerializeField] private Tilemap VisibleTilemap;
        [SerializeField] private Tilemap ValidPlacingTilemap;
        
        [SerializeField] private TileBase TileBase;

        private Grid grid;
        private PlaceableObject placeableObject;
        private TilemapRenderer visibleMapRenderer;

        private void Awake()
        {
            Current = this;
            grid = GridLayout.gameObject.GetComponent<Grid>();
            visibleMapRenderer = VisibleTilemap.GetComponent<TilemapRenderer>();
            visibleMapRenderer.enabled = false;
        }

        public void WithdrawSelectedObject()
        {
            if (!placeableObject) return;
            if (placeableObject.WasPlacedBefore)
            {
                RemoveArea(placeableObject.placedPosition, placeableObject.Size);
            }
            Destroy(placeableObject.gameObject);
            placeableObject = null;
            visibleMapRenderer.enabled = false;
        }

        public void PlaceLastObjectOnGrid()
        {
            if (CanBePlaced(placeableObject))
            {
                Vector3Int start = GridLayout.WorldToCell(placeableObject.GetStartPosition());
                placeableObject.Place(start);
                TakeArea(start, placeableObject.Size, TileBase);
            }
            else
            {
                if (placeableObject.WasPlacedBefore)
                {
                    placeableObject.transform.position = placeableObject.placePosition;
                    placeableObject.Place(placeableObject.placedPosition);
                    TakeArea(placeableObject.placedPosition, placeableObject.Size, TileBase);
                    return;
                }
                Destroy(placeableObject.gameObject);
            }

            visibleMapRenderer.enabled = false;

            placeableObject = null;
        }

        public GameObject StartPlacingObjectOnGrid(GameObject objectToPlace)
        {
            GameObject go = SpawnObjectOnGrid(objectToPlace);
            InitializePlacing(go);
            visibleMapRenderer.enabled = true;
            return go;
        }

        public void ReplaceObjectOnGrid(GameObject objectToReplace)
        {
            var placeable = objectToReplace.GetComponent<PlaceableObject>();
            RemoveArea(placeable.placedPosition, placeable.Size);
            InitializePlacing(objectToReplace);
            visibleMapRenderer.enabled = true;
        }

        public static Vector3 GetMousePositionWorld()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                return raycastHit.point;
            }

            return Vector3.zero;
        }

        public Vector3 SnapCoordinateToGrid(Vector3 position)
        {
            Vector3Int cellPos = GridLayout.WorldToCell(position);
            position = grid.GetCellCenterWorld(cellPos);
            return position;
        }

        #region private

        private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
        {
            TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
            int counter = 0;

            foreach (var v in area.allPositionsWithin)
            {
                Vector3Int pos = new Vector3Int(v.x, v.y, 0);
                array[counter] = tilemap.GetTile(pos);
                counter++;
            }

            return array;
        }

        private void InitializePlacing(GameObject objectToPLace)
        {
            placeableObject = objectToPLace.GetComponent<PlaceableObject>();
            objectToPLace.GetComponent<Interactable>().EnableDragging();
        }

        private GameObject SpawnObjectOnGrid(GameObject objectToPlace)
        {
            Vector3 middleofScreen = GetPointWhereRayHitsGroundInMiddleOfScreen();

            Vector3 position = SnapCoordinateToGrid(middleofScreen);

            objectToPlace.transform.SetPositionAndRotation(position, Quaternion.identity);

            objectToPlace.AddComponent<PlaceableObject>();

            return objectToPlace;
        }

        private static Vector3 GetPointWhereRayHitsGroundInMiddleOfScreen()
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            Plane plane = new Plane(Vector3.up, Vector3.zero);
            
            Vector3 middleofScreen = Vector3.zero;
            if (plane.Raycast(ray, out float distance))
            {
                middleofScreen = ray.GetPoint(distance);
            }

            return middleofScreen;
        }

        private bool CanBePlaced(PlaceableObject objectToPlace)
        {
            BoundsInt area = new BoundsInt();
            area.position = GridLayout.WorldToCell(objectToPlace.GetStartPosition());

            area.size = new Vector3Int(objectToPlace.Size.x + 1, objectToPlace.Size.y + 1, 1);

            TileBase[] baseArray = GetTilesBlock(area, PlacingTilemap);
            
            foreach (var tileBase in baseArray)
            {
                if (tileBase == TileBase)
                {
                    return false;
                }
            }
            
            baseArray = GetTilesBlock(area, ValidPlacingTilemap);

            foreach (var tileBase in baseArray)
            {
                if (tileBase != TileBase)
                {
                    return false;
                }
            }

            return true;
        }

        private void RemoveArea(Vector3Int start, Vector3Int size)
        {
            PlacingTilemap.BoxFill(start, null, start.x, start.y, start.x + size.x, start.y + size.y);
        }

        private void TakeArea(Vector3Int start, Vector3Int size, TileBase tileBase)
        {
            PlacingTilemap.BoxFill(start, tileBase, start.x, start.y, start.x + size.x, start.y + size.y);
        }

        private void TakeArea(Tilemap tilemap, Vector3Int start, Vector3Int size, TileBase tileBase)
        {
            tilemap.BoxFill(start, tileBase, start.x, start.y, start.x + size.x, start.y + size.y);
        }

        #endregion
    }
}