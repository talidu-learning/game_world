using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Interactables;
using Shop;
using UnityEngine;
using UnityEngine.Events;

namespace ServerConnection
{
    public class SaveGame : MonoBehaviour
    {
        [SerializeField] private Game.ServerConnection ServerConnection;
        
        [SerializeField] private ItemCreator itemCreator;
        [SerializeField] private GameObject SocketItem;
        [SerializeField] private ShopInventory shopInventory;
        
        public static UnityEvent LoadedPlayerData = new UnityEvent();

        private LocalPlayerData _localPlayerData;
        
        private void Awake()
        {
            ServerConnection.GetStudentData();
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
            yield return new WaitUntil(()=> Game.ServerConnection.Loaded);
            
            // if (!File.Exists(Application.persistentDataPath + "/gamedata.json"))
            // {
            //     SaveGameData();
            // }
            
            // _localPlayerData.Initialize(
            //     JsonUtility.FromJson<PlayerDataContainer>(
            //         File.ReadAllText(Application.persistentDataPath + "/gamedata.json")));
            
            Debug.Log("Purchased ItemData: " + Game.ServerConnection.purchasedItems.Count);
            
            _localPlayerData.Initialize(new PlayerDataContainer
            {
                _ownedItems = Game.ServerConnection.purchasedItems
            });
            
            var itemDatas = _localPlayerData.GetPlacedItems().ToList();

            var gos = CreateGameObjects(itemDatas);

            BuildingSystem.BuildingSystem.Current.OnLoadedGame(gos.ToArray());
            
            StarCountUI.UpdateStarCount.Invoke(Game.ServerConnection.StudentData.Stars.ToString());
            LocalPlayerData.Instance.SetStarCount(Game.ServerConnection.StudentData.Stars);
            
            Debug.Log("Tables: " + LocalPlayerData.Instance.GetCountOfOwnedItems("Table"));
            
            yield return null;
            LoadedPlayerData.Invoke();
        }

        private List<GameObject> CreateGameObjects(List<ItemData> itemDatas)
        {
            List<GameObject> gos = new List<GameObject>();
            List<Guid> uids = new List<Guid>();

            var itemswithsockets = itemDatas.Where(i => i.itemsPlacedOnSockets.Length > 0);
            var itemswithoutsockets = itemDatas.Where(i => i.itemsPlacedOnSockets.Length == 0).ToList();

            foreach (var item in itemswithsockets)
            {
                if (uids.Contains(item.uid)) continue;
                var go = itemCreator.CreateItem(item.id, item.uid);
                go.transform.position = new Vector3(item.x, 0, item.z);
                gos.Add(go);
                uids.Add(item.uid);

                var sockets = go.transform.GetChild(0).GetComponentsInChildren<Socket>();

                for (int i = 0; i < sockets.Length; i++)
                {
                    Debug.Log(item.itemsPlacedOnSockets[i]);
                    if (item.itemsPlacedOnSockets[i] != Guid.Empty)
                    {
                        sockets[i].Place(item.itemsPlacedOnSockets[i]);
                        var data = itemDatas.FirstOrDefault(idata => idata.uid == item.itemsPlacedOnSockets[i]);
                        CreateSocketItem(data.id, item.itemsPlacedOnSockets[i], sockets[i]);
                        uids.Add(item.itemsPlacedOnSockets[i]);
                    }
                }
            }

            foreach (var item in itemswithoutsockets)
            {
                if (uids.Contains(item.uid)) continue;
                var go = itemCreator.CreateItem(item.id, item.uid);
                go.transform.position = new Vector3(item.x, 0, item.z);
                gos.Add(go);
                uids.Add(item.uid);
            }

            return gos;
        }

        private GameObject CreateSocketItem(string itemId, Guid uid, Socket currentSocket)
        {
            var socketItem = Instantiate(SocketItem, currentSocket.gameObject.transform, false);

            var component = socketItem.AddComponent<ItemID>();
            component.id = itemId;
            component.uid = uid;
            component.ItemAttributes = shopInventory.ShopItems.FirstOrDefault(i => i.ItemID == itemId)?.Attributes;

            var spriteRenderer = socketItem.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = shopInventory.ShopItems.FirstOrDefault(i => i.ItemID == itemId)?.ItemSprite;

            return socketItem;
        }

        // private void OnApplicationQuit()
        // {
        //     SaveGameData();
        // }
    }
}