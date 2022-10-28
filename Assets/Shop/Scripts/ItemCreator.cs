using UnityEngine;

namespace Shop
{
    public class ItemCreator : MonoBehaviour
    {
        [SerializeField] private GameObject InteractablePrefab;
        
        public GameObject CreateItem(string id)
        {
            // TODO Find Object in database and customize prefab
            var go = GameObject.Instantiate(InteractablePrefab);
            go.AddComponent<ItemID>().id = id;
            return go;
        }
    }
}