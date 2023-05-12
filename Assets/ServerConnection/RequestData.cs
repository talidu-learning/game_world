using Newtonsoft.Json;

namespace ServerConnection
{
    /// <summary>
    /// Container class for all types of requests
    /// </summary>
    public class RequestData
    {
        [JsonConverter(typeof(RequestDataConverter))]
        public JsonData data { get; set; }
    }
    
    /// <summary>
    /// Every data class needs to inherit from this class.
    /// Then go to <see cref="RequestDataConverter"/> in order to tell the converter,
    /// how to parse this kind of <see cref="JsonData"/>.
    /// An instance of this class is returned, if the conversion did not match any of the child classes.
    /// </summary>
    public class JsonData { }

    #region UpdateStars
    public class UpdateStars : JsonData
    {
        public UpdateStudentById updateStudentById;
    }

    public class UpdateStudentById
    {
        public Student student;
    }

    public class Student
    {
        public int stars;
    }
    
    #endregion

    #region StudentDataJson

    public class StudentDataJson : JsonData
    {
        public StudentById studentById;   
    }
    
    public class StudentById
    {
        public int proficiency;
        public int stars;
    }

    #endregion

    #region IdRequest

    public class IdRequestData : JsonData
    {
        public Query query;
    }

    public class Query
    {
        public string currentUserId;
    }

    #endregion
    
}