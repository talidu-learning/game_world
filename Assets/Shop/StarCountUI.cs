using ServerConnection;
using TMPro;
using UnityEngine;

namespace Shop
{
    public class StarCountUI : MonoBehaviour
    {

        public static StringUnityEvent UpdateStarCount = new StringUnityEvent();
        private TextMeshProUGUI starCountText;
    
        private void Awake()
        {
            starCountText = GetComponent<TextMeshProUGUI>();
            UpdateStarCount.AddListener(OnUpdateStarCount);
        }

        private void OnUpdateStarCount(string count)
        {
            starCountText.text = count;
        }

        void Start()
        {
            starCountText.text = LocalPlayerData.Instance.GetStarCount().ToString();
        }

    }
}
