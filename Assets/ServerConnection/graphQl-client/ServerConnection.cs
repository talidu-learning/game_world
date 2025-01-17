using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomInput;
using GraphQlClient.Core;
using Plugins.WebGL;
using ServerConnection.graphQl_client;
using UnityEngine;
using UnityEngine.Networking;

namespace ServerConnection
{
    public class ServerConnection : MonoBehaviour
    {
        [SerializeField] private GraphApi taliduGraphApi;
        [SerializeField] private string token;
        [SerializeField] private GameObject TouchManager;

        public static StudentData StudentData;

        public static bool Loaded;

        private Guid id;

        public static List<ItemData> purchasedItems = new List<ItemData>();

        private bool deactivatedServerConnection = false;

        public void DeactivateServerConnection()
        {
            deactivatedServerConnection = true;
        }
        
        public async Task GetStudentData()
        {
            // WebGLPluginJS.SetUpTestToken();
            //#if DEVELOPMENT_BUILD
            //token = WebGLPluginJS.GetTokenFromLocalStorage();  // im editor auskommentieren damit es funktioniert
            taliduGraphApi.SetAuthToken(token);
            id = new Guid(await GetStudentID());

            if (id == null || id == Guid.Empty) return;

            var studentData = await GetStudentData(id);

            if (!studentData) return;

            var items = await GetAllItems(id);

            if (items == null) return;

            purchasedItems = items;

            Loaded = true;
        }

        public async Task<bool> UpdateStarCount(int starCount)
        {
            return await UpdateStars(id, starCount);
        }

        public async Task<ItemData> CreateNewItemForCurrentPlayer(string itemId)
        {
            return await CreateItem(id, itemId);
        }

        private async Task<ItemData> CreateItem(Guid guid, string itemId)
        {
            if (deactivatedServerConnection)
            {
                var item = new ItemData
                {
                    id = itemId,
                    uid = new Guid()
                };
                purchasedItems.Add(item);
                return item;
            }
            
            GraphApi.Query query = taliduGraphApi.GetQueryByName("CreateItem", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {input = new {purchasedItem = new {owner = guid, id = itemId}}});
            var response = await SendRequest(query);

            if (string.IsNullOrEmpty(response))
            {
                ServerConnectionErrorUI.ServerErrorOccuredEvent.Invoke();
                return null;
            }

            var uid = RegExJsonParser.GetValueOfField("uid", response);

            var newItem = new ItemData
            {
                id = itemId,
                uid = new Guid(uid)
            };
            purchasedItems.Add(newItem);

            return newItem;
        }

        public async void UpdateItemPosition(Guid itemGuid, string itemID, float xCoord, float zCoord, bool isFlipped,
            Action<bool, string, Guid> callBack = null)
        {
            if (deactivatedServerConnection)
            {
                callBack?.Invoke(true, itemID, itemGuid);
                return;
            }
            
            GraphApi.Query query = taliduGraphApi.GetQueryByName("UpdateItem", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {input = new {purchasedItemPatch = new {x = xCoord, z = zCoord, flipped = isFlipped}, uid = itemGuid}});
            var response = await SendRequest(query);

            if (string.IsNullOrEmpty(response))
            {
                callBack?.Invoke(false, itemID, itemGuid);
                return;
            }

            callBack?.Invoke(true, itemID, itemGuid);
        }

