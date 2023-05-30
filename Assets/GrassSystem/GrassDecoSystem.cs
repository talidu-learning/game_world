using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class GrassDecoSystem : MonoBehaviour
{
    [Tooltip("The grid used to indicate used spaces")] [SerializeField]
    private Tilemap PlacingGrid;

    private Tilemap grassMap;

    public static readonly UnityEvent UpdateGrassMap = new UnityEvent();

    private Dictionary<Vector3Int, GameObject> GrassTiles = new Dictionary<Vector3Int, GameObject>();

    [SerializeField] private GameObject Grass;

    void Awake()
    {
        grassMap = GetComponent<Tilemap>();
        foreach (var pos in grassMap.cellBounds.allPositionsWithin)
        {
            Debug.Log(grassMap.GetTile(pos));
            if (grassMap.GetTile(pos))
            {
                GrassTiles.Add(pos, SpawnGrass(pos));
            }
        }

        UpdateGrassMap.AddListener(StartUpdate);
    }

    private GameObject SpawnGrass(Vector3Int pos)
    {
        var realPos = new Vector3(pos.x, pos.z, pos.y);
        return Instantiate(Grass, realPos, Quaternion.identity);
    }

    private void StartUpdate()
    {
        StartCoroutine(OnUpdatedPlacingMap());
    }

    private IEnumerator OnUpdatedPlacingMap()
    {
        foreach (var pos in GrassTiles)
        {
            if (PlacingGrid.HasTile(pos.Key))
            {
                if(GrassTiles.TryGetValue(pos.Key, out GameObject grass))
                    grass.SetActive(false);
            }
            else
            {
                if(GrassTiles.TryGetValue(pos.Key, out GameObject grass))
                    grass.SetActive(true);
            }
            yield return null;
        }
    }
}