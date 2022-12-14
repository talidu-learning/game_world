using System;
using UnityEngine;

namespace BuildingSystem
{
    public class Home : MonoBehaviour
    {
        [SerializeField] private GameObject HomePrefab;
        
        private void Start()
        {
            PlaceHomeOnGrid();
        }

        private void PlaceHomeOnGrid()
        {
            BuildingSystem.Current.PlaceObjectOnGrid(Instantiate(HomePrefab).AddComponent<PlaceableObject>());
        }
    }
}