        public async void DeleteItem(Guid itemGuid, string itemID, Action<bool, string, Guid> callBack = null)
        {
            if (deactivatedServerConnection)
            {
                callBack?.Invoke(true, itemID, itemGuid);
                return;
            }
            
            var itemWithSocket = purchasedItems.FirstOrDefault(i => i.uid == itemGuid);

            if (itemWithSocket == null)
            {
                callBack?.Invoke(false, itemID, itemGuid);
                return;
            }

            GraphApi.Query query = taliduGraphApi.GetQueryByName("UpdateItem", GraphApi.Query.Type.Mutation);

            if (itemWithSocket.itemsPlacedOnSockets != null)
            {
                Guid[] newSockets = new Guid[itemWithSocket.itemsPlacedOnSockets.Length];
                query.SetArgs(new
                {
                    input = new {purchasedItemPatch = new {sockets = newSockets, x = 0, z = 0, flipped = false}, uid = itemGuid}
                });
            }
            else
            {
                query.SetArgs(new {input = new {purchasedItemPatch = new {x = 0, z = 0}, uid = itemGuid}});
            }


            var response = await SendRequest(query);

            if (string.IsNullOrEmpty(response))
            {
                callBack?.Invoke(false, itemID, itemGuid);
                return;
            }

            callBack?.Invoke(true, itemID, itemGuid);
        }

        public async void OnPlacedItemOnSocket(Guid onSocketPlacedItemGuid, int socketCount, int socketIndex,
            Guid itemWithSocketsGuid, string itemId, Action<bool, string, Guid> callBack)
        {
            if (deactivatedServerConnection)
            {
                callBack.Invoke(true, itemId, onSocketPlacedItemGuid);
                return;
            }
            
            var itemWithSocket = purchasedItems.FirstOrDefault(i => i.uid == itemWithSocketsGuid);

            if (itemWithSocket == null)
            {
                callBack.Invoke(false, itemId, onSocketPlacedItemGuid);
                return;
            }

            itemWithSocket.itemsPlacedOnSockets ??= new Guid[socketCount];

            itemWithSocket.itemsPlacedOnSockets[socketIndex] = onSocketPlacedItemGuid;
            Debug.Log(itemWithSocket.itemsPlacedOnSockets[socketIndex]);

            GraphApi.Query query = taliduGraphApi.GetQueryByName("UpdateItem", GraphApi.Query.Type.Mutation);
            query.SetArgs(new
            {
                input = new
                {
                    purchasedItemPatch = new {sockets = itemWithSocket.itemsPlacedOnSockets},
                    uid = itemWithSocketsGuid
                }
            });
            var response = await SendRequest(query);

            if (string.IsNullOrEmpty(response))
            {
                callBack.Invoke(false, itemId, onSocketPlacedItemGuid);
                return;
            }

            callBack.Invoke(true, itemId, onSocketPlacedItemGuid);
        }

        public async void OnDeletedItemOnSocket(Guid onSocketPlacedItemGuid, int socketIndex, Guid itemWithSocketsGuid,
            Action<bool, Guid, Guid, int> callBack)
        {
            if (deactivatedServerConnection)
            {
                callBack.Invoke(true, onSocketPlacedItemGuid, itemWithSocketsGuid, socketIndex);
                return;
            }
            
            var itemWithSocket = purchasedItems.FirstOrDefault(i => i.uid == itemWithSocketsGuid);

            if (itemWithSocket == null)
            {
                callBack.Invoke(false, onSocketPlacedItemGuid, itemWithSocketsGuid, socketIndex);
                return;
            }

            itemWithSocket.itemsPlacedOnSockets[socketIndex] = Guid.Empty;

            GraphApi.Query query = taliduGraphApi.GetQueryByName("UpdateItem", GraphApi.Query.Type.Mutation);
            query.SetArgs(new
            {
                input = new
                {
                    purchasedItemPatch = new {sockets = itemWithSocket.itemsPlacedOnSockets},
                    uid = itemWithSocketsGuid
                }
            });
            var response = await SendRequest(query);

            if (string.IsNullOrEmpty(response))
            {
                callBack.Invoke(false, onSocketPlacedItemGuid, itemWithSocketsGuid, socketIndex);
                return;
            }

            callBack.Invoke(true, onSocketPlacedItemGuid, itemWithSocketsGuid, socketIndex);
        }

        #region Initializing

