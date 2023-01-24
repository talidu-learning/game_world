using System;
using System.Collections;
using System.Linq;
using Interactables;
using ServerConnection;
using Shop;
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

        [SerializeField] private ServerConnection.ServerConnection ServerConnection;

        private Grid grid;
        private PlaceableObject placeableObject;
        private TilemapRenderer visibleMapRenderer;

        private Action<bool, string, Guid> serverCallbackPlacing;
        private Action<bool, string, Guid> serverCallbackDelete;

        private void Awake()
        {
            Current = this;
            grid = GridLayout.gameObject.GetComponent<Grid>();
            visibleMapRenderer = VisibleTilemap.GetComponent<TilemapRenderer>();
            visibleMapRenderer.enabled = false;

            serverCallbackPlacing = ServerCallbackOnTriedPlacing;
            serverCallbackDelete = ServerCallbackOnTriedDeleting;
        }

        public void OnLoadedGame(GameObject[] placedObjects)
        {
            StartCoroutine(LoadItems(placedObjects));
        }

        public void DeleteSelectedObject()
        {
            if (!placeableObject) return;
            var itemId = placeableObject.gameObject.GetComponent<ItemID>();
            Debug.Log("Delete!");
            ServerConnection.DeleteItem(itemId.uid, itemId.id, serverCallbackDelete);
        }

        public void PlaceLastObjectOnGrid()
        {
            if (placeableObject == null) return;
            if (CanBePlaced(placeableObject))
            {
                ServerConnection.UpdateItemPosition(placeableObject.gameObject.GetComponent<ItemID>().uid,
                    placeableObject.gameObject.GetComponent<ItemID>().id,
                    placeableObject.gameObject.transform.position.x, placeableObject.gameObject.transform.position.z,
                    serverCallbackPlacing);
            }
            else
            {
                OnFailedPlacement();
            }
        }

        private void ServerCallbackOnTriedDeleting(bool sucessfullyConnected, string itemID, Guid uid)
        {
            if (sucessfullyConnected)
                OnSuccessfulDelete();
            else OnFailedDeleting();
        }

        private void OnFailedDeleting()
        {
            Debug.Log("Failed to delete");
            // dunno
        }

        private void OnSuccessfulDelete()
        {
            Debug.Log("Successful to delete");
            if (placeableObject.WasPlacedBefore)
            {
                RemoveArea(placeableObject.PlacedPosition, placeableObject.Size);
            }

            ShopManager.OnTriedPlacingGameObjectEvent.Invoke(false, placeableObject.gameObject);
            LocalPlayerData.Instance.OnDeletedItem(placeableObject.GetComponent<ItemID>().uid);
            placeableObject = null;
            visibleMapRenderer.enabled = false;
        }

        private void ServerCallbackOnTriedPlacing(bool sucessfullyConnected, string itemID, Guid uid)
        {
            if (sucessfullyConnected)
                OnSuccessfulPlacement();
            else OnFailedPlacement();
        }

        private void OnSuccessfulPlacement()
        {
            PlacePlaceable(placeableObject);
            ShopManager.OnTriedPlacingGameObjectEvent.Invoke(true, placeableObject.gameObject);
            visibleMapRenderer.enabled = false;
            placeableObject = null;
        }

        private void OnFailedPlacement()
        {
            if (placeableObject.WasPlacedBefore)
            {
                placeableObject.transform.position = placeableObject.PlacePosition;
                placeableObject.Place(placeableObject.PlacedPosition);
                TakeArea(placeableObject.PlacedPosition, placeableObject.Size, TileBase);
                return;
            }

            ShopManager.OnTriedPlacingGameObjectEvent.Invoke(false, placeableObject.gameObject);
            visibleMapRenderer.enabled = false;
            placeableObject = null;
        }

        public void PlaceObjectOnGrid(PlaceableObject placeableObject)
        {
            PlacePlaceable(placeableObject);
        }

        private void PlacePlaceable(PlaceableObject objectToPlace)
        {
            Vector3Int start = GridLayout.WorldToCell(objectToPlace.GetStartPosition());
            objectToPlace.Place(start);
            TakeArea(start, objectToPlace.Size, TileBase);
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
            RemoveArea(placeable.PlacedPosition, placeable.Size);
            InitializePlacing(objectToReplace);
            visibleMapRenderer.enabled = true;
        }

        public Vector3 SnapCoordinateToGrid(Vector3 position)
        {
            Vector3Int cellPos = GridLayout.WorldToCell(position);
            position = grid.GetCellCenterWorld(cellPos);
            return position;
        }

        #region private

        private IEnumerator LoadItems(GameObject[] placedObjects)
        {
            foreach (var go in placedObjects)
            {
                go.AddComponent<PlaceableObject>();
            }

            yield return null;

            foreach (var go in placedObjects)
            {
                PlacePlaceable(go.GetComponent<PlaceableObject>());
            }
        }

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

            // Not really tested. Decided to automatically zoom out when placing stuff
            // var placeable = objectToPlace.GetComponent<PlaceableObject>();
            //
            // while (!CanBePlaced(placeable))
            // {
            //     objectToPlace.transform.position += new Vector3(0, 0, -1);
            // }
            //
            // objectToPlace.transform.position = SnapCoordinateToGrid(objectToPlace.transform.position);

            return objectToPlace;
        }

        private static Vector3 GetPointWhereRayHitsGroundInMiddleOfScreen()
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Vector3 middleOfScreen = Vector3.zero;
            if (plane.Raycast(ray, out float distance))
            {
                middleOfScreen = ray.GetPoint(distance);
            }

            return middleOfScreen;
        }

        private bool CanBePlaced(PlaceableObject objectToPlace)
        {
            BoundsInt area = new BoundsInt();
            Debug.Log("ObjectToPlace:" + objectToPlace);
            area.position = GridLayout.WorldToCell(objectToPlace.GetStartPosition());

            area.size = new Vector3Int(objectToPlace.Size.x + 1, objectToPlace.Size.y + 1, 1);

            TileBase[] baseArray = GetTilesBlock(area, PlacingTilemap);

            if (baseArray.Any(tileBase => tileBase == TileBase))
            {
                return false;
            }

            baseArray = GetTilesBlock(area, ValidPlacingTilemap);

            return baseArray.All(tileBase => tileBase == TileBase);
        }

        private void RemoveArea(Vector3Int start, Vector3Int size)
        {
            PlacingTilemap.BoxFill(start, null, start.x, start.y, start.x + size.x, start.y + size.y);
        }

        private void TakeArea(Vector3Int start, Vector3Int size, TileBase tileBase)
        {
            PlacingTilemap.BoxFill(start, tileBase, start.x, start.y, start.x + size.x, start.y + size.y);
        }

        #endregion
    }
}