using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GraphQlClient.Core;
using Newtonsoft.Json;
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

        public async void GetStudentData(){
            
            // WebGLPluginJS.SetUpTestToken();
            // var token = WebGLPluginJS.GetTokenFromLocalStorage();
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoic3R1ZGVudCIsInVzZXJfaWQiOiJmMDFlY2VjZC00YjhlLTQ4ODctOWYwNi0xZjE0NmUxN2VlNGIiLCJuYW1lIjpudWxsLCJpYXQiOjE2NzI3NDA0NDUsImV4cCI6MTY3MjgyNjg0NSwiYXVkIjoicG9zdGdyYXBoaWxlIiwiaXNzIjoicG9zdGdyYXBoaWxlIn0.gLdaDkJZysYnuNN8l4sysXHV5rhAvMLdogyJnTlncl4";
            
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

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                request.Dispose();
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
        
        public async Task<bool> DeleteItem(Guid guid, string itemid)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("DeleteItem", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {});
            UnityWebRequest request = await taliduGraphApi.Post(query);
            
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                request.Dispose();
                return false;
            }
            
            request.Dispose();

            return true;
        }
        
        public async Task<bool> UpdateItemPosition(Guid itemguid, float xCoord, float zCoord, Guid[] socketguids = null)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("UpdateItem", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {input= new{purchasedItemPatch= new {sockets= socketguids, x= xCoord, z= zCoord}, uid=itemguid}});
            UnityWebRequest request = await taliduGraphApi.Post(query);
            
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                request.Dispose();
                return false;
            }
            
            request.Dispose();

            return true;
        }
        
        public async Task<bool> UpdateItemSockets(Guid itemguid, Guid[] socketguids)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("UpdateItem", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {input= new{purchasedItemPatch= new {sockets= socketguids}, uid=itemguid}});
            UnityWebRequest request = await taliduGraphApi.Post(query);
            
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                request.Dispose();
                return false;
            }
            
            request.Dispose();

            return true;
        }
        
        public async Task<List<ItemData>> GetAllItems(Guid guid)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("GetAllItems", GraphApi.Query.Type.Query);
            query.SetArgs(new {condition = new {owner = guid}});
            UnityWebRequest request = await taliduGraphApi.Post(query);

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                request.Dispose();
                return null;
            }
            
            var result = request.downloadHandler.text;
            request.Dispose();
            var deserializedData = AllPurchasedItemsDataContainer.FromJson(result);
            
            List<ItemData> items = new List<ItemData>();

            foreach (var node in deserializedData.Data.AllPurchasedItems.Nodes)
            {
                ItemData itemData = new ItemData
                {
                    nodeId = node.NodeId,
                    id = node.Id,
                    uid = node.Uid,
                    x = Convert.ToSingle(node.X),
                    z = Convert.ToSingle(node.Z)
                };
                items.Add(itemData);
            }
            Debug.Log("Purchased Items: " + items.Count);
            return items;
        }
        
        public async Task<bool> UpdateStars(Guid guid, int starCount)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("UpdateStars", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {input = new{ studentPatch = new{stars = starCount}, id = guid}});
            UnityWebRequest request = await taliduGraphApi.Post(query);
            
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
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
            
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
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
            
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
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