        private async Task<string> GetStudentID()
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("UserId", GraphApi.Query.Type.Query);
            var response = await SendRequest(query);
            return RegExJsonParser.GetValueOfField("currentUserId", response);
        }

        private async Task<bool> GetStudentData(Guid guid)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("Student", GraphApi.Query.Type.Query);
            query.SetArgs(new {id = guid});
            var responseText = await SendRequest(query);

            var dataString = responseText.Replace("{\"data\":{\"studentById\":{", "")
                .Replace("}", "").Replace('"', ' ').Replace(" ", "");

            var array = dataString.Split(',');

            string[] arrayTrimmed = new string[array.Length];
            int i = 0;
            foreach (var data in array)
            {
                var trimmed = data.Substring(data.IndexOf(':') + 1);
                arrayTrimmed[i] = trimmed;
                i++;
            }

            int stars = Int32.Parse(arrayTrimmed[3]);

            StudentData = new StudentData
            {
                Proficiency = int.Parse(arrayTrimmed[0]),
                Age = int.Parse(arrayTrimmed[1]),
                Name = arrayTrimmed[2],
                Stars = stars
            };
            return true;
        }
        
        private async Task<List<ItemData>> GetAllItems(Guid guid)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("GetAllItems", GraphApi.Query.Type.Query);
            query.SetArgs(new {condition = new {owner = guid}});

            var result = await SendRequest(query);
            if (string.IsNullOrEmpty(result)) return null;

            var deserializedData = AllPurchasedItemsDataContainer.FromJson(result);

            List<ItemData> items = new List<ItemData>();

            foreach (var node in deserializedData.Data.AllPurchasedItems.Nodes)
            {
                if (node.Sockets != null)
                {
                    Guid[] socketData = new Guid[node.Sockets.Length];

                    for (int i = 0; i < node.Sockets.Length; i++)
                    {
                        socketData[i] = new Guid(node.Sockets[i]);
                    }

                    ItemData itemData = new ItemData
                    {
                        id = node.Id,
                        uid = node.Uid,
                        x = Convert.ToSingle(node.X),
                        z = Convert.ToSingle(node.Z),
                        itemsPlacedOnSockets = socketData,
                        isFlipped = node.Flipped
                    };
                    items.Add(itemData);
                }
                else
                {
                    ItemData itemData = new ItemData
                    {
                        id = node.Id,
                        uid = node.Uid,
                        x = Convert.ToSingle(node.X),
                        z = Convert.ToSingle(node.Z),
                        isFlipped = node.Flipped
                    };
                    items.Add(itemData);
                }
            }

            return items;
        }

        #endregion

        private async Task<bool> UpdateStars(Guid guid, int starCount)
        {
            if (deactivatedServerConnection) return true;
            GraphApi.Query query = taliduGraphApi.GetQueryByName("UpdateStars", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {input = new {studentPatch = new {stars = starCount}, id = guid}});
            var response = await SendRequest(query);
            return !string.IsNullOrEmpty(response);
        }

        private async Task<string> SendRequest(GraphApi.Query query)
        {
            var timer = StartCoroutine(WaitForResponseTimer());
            TouchManager.SetActive(false);
            UnityWebRequest request = await taliduGraphApi.Post(query);
            StopCoroutine(timer);
            ServerLoadingAnimation.DISABLE_ANIMATION.Invoke();
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError
                or UnityWebRequest.Result.DataProcessingError)
            {
                ServerConnectionErrorUI.ServerErrorOccuredEvent.Invoke();
                request.Dispose();
                return String.Empty;
            }
            
            TouchManager.SetActive(true);

            var text = request.downloadHandler.text;
            request.Dispose();

            return text;
        }

        private IEnumerator WaitForResponseTimer()
        {
            var time = 0f;
            while (true)
            {
                time += Time.deltaTime;
                if (time > 1.0f)
                {
                    ServerLoadingAnimation.ENABLE_ANIMATION.Invoke();
                    break;
                }

                yield return null;
            }
        }
    }
}