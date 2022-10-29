using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ItemButton : MonoBehaviour
    {
        private Image image;
        [SerializeField] private GameObject PlaceButton;

        void Awake()
        {
            image = GetComponent<Image>();
        }

    }
}
