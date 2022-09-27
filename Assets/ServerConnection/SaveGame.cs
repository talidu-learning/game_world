using System.Collections;
using System.Collections.Generic;
using System.IO;
using Shop;
using UnityEngine;
using UnityEngine.Events;

namespace ServerConnection
{
    public class SaveGame : MonoBehaviour
    {
        [SerializeField] private ItemCreator itemCreator;
        
        public static UnityEvent LoadedPlayerData = new UnityEvent();

        private LocalPlayerData _localPlayerData;
        
        private void Awake()
        {
            // TODO Load Data from Server
            _localPlayerData = gameObject.AddComponent<LocalPlayerData>();
        }

        private void Start()
        {
            StartCoroutine(LoadGameData());
        }

        private void SaveGameData()
        {
            string json = _localPlayerData.GetJsonData();
            
            File.WriteAllText(Application.persistentDataPath + "/gamedata.json", json);
        }

        private IEnumerator LoadGameData()
        {
            _localPlayerData.Initilize(
                JsonUtility.FromJson<PlayerDataContainer>(
                    File.ReadAllText(Application.persistentDataPath + "/gamedata.json")));

            var ids = _localPlayerData.GetPlacedItems();

            List<GameObject> gos = new List<GameObject>();
            
            foreach (var item in ids)
            {
                var go = itemCreator.CreateItem(item.id);
                go.transform.position = new Vector3(item.x, 0, item.z);
                gos.Add(go);
            }
            
            BuildingSystem.BuildingSystem.Current.OnLoadedGame(gos.ToArray());
            
            yield return null;
            LoadedPlayerData.Invoke();
        }

        private void OnApplicationQuit()
        {
            SaveGameData();
        }
    }
}