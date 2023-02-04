using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Interactables;
using Inventory;
using Shop;
using UnityEngine;
using UnityEngine.Events;

namespace ServerConnection
{
    public class SaveGame : MonoBehaviour
    {
        [SerializeField] private ServerConnection serverConnection;

        [SerializeField] private ItemCreator itemCreator;
        [SerializeField] private GameObject socketItem;
        [SerializeField] private ShopInventory shopInventory;
        [SerializeField] private bool useServerConnection;
        public static readonly UnityEvent TEXT_LOADED_PLAYER_DATA = new UnityEvent();

        private LocalPlayerData localPlayerData;

        private void Awake()
        {
            useServerConnection = true;

            localPlayerData = gameObject.GetComponent<LocalPlayerData>();
        }

        async void Start()
        {
            if (useServerConnection)
            {
                await serverConnection.GetStudentData();
                StartCoroutine(LoadGameDataFromServer());
            }
            else
            {
                serverConnection.DeactivateServerConnection();
                StartCoroutine(LoadGameDataFromLocalFile());
            }
        }

        private IEnumerator LoadGameDataFromLocalFile()
        {
            LoadGameStatus();

            StarCountUI.UpdateStarCount.Invoke(LocalPlayerData.Instance.GetStarCount().ToString());

            yield return null;
            TEXT_LOADED_PLAYER_DATA.Invoke();
        }

        private void LoadGameStatus()
        {
            var itemDatas = localPlayerData.GetPlacedItems().ToList();

            var gos = CreateGameObjects(itemDatas, ref localPlayerData._ownedItems);

            BuildingSystem.BuildingSystem.Current.OnLoadedGame(gos.ToArray());
        }

        private IEnumerator LoadGameDataFromServer()
        {
            yield return new WaitUntil(() => ServerConnection.Loaded);

            localPlayerData._ownedItems = ServerConnection.purchasedItems;
            localPlayerData.Initialize();

            LoadGameStatus();

            StarCountUI.UpdateStarCount.Invoke(ServerConnection.StudentData.Stars.ToString());
            LocalPlayerData.Instance.SetStarCount(ServerConnection.StudentData.Stars);


            yield return null;
            TEXT_LOADED_PLAYER_DATA.Invoke();
        }

        private List<GameObject> CreateGameObjects(List<ItemData> placedItems, ref List<ItemData> allObjects)
        {
            List<GameObject> gos = new List<GameObject>();
            List<Guid> uids = new List<Guid>();

            var itemswithsockets = placedItems.Where(i => i.itemsPlacedOnSockets != null);
            var itemswithoutsockets = placedItems.Where(i => i.itemsPlacedOnSockets == null);

            foreach (var item in itemswithsockets)
            {
                if (uids.Contains(item.uid)) continue;
                var go = itemCreator.CreateItem(item.id, item.uid);
                go.transform.position = new Vector3(item.x, 0, item.z);
                gos.Add(go);
                uids.Add(item.uid);

                if (item.isFlipped) go.GetComponent<Interactable>().Flip();

                var sockets = go.transform.GetChild(0).GetComponentsInChildren<Socket>();

                for (int i = 0; i < sockets.Length; i++)
                {
                    if (item.itemsPlacedOnSockets[i] != Guid.Empty)
                    {
                        sockets[i].Place(item.itemsPlacedOnSockets[i]);
                        var data = allObjects.FirstOrDefault(idata => idata.uid == item.itemsPlacedOnSockets[i]);
                        allObjects.FirstOrDefault(idata => idata.uid == item.itemsPlacedOnSockets[i])!
                                .isPlacedOnSocket = true;
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
                if (item.isFlipped) go.GetComponent<Interactable>().Flip();
            }

            return gos;
        }

        private void CreateSocketItem(string itemId, Guid uid, Socket currentSocket)
        {
            var socketItemGo = Instantiate(this.socketItem, currentSocket.gameObject.transform, false);

            var localScale = shopInventory.ShopItems.First(i => i.ItemID == itemId).Prefab.transform
                .GetChild(0).localScale;

            SocketPlacement.ScaleGameObjectForSocket(localScale, socketItemGo);

            var component = socketItemGo.AddComponent<ItemID>();
            component.id = itemId;
            component.uid = uid;
            component.ItemAttributes = shopInventory.ShopItems.FirstOrDefault(i => i.ItemID == itemId)?.Attributes;

            var spriteRenderer = socketItemGo.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = shopInventory.ShopItems.FirstOrDefault(i => i.ItemID == itemId)?.ItemSprite;
        }
        
    }
}