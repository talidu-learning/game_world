using Interactables;
using UnityEngine;

namespace BuildingSystem
{
    public class TestBuildingSystem : MonoBehaviour
    {
        public GameObject Prefab;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                SpawnObject();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BuildingSystem.Current.DeleteSelectedObject();
            }
        }

        public void SpawnObject()
        {
            var go = Instantiate(Prefab);
            SelectionManager.SELECT_OBJECT_EVENT.Invoke(go.GetComponent<Interactable>());
        }
    }
}