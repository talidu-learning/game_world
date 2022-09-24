using Interactables;
using ServerConnection;
using UnityEngine;
using UnityEngine.Events;

namespace Shop
{
    public class BoolGameObjectUnityEvent : UnityEvent<bool, GameObject>{}
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private ItemCreator itemCreator;
        [SerializeField] private ShopInventoryDisplay shopInventoryDisplay;

        public static StringUnityEvent InitilizePlaceObjectEvent = new StringUnityEvent();

        public static BoolGameObjectUnityEvent OnTriedPlacingGameObjectEvent = new BoolGameObjectUnityEvent();

        private void Awake()
        {
            InitilizePlaceObjectEvent.AddListener(OnPlaceObject);
            OnTriedPlacingGameObjectEvent.AddListener(OnTriedPlacingGameObject);
        }

        private void OnTriedPlacingGameObject(bool wasPlacedSuccessfully, GameObject placedObject)
        {
            var id = placedObject.GetComponent<ItemID>().id;
            shopInventoryDisplay.OnPlacedItem(id, wasPlacedSuccessfully);
            
            if(wasPlacedSuccessfully)LocalPlayerData.Instance.OnPlacedItem(id, placedObject.transform.position.x, placedObject.transform.position.z);
            else
            {
                LocalPlayerData.Instance.OnWithdrewItem(id);
                Destroy(placedObject);
            }
        }

        private void OnPlaceObject(string itemId)
        {
            var go = itemCreator.CreateItem(itemId);
            SelectionManager.SELECT_OBJECT_EVENT.Invoke(go.GetComponent<Interactable>());
        }
    }
}