using UnityEngine;

namespace ServerConnection
{
    public class SaveGame : MonoBehaviour
    {
        private void Awake()
        {
            // TODO Load Data from Server
            var playerData = gameObject.AddComponent<LocalPlayerData>();
            Debug.Log(playerData.GetJSONData());
        }
    }
}