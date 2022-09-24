using UnityEngine;

namespace Shop
{
    public class ItemCreator : MonoBehaviour
    {
        [SerializeField] private GameObject InteractablePrefab;
        
        public GameObject CreateItem(string id)
        {
            // TODO Find Object in database and customize prefab
            
            return GameObject.Instantiate(InteractablePrefab);
        }
    }
}