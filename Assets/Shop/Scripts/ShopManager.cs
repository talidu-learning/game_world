using Interactables;
using UnityEngine;

namespace Shop
{
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private ItemCreator itemCreator;

        public static StringUnityEvent PlaceObjectEvent = new StringUnityEvent();

        private void Awake()
        {
            PlaceObjectEvent.AddListener(OnPlaceObject);
        }

        private void OnPlaceObject(string itemId)
        {
            var go = itemCreator.CreateItem(itemId);
            SelectionManager.SELECT_OBJECT_EVENT.Invoke(go.GetComponent<Interactable>());
        }
    }
}