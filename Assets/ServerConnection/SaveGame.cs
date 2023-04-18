using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private ServerConnection ServerConnection;

        [SerializeField] private ItemCreator ItemCreator;
        [SerializeField] private GameObject SocketItem;
        [SerializeField] private ShopInventory ShopInventory;
        [SerializeField] private bool UseServerConnection;
        public static readonly UnityEvent LoadedPlayerData = new UnityEvent();

        private LocalPlayerData localPlayerData;

        private void Awake()
        {
            UseServerConnection = true;

            localPlayerData = gameObject.GetComponent<LocalPlayerData>();
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
                ServerConnection.DeactivateServerConnection();
                StartCoroutine(LoadGameDataFromLocalFile());
            }
        }

        private IEnumerator LoadGameDataFromLocalFile()
        {
            LoadGameStatus();

            StarCountUI.UpdateStarCount.Invoke(LocalPlayerData.Instance.GetStarCount().ToString());

            yield return null;
            LoadedPlayerData.Invoke();
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

            var ownedItems = ServerConnection.purchasedItems;
            localPlayerData._ownedItems = ValidateItems(ownedItems);
            localPlayerData.Initialize();

            LoadGameStatus();

            StarCountUI.UpdateStarCount.Invoke(ServerConnection.StudentData.Stars.ToString());
            LocalPlayerData.Instance.SetStarCount(ServerConnection.StudentData.Stars);


            yield return null;
            LoadedPlayerData.Invoke();
        }

        private List<ItemData> ValidateItems(List<ItemData> ownedItems)
        {
            List<ItemData> validItems = new List<ItemData>();
            foreach (var item in ownedItems)
            {
                if (ShopInventory.ShopItems.Count(i => i.ItemID == item.id) < 1)
                {
                    ServerConnection.DeleteItemFromDatabase(item.uid);
#pragma warning disable 4014
                    ServerConnection.UpdateStarCount(ServerConnection.StudentData.Stars+50);
#pragma warning restore 4014
                }
                else
                {
                    validItems.Add(item);
                }
            }

            return validItems;
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
                var go = ItemCreator.CreateItem(item.id, item.uid);
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
                var go = ItemCreator.CreateItem(item.id, item.uid);
                go.transform.position = new Vector3(item.x, 0, item.z);
                gos.Add(go);
                uids.Add(item.uid);
                if (item.isFlipped) go.GetComponent<Interactable>().Flip();
            }

            return gos;
        }

        private void CreateSocketItem(string itemId, Guid uid, Socket currentSocket)
        {
            var socketItemGo = Instantiate(this.SocketItem, currentSocket.gameObject.transform, false);

            var localScale = ShopInventory.ShopItems.First(i => i.ItemID == itemId).Prefab.transform
                .GetChild(0).localScale;

            SocketPlacement.ScaleGameObjectForSocket(localScale, socketItemGo);

            var component = socketItemGo.AddComponent<ItemID>();
            component.id = itemId;
            component.uid = uid;
            component.ItemAttributes = ShopInventory.ShopItems.FirstOrDefault(i => i.ItemID == itemId)?.Attributes;

            var spriteRenderer = socketItemGo.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = ShopInventory.ShopItems.FirstOrDefault(i => i.ItemID == itemId)?.ItemSprite;
        }
        
    }
}