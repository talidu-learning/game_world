using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlClient.Core;
using ServerConnection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Game
{
    public class IntUnityEvent : UnityEvent<int>{}
    public class ServerConnection : MonoBehaviour
    {
        
        [SerializeField] private GraphApi taliduGraphApi;

        public static StudentData StudentData;

        public static bool Loaded = false;

        private Guid id;

        public static List<ItemData> purchasedItems = new List<ItemData>();

        public async Task GetStudentData(){
            
            // WebGLPluginJS.SetUpTestToken();
            // var token = WebGLPluginJS.GetTokenFromLocalStorage();
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoic3R1ZGVudCIsInVzZXJfaWQiOiJmMDFlY2VjZC00YjhlLTQ4ODctOWYwNi0xZjE0NmUxN2VlNGIiLCJuYW1lIjpudWxsLCJpYXQiOjE2NzMyNjg0OTMsImV4cCI6MTY3MzM1NDg5MywiYXVkIjoicG9zdGdyYXBoaWxlIiwiaXNzIjoicG9zdGdyYXBoaWxlIn0.6uKd6Wmc_PQwCvxUZ8zNDLJ3tp_IzWZWwbB30sizrFQ";
            
            taliduGraphApi.SetAuthToken(token);
            id = new Guid(await GetStudentID());

            if (id == null || id == Guid.Empty)
            {
                ServerConnectionErrorUI.ServerErrorOccuredEvent.Invoke();
                return;
            }

            var studentData = await GetStudentData(id);
            
            if (!studentData)
            {
                ServerConnectionErrorUI.ServerErrorOccuredEvent.Invoke();
                return;
            }

            var items = await GetAllItems(id);
            
            if (items == null)
            {
                ServerConnectionErrorUI.ServerErrorOccuredEvent.Invoke();
                return;
            }

            purchasedItems = items;

            // await UpdateItem(purchasedItems[0].uid, 7, -5);
            
            // await CreateItem(id, "Table");
            
            // await GetAllItems(id);

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

        private async Task<ItemData> CreateItem(Guid guid, string itemid)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("CreateItem", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {input = new{purchasedItem = new{owner = guid, id = itemid}}});
            UnityWebRequest request = await taliduGraphApi.Post(query);

            var uid = RegExJsonParser.GetValueOfField("uid", request.downloadHandler.text);

            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.DataProcessingError)
            {
                request.Dispose();
                ServerConnectionErrorUI.ServerErrorOccuredEvent.Invoke();
                return null;
            }

            var newItem = new ItemData
            {
                id = itemid,
                uid = new Guid(uid)
            };
            purchasedItems.Add(newItem);
            
            request.Dispose();
            return newItem;
        }
        
        // public async Task<bool> DeleteItem(Guid guid, string itemid)
        // {
        //     GraphApi.Query query = taliduGraphApi.GetQueryByName("DeleteItem", GraphApi.Query.Type.Mutation);
        //     query.SetArgs(new {});
        //     UnityWebRequest request = await taliduGraphApi.Post(query);
        //     
        //     if (request.result == UnityWebRequest.Result.ConnectionError)
        //     {
        //         request.Dispose();
        //         return false;
        //     }
        //     
        //     request.Dispose();
        //
        //     return true;
        // }
        
        public async void UpdateItemPosition(Guid itemguid, string itemID, float xCoord, float zCoord, Action<bool, string, Guid> callBack = null)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("UpdateItem", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {input= new{purchasedItemPatch= new {x= xCoord, z= zCoord}, uid=itemguid}});
            UnityWebRequest request = await taliduGraphApi.Post(query);
            
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.DataProcessingError)
            {
                request.Dispose();
                callBack?.Invoke(false, itemID, itemguid);
                ServerConnectionErrorUI.ServerErrorOccuredEvent.Invoke();
                return;
            }
            
            request.Dispose();

            callBack?.Invoke(true, itemID, itemguid);
        }
        
        public async void DeleteItem(Guid itemguid, string itemID, Action<bool, string, Guid> callBack = null)
        {
            var itemWithSocket = purchasedItems.FirstOrDefault(i => i.uid == itemguid);

            if (itemWithSocket == null)
            {
                callBack?.Invoke(false,itemID, itemguid);
                return;
            }

            GraphApi.Query query = taliduGraphApi.GetQueryByName("UpdateItem", GraphApi.Query.Type.Mutation);
            
            if (itemWithSocket.itemsPlacedOnSockets != null)
            {
                Guid[] newSockets = new Guid[itemWithSocket.itemsPlacedOnSockets.Length];
                query.SetArgs(new {input= new{purchasedItemPatch= new {sockets = newSockets, x= 0, z= 0}, uid=itemguid}});
            }
            else
            {
                query.SetArgs(new {input= new{purchasedItemPatch= new {x= 0, z= 0}, uid=itemguid}});
            }

            
            UnityWebRequest request = await taliduGraphApi.Post(query);
            
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.DataProcessingError)
            {
                request.Dispose();
                callBack?.Invoke(false, itemID, itemguid);
                ServerConnectionErrorUI.ServerErrorOccuredEvent.Invoke();
                return;
            }
            
            request.Dispose();

            callBack?.Invoke(true, itemID, itemguid);
        }

        public async void OnPlacedItemOnSocket(Guid onSocketPlacedItemguid, int socketcount, int socketindex, Guid itemWithSocketsGuid,string itemId, Action<bool, string, Guid> callBack)
        {
            var itemWithSocket = purchasedItems.FirstOrDefault(i => i.uid == itemWithSocketsGuid);

            if (itemWithSocket == null)
            {
                callBack.Invoke(false,itemId, onSocketPlacedItemguid);
                return;
            }
            
            if (itemWithSocket.itemsPlacedOnSockets == null)
                itemWithSocket.itemsPlacedOnSockets = new Guid[socketcount];
            
            itemWithSocket.itemsPlacedOnSockets[socketindex] = onSocketPlacedItemguid;
            Debug.Log(itemWithSocket.itemsPlacedOnSockets[socketindex]);
            
            GraphApi.Query query = taliduGraphApi.GetQueryByName("UpdateItem", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {input= new{purchasedItemPatch= new {sockets= itemWithSocket.itemsPlacedOnSockets}, uid=itemWithSocketsGuid}});
            UnityWebRequest request = await taliduGraphApi.Post(query);
            
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.DataProcessingError)
            {
                request.Dispose();
                callBack.Invoke(false,itemId, onSocketPlacedItemguid);
                ServerConnectionErrorUI.ServerErrorOccuredEvent.Invoke();
                return;
            }
            
            request.Dispose();

            callBack.Invoke(true,itemId, onSocketPlacedItemguid);
        }
        
        public async void OnDeletedItemOnSocket(Guid onSocketPlacedItemguid, int socketindex, Guid itemWithSocketsGuid, Action<bool, Guid, Guid, int> callBack)
        {
            var itemWithSocket = purchasedItems.FirstOrDefault(i => i.uid == itemWithSocketsGuid);

            if (itemWithSocket == null)
            {
                callBack.Invoke(false,onSocketPlacedItemguid, itemWithSocketsGuid, socketindex);
                return;
            }
            
            itemWithSocket.itemsPlacedOnSockets[socketindex] = Guid.Empty;
            
            GraphApi.Query query = taliduGraphApi.GetQueryByName("UpdateItem", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {input= new{purchasedItemPatch= new {sockets= itemWithSocket.itemsPlacedOnSockets}, uid=itemWithSocketsGuid}});
            UnityWebRequest request = await taliduGraphApi.Post(query);
            
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.DataProcessingError)
            {
                request.Dispose();
                callBack.Invoke(false,onSocketPlacedItemguid, itemWithSocketsGuid, socketindex);
                ServerConnectionErrorUI.ServerErrorOccuredEvent.Invoke();
                return;
            }
            
            request.Dispose();

            callBack.Invoke(true,onSocketPlacedItemguid, itemWithSocketsGuid, socketindex);
        }
        
        public async Task<bool> UpdateItemSockets(Guid itemguid, Guid[] socketguids, Action<bool> callBack = null)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("UpdateItem", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {input= new{purchasedItemPatch= new {sockets= socketguids}, uid=itemguid}});
            UnityWebRequest request = await taliduGraphApi.Post(query);
            
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.DataProcessingError)
            {
                request.Dispose();
                callBack?.Invoke(false);
                ServerConnectionErrorUI.ServerErrorOccuredEvent.Invoke();
                return false;
            }
            
            request.Dispose();

            callBack?.Invoke(true);
            return true;
        }
        
        public async Task<List<ItemData>> GetAllItems(Guid guid)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("GetAllItems", GraphApi.Query.Type.Query);
            query.SetArgs(new {condition = new {owner = guid}});
            UnityWebRequest request = await taliduGraphApi.Post(query);

            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.DataProcessingError)
            {
                ServerConnectionErrorUI.ServerErrorOccuredEvent.Invoke();
                request.Dispose();
                return null;
            }
            
            var result = request.downloadHandler.text;
            request.Dispose();
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
                        itemsPlacedOnSockets = socketData
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
                    };    
                    items.Add(itemData);
                }
                
            }
            Debug.Log("Purchased Items: " + items.Count);
            return items;
        }
        
        public async Task<bool> UpdateStars(Guid guid, int starCount)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("UpdateStars", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {input = new{ studentPatch = new{stars = starCount}, id = guid}});
            UnityWebRequest request = await taliduGraphApi.Post(query);
            
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.DataProcessingError)
            {
                ServerConnectionErrorUI.ServerErrorOccuredEvent.Invoke();
                request.Dispose();
                return false;
            }
            
            request.Dispose();
            return true;
        }

        public async Task<string> GetStudentID()
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("UserId", GraphApi.Query.Type.Query);
            UnityWebRequest request = await taliduGraphApi.Post(query);
            
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.DataProcessingError)
            {
                ServerConnectionErrorUI.ServerErrorOccuredEvent.Invoke();
                request.Dispose();
                return String.Empty;
            }
            
            
            var id = RegExJsonParser.GetValueOfField("currentUserId", request.downloadHandler.text);
            request.Dispose();
            return id;
        }

        public async Task<bool> GetStudentData(Guid guid)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("Student", GraphApi.Query.Type.Query);
            query.SetArgs(new {id = guid});
            UnityWebRequest request = await taliduGraphApi.Post(query);
            
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.DataProcessingError)
            {
                ServerConnectionErrorUI.ServerErrorOccuredEvent.Invoke();
                request.Dispose();
                return false;
            }

            var dataString = request.downloadHandler.text.Replace("{\"data\":{\"studentById\":{", "")
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
            request.Dispose();

            return true;
        }
    }

    [Serializable]
    public class StudentData
    {
        public string Name;
        public int Age;
        public int Proficiency;
        public int Stars;
    }

}
