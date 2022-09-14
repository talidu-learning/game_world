using UnityEngine;
using UnityEngine.Tilemaps;

namespace BuildingSystem
{
    public class BuildingSystem : MonoBehaviour
    {
        public static BuildingSystem Current;

        public GridLayout GridLayout;

        private Grid grid;

        [SerializeField] private Tilemap PlacingTilemap;
        [SerializeField] private Tilemap VisibleTilemap;
        //[SerializeField] private Tilemap HeightTilemap;
        [SerializeField] private TileBase TileBase;
        [SerializeField] private TileBase VisibleTileBase;

        [SerializeField] private int GridSize = 50;
        [SerializeField] private Vector3Int Origin;

        public GameObject prefab1;

        private PlaceableObject placeableObject;

        private void Awake()
        {
            Current = this;
            grid = GridLayout.gameObject.GetComponent<Grid>();
        }

        private void Start()
        {
            InitializeVisibleGrid();
            InitializeWithObject(prefab1);
        }

        private void InitializeVisibleGrid()
        {
            for (int i = Origin.x; i <= Origin.x + GridSize; i++)
            {
                for (int j = Origin.y; j <= Origin.y + GridSize; j++)
                {
                    VisibleTilemap.SetTile(new Vector3Int(i,j,0), VisibleTileBase);
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                InitializeWithObject(prefab1);
            }

            if (!placeableObject) return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(placeableObject.gameObject);
            }

            // var tilepos = GridLayout.WorldToCell(placeableObject.GetStartPosition());
            // var tile = HeightTilemap.GetTile(tilepos);
            // Debug.Log(tile);
            // if (tile == null)
            //     placeableObject.transform.position = new Vector3(placeableObject.transform.position.x,
            //         0, placeableObject.transform.position.z);
            // else if (tile.name == "Red")
            //     placeableObject.transform.position = new Vector3(placeableObject.transform.position.x, 1, placeableObject.transform.position.z);
            // else if (tile.name == "Green")
            //     placeableObject.transform.position = new Vector3(placeableObject.transform.position.x, -1, placeableObject.transform.position.z);
        }

        public void PlaceObjectOnGrid()
        {
            if (CanBePlaced(placeableObject))
            {
                Vector3Int start = GridLayout.WorldToCell(placeableObject.GetStartPosition());
                placeableObject.Place(start);
                TakeArea(start, placeableObject.Size, TileBase);
            }
            else
            {
                Destroy(placeableObject.gameObject);
            }

            placeableObject = null;
        }

        public static Vector3 GetMousePositionWorld()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit raycastHit))
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

        private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
        {
            TileBase[] array = new TileBase[(area.size.x+1) * (area.size.y+1) * area.size.z];
            int counter = 0;

            foreach (var v in area.allPositionsWithin)
            {
                Vector3Int pos = new Vector3Int(v.x, v.y, 0);
                array[counter] = tilemap.GetTile(pos);
                counter++;
            }

            return array;
        }
    
        public void InitializeWithObject(GameObject prefab)
        {
            // TODO place Object in middle of camera
            Vector3 position = SnapCoordinateToGrid(Vector3.zero);

            GameObject obj = Instantiate(prefab, position, Quaternion.identity);

            obj.AddComponent<PlaceableObject>();
        
            placeableObject = obj.GetComponent<PlaceableObject>();
        }

        public void SetPlaceableObject(PlaceableObject placeable)
        {
            placeableObject = placeable;
        }

        private bool CanBePlaced(PlaceableObject objectToPlace)
        {
            BoundsInt area = new BoundsInt();
            area.position = GridLayout.WorldToCell(objectToPlace.GetStartPosition());

            area.size = objectToPlace.Size;

            TileBase[] baseArray = GetTilesBlock(area, PlacingTilemap);

            foreach (var tileBase in baseArray)
            {
                if (tileBase == TileBase)
                {
                    return false;
                }
            }
        
            baseArray = GetTilesBlock(area, VisibleTilemap);

            foreach (var tileBase in baseArray)
            {
                if (tileBase != VisibleTileBase)
                {
                    return false;
                }
            }

            return true;
        }

        public void RemoveArea(Vector3Int start, Vector3Int size)
        {
            PlacingTilemap.BoxFill(start, null, start.x, start.y, start.x + size.x, start.y + size.y);
        }

        public void TakeArea(Vector3Int start, Vector3Int size, TileBase tileBase)
        {
            PlacingTilemap.BoxFill(start, tileBase, start.x, start.y, start.x + size.x, start.y + size.y);
        }
    
        public void TakeArea(Tilemap tilemap, Vector3Int start, Vector3Int size, TileBase tileBase)
        {
            tilemap.BoxFill(start, tileBase, start.x, start.y, start.x + size.x, start.y + size.y);
        }
    }
}
