using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class BuyItemButton : MonoBehaviour
    {
        [SerializeField] private GameObject PlaceButton;
        
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                PlaceButton.SetActive(true);
                gameObject.SetActive(false);
            });
        }
    }
}
