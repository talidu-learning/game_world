using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GraphQlClient.Core;
using Plugins.WebGL;
using Shop;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Game
{
    public class IntUnityEvent : UnityEvent<int>{}
    public class ServerConnection : MonoBehaviour
    {
        public static IntUnityEvent UpdateStarsEvent = new IntUnityEvent();
        
        [SerializeField] private GraphApi taliduGraphApi;

        public static StudentData StudentData;

        public static bool Loaded = false;

        private Guid id;

        private void Awake()
        {
            UpdateStarsEvent.AddListener(UpdateStarCount);
        }

        public async void GetStudentData(){
            
            // WebGLPluginJS.SetUpTestToken();
            // var token = WebGLPluginJS.GetTokenFromLocalStorage();
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoic3R1ZGVudCIsInVzZXJfaWQiOiJmMDFlY2VjZC00YjhlLTQ4ODctOWYwNi0xZjE0NmUxN2VlNGIiLCJuYW1lIjpudWxsLCJpYXQiOjE2NzA1ODIwNTUsImV4cCI6MTY3MDY2ODQ1NSwiYXVkIjoicG9zdGdyYXBoaWxlIiwiaXNzIjoicG9zdGdyYXBoaWxlIn0.QsIg4bcfa1aMRogFZl0WPwm8H2VEFxs8DsJnacrEHxE";
            Debug.Log(token);
            
            taliduGraphApi.SetAuthToken(token);
            id = new Guid(await GetStudentID());

            await GetStudentData(id);

            await GetAllItems(id);

            //await CreateItem(id, "Table");
            
            await GetAllItems(id);

            Loaded = true;

        }

        private void UpdateStarCount(int starCount)
        {
            UpdateStars(id, starCount);
        }

        private async Task CreateItem(Guid guid, string itemid)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("CreateItem", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {input = new{purchasedItem = new{owner = guid, id = itemid}}});
            UnityWebRequest request = await taliduGraphApi.Post(query);
            request.Dispose();
        }
        
        private async Task DeleteItem(Guid guid, string itemid)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("DeleteItem", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {});
            UnityWebRequest request = await taliduGraphApi.Post(query);
            request.Dispose();
        }
        
        private async Task UpdateItem(Guid itemguid, int xCoord, int zCoord, Guid[] socketguids = null)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("UpdateItem", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {purchasedItemPatch = new{sockets = socketguids, x = xCoord, z = zCoord}, uid = itemguid});
            UnityWebRequest request = await taliduGraphApi.Post(query);
            request.Dispose();
        }
        
        private async Task<List<ItemData>> GetAllItems(Guid guid)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("GetAllItems", GraphApi.Query.Type.Query);
            query.SetArgs(new {condition = new {owner = guid}});
            UnityWebRequest request = await taliduGraphApi.Post(query);
            var result = request.downloadHandler.text;
            Debug.Log(result);
            request.Dispose();
            List<ItemData> items = new List<ItemData>();
            
            return items;
        }
        
        private async Task UpdateStars(Guid guid, int starCount)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("UpdateStars", GraphApi.Query.Type.Mutation);
            query.SetArgs(new {input = new{ studentPatch = new{stars = starCount}, id = guid}});
            UnityWebRequest request = await taliduGraphApi.Post(query);
            request.Dispose();
        }

        private async Task<string> GetStudentID()
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("UserId", GraphApi.Query.Type.Query);
            UnityWebRequest request = await taliduGraphApi.Post(query);
            string pattern = "(?<=\"currentUserId\":\").*\"";
            Regex regex = new Regex(pattern);
            var id = regex.Match(request.downloadHandler.text).Value.Replace('"', ' ').Replace(" ", "");
            request.Dispose();
            return id;
        }

        private async Task GetStudentData(Guid guid)
        {
            GraphApi.Query query = taliduGraphApi.GetQueryByName("Student", GraphApi.Query.Type.Query);
            query.SetArgs(new {id = guid});
            UnityWebRequest request = await taliduGraphApi.Post(query);

            Debug.Log(request.downloadHandler.text);
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
            Debug.Log("StudentData: Proficiency: " + StudentData.Stars);
            request.Dispose();
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
