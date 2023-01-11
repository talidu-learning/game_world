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
        
        [SerializeField] private ItemCreator ItemCreator;
        [SerializeField] private GameObject SocketItem;
        [SerializeField] private ShopInventory ShopInventory;
        [SerializeField] private bool UseServerConnection;
        public static readonly UnityEvent LoadedPlayerData = new UnityEvent();

        private LocalPlayerData _localPlayerData;
        
        private void Awake()
        {
            #if DEVELOPMENT_BUILD
                UseServerConnection = true;
            #endif
            _localPlayerData = gameObject.AddComponent<LocalPlayerData>();
        }

        async void Start()
        {
            if (UseServerConnection)
            {
                await ServerConnection.GetStudentData();
                StartCoroutine(LoadGameDataFromServer());  
            }
            else
            {
                StartCoroutine(LoadGameDataFromLocalFile());   
            }
        }

        private void SaveGameData()
        {
            string json = _localPlayerData.GetJsonData();
            
            File.WriteAllText(Application.persistentDataPath + "/gamedata.json", json);
        }

        private IEnumerator LoadGameDataFromLocalFile()
        {
            if (!File.Exists(Application.persistentDataPath + "/gamedata.json"))
            {
                SaveGameData();
            }
            
            LoadGameStatus();
            
            StarCountUI.UpdateStarCount.Invoke(LocalPlayerData.Instance.GetStarCount().ToString());

            yield return null;
            LoadedPlayerData.Invoke();
        }

        private void LoadGameStatus()
        {
            var itemDatas = _localPlayerData.GetPlacedItems().ToList();

            var gos = CreateGameObjects(itemDatas, ref _localPlayerData._ownedItems);

            BuildingSystem.BuildingSystem.Current.OnLoadedGame(gos.ToArray());
        }

        private IEnumerator LoadGameDataFromServer()
        {
            yield return new WaitUntil(()=> Game.ServerConnection.Loaded);

            Debug.Log("Purchased ItemData: " + Game.ServerConnection.purchasedItems.Count);

            _localPlayerData._ownedItems = Game.ServerConnection.purchasedItems;
            _localPlayerData.Initialize();
            
            LoadGameStatus();
            
            StarCountUI.UpdateStarCount.Invoke(Game.ServerConnection.StudentData.Stars.ToString());
            LocalPlayerData.Instance.SetStarCount(Game.ServerConnection.StudentData.Stars);
            
            Debug.Log("Tables: " + LocalPlayerData.Instance._ownedItems.Count);

            yield return null;
            LoadedPlayerData.Invoke();
        }

        private List<GameObject> CreateGameObjects(List<ItemData> placedItems, ref List<ItemData> allObjects)
        {
            List<GameObject> gos = new List<GameObject>();
            List<Guid> uids = new List<Guid>();

            var itemswithsockets = placedItems.Where(i => i.itemsPlacedOnSockets != null);
            Debug.Log("SocketedItems: " + itemswithsockets.ToList().Count);
            var itemswithoutsockets = placedItems.Where(i => i.itemsPlacedOnSockets == null);
            Debug.Log("UnsocketedItems: " + itemswithoutsockets.ToList().Count);

            foreach (var item in itemswithsockets)
            {
                if (uids.Contains(item.uid)) continue;
                var go = ItemCreator.CreateItem(item.id, item.uid);
                go.transform.position = new Vector3(item.x, 0, item.z);
                gos.Add(go);
                uids.Add(item.uid);

                var sockets = go.transform.GetChild(0).GetComponentsInChildren<Socket>();

                for (int i = 0; i < sockets.Length; i++)
                {
                    Debug.Log("Item on Socket: " + item.itemsPlacedOnSockets[i]);
                    if (item.itemsPlacedOnSockets[i] != Guid.Empty)
                    {
                        sockets[i].Place(item.itemsPlacedOnSockets[i]);
                        var data = allObjects.FirstOrDefault(idata => idata.uid == item.itemsPlacedOnSockets[i]);
                        allObjects.FirstOrDefault(idata => idata.uid == item.itemsPlacedOnSockets[i])!.isPlacedOnSocket =
                            true;
                        CreateSocketItem(data.id, item.itemsPlacedOnSockets[i], sockets[i]);
                        uids.Add(item.itemsPlacedOnSockets[i]);
                    }
                }
            }

            foreach (var item in itemswithoutsockets)
            {
                if (uids.Contains(item.uid)) continue;
                var go = ItemCreator.CreateItem(item.id, item.uid);
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
            component.ItemAttributes = ShopInventory.ShopItems.FirstOrDefault(i => i.ItemID == itemId)?.Attributes;

            var spriteRenderer = socketItem.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = ShopInventory.ShopItems.FirstOrDefault(i => i.ItemID == itemId)?.ItemSprite;

            return socketItem;
        }

        private void OnApplicationQuit()
        {
            #if !DEVELOPMENT_BUILD
                if(!UseServerConnection)
                    SaveGameData();
            #endif
        }
    }
}