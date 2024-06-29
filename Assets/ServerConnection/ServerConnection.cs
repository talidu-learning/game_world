using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomInput;
using GraphQlClient.Core;
using Newtonsoft.Json;
using Plugins.WebGL;
using ServerConnection.graphQl_client;
using UnityEngine;
using UnityEngine.Networking;

namespace ServerConnection
{
    public class ServerConnection : MonoBehaviour
    {
        [SerializeField] private GraphApi TaliduGraphApi;
        [SerializeField] private string Token;
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
            if(deactivatedServerConnection)return;
            // token = WebGLPluginJS.GetTokenFromLocalStorage();
            Debug.Log("Took token from local storage: " + Token);
            TaliduGraphApi.SetAuthToken(Token);

            var idString = await GetStudentID();

            bool isValid = Guid.TryParse(idString, out Guid guid);

            if (!isValid)
            {
                ThrowServerError();
                return;
            }
            
            id = guid;

            if (id == null || id == Guid.Empty)
            {
                ThrowServerError();
                return;
            }

            var studentData = await GetStudentData(id);

            if (!studentData)
            {
                ThrowServerError();
                return;
            }

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
            
            GraphApi.Query query = TaliduGraphApi.GetQueryByName("CreateItem", GraphApi.Query.Type.Mutation);
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
            
            GraphApi.Query query = TaliduGraphApi.GetQueryByName("UpdateItem", GraphApi.Query.Type.Mutation);
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

            GraphApi.Query query = TaliduGraphApi.GetQueryByName("UpdateItem", GraphApi.Query.Type.Mutation);

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
        
        public async void DeleteItemFromDatabase(Guid itemGuid)
        {
            if (deactivatedServerConnection)
            {
                return;
            }
            
            var itemWithSocket = purchasedItems.FirstOrDefault(i => i.uid == itemGuid);

            if (itemWithSocket == null)
            {
                return;
            }

            GraphApi.Query query = TaliduGraphApi.GetQueryByName("DeleteItem", GraphApi.Query.Type.Mutation);

            query.SetArgs(new{input = new {uid = itemGuid}});


            await SendRequest(query);
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

            GraphApi.Query query = TaliduGraphApi.GetQueryByName("UpdateItem", GraphApi.Query.Type.Mutation);
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

            GraphApi.Query query = TaliduGraphApi.GetQueryByName("UpdateItem", GraphApi.Query.Type.Mutation);
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
            GraphApi.Query idQuery = TaliduGraphApi.GetQueryByName("UserId", GraphApi.Query.Type.Query);
            IdRequestData idRequestData = await WebRequest<IdRequestData>(idQuery);
            return idRequestData.query.currentUserId;
        }

        private async Task<bool> GetStudentData(Guid guid)
        {
            GraphApi.Query studentQuery = TaliduGraphApi.GetQueryByName("Student", GraphApi.Query.Type.Query);
            studentQuery.SetArgs( new {id = guid});
            StudentDataJson studentDataJson = await WebRequest<StudentDataJson>(studentQuery);

            StudentData = new StudentData
            {
                Proficiency = studentDataJson.studentById.proficiency,
                Stars = studentDataJson.studentById.stars
            };
            return true;
        }
        
        private async Task<List<ItemData>> GetAllItems(Guid guid)
        {
            GraphApi.Query query = TaliduGraphApi.GetQueryByName("GetAllItems", GraphApi.Query.Type.Query);
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
            GraphApi.Query query = TaliduGraphApi.GetQueryByName("UpdateStars", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {input = new {studentPatch = new {stars = starCount}, id = guid}});
            var response = await WebRequest<UpdateStars>(query);
            if(response.updateStudentById.student.stars == starCount)
                return true;

            return false;
        }

        private async Task<string> SendRequest(GraphApi.Query query)
        {
            var timer = StartCoroutine(WaitForResponseTimer());
            TouchManager.SetActive(false);
            UnityWebRequest request = await TaliduGraphApi.Post(query);
            StopCoroutine(timer);
            ServerLoadingAnimation.DISABLE_ANIMATION.Invoke();
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError
                    or UnityWebRequest.Result.DataProcessingError || !String.IsNullOrEmpty(request.error))
            {
                Debug.Log("Error: " + request.error);
                ThrowServerError();
                request.Dispose();
                return String.Empty;
            }
            
            TouchManager.SetActive(true);
        
            var text = request.downloadHandler.text;
            request.Dispose();
        
            return text;
        }
        
        private async Task<T> WebRequest<T>(GraphApi.Query query)
        {
            var timer = StartCoroutine(WaitForResponseTimer());
            TouchManager.SetActive(false);
            UnityWebRequest request = await TaliduGraphApi.Post(query);
            StopCoroutine(timer);
            ServerLoadingAnimation.DISABLE_ANIMATION.Invoke();
            if (request.result != UnityWebRequest.Result.Success)
            {
                ThrowServerError();
                request.Dispose();
                object obj = new object();
                return (T)obj;
            }

            TouchManager.SetActive(true);

            return DeserializeData<T>(request.downloadHandler.text);
        }
        
        private async void WebRequest(GraphApi.Query query)
        {
            var timer = StartCoroutine(WaitForResponseTimer());
            TouchManager.SetActive(false);
            UnityWebRequest request = await TaliduGraphApi.Post(query);
            StopCoroutine(timer);
            ServerLoadingAnimation.DISABLE_ANIMATION.Invoke();
            if (request.result != UnityWebRequest.Result.Success)
            {
                ThrowServerError();
                request.Dispose();
            }
            
            TouchManager.SetActive(true);
        }

        private static T DeserializeData<T>(string json)
        {
            RequestData requestData = JsonConvert.DeserializeObject<RequestData>(json, new RequestDataConverter());
            object data = requestData?.data;
            return (T)data;
        }

        private static void ThrowServerError()
        {
            ServerConnectionErrorUI.ServerErrorOccuredEvent.Invoke();
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