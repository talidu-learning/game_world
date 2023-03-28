using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ServerConnection
{
    /// <summary>
    /// Custom JSON Converter for identifying which kind of data was received from the server.
    /// <seealso cref="RequestData"/>
    /// </summary>
    public class RequestDataConverter : JsonConverter<RequestData>
    {
        public override void WriteJson(JsonWriter writer, RequestData value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override RequestData ReadJson(JsonReader reader, Type objectType, RequestData existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var data = JObject.Load(reader)["data"];

            JToken studentById = data["studentById"];
            JToken query = data["query"];
            JToken updateStudentById = data["updateStudentById"];

            if (updateStudentById != null)
            {
                JToken student = updateStudentById["student"];
                UpdateStudentById studentData = new UpdateStudentById();
                studentData.student = new Student();
                Debug.Log(updateStudentById["student"]);
                Debug.Log(student["stars"]);
                studentData.student.stars = (int)student["stars"];
                UpdateStars updateStars = new UpdateStars();
                updateStars.updateStudentById = studentData;
                return new RequestData{ data = updateStars} ;
            }
            if (studentById != null)
            {
                StudentDataJson studentDataJson = new();
                studentDataJson.studentById = new ();
                studentDataJson.studentById.proficiency= (int)studentById["proficiency"];
                studentDataJson.studentById.stars = (int)studentById["stars"];
                return new RequestData{data = studentDataJson};
            }
            if (query != null)
            {
                IdRequestData idRequestData = new();
                idRequestData.query = new ();
                idRequestData.query.currentUserId = query["currentUserId"].ToString();
                return new RequestData{data = idRequestData};
            }

            return new RequestData();
        }
    }
}