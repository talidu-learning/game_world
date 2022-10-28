using System;
using System.Text.RegularExpressions;
using GraphQlClient.Core;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Pokemon : MonoBehaviour
{
    public GraphApi pokemonGraph;
    public Text displayText;

    public async void GetPokemonDetails(){
        pokemonGraph.SetAuthToken("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoic3R1ZGVudCIsInVzZXJfaWQiOiJmMDFlY2VjZC00YjhlLTQ4ODctOWYwNi0xZjE0NmUxN2VlNGIiLCJuYW1lIjpudWxsLCJpYXQiOjE2NjU0MDM0NTcsImV4cCI6MTY2NTQ4OTg1NywiYXVkIjoicG9zdGdyYXBoaWxlIiwiaXNzIjoicG9zdGdyYXBoaWxlIn0.3V_Sl3kwedpAD4B8FCOorwNyraL-42lCbAYDRtIYGFM");
        GraphApi.Query query = pokemonGraph.GetQueryByName("UserId", GraphApi.Query.Type.Query);
        UnityWebRequest request = await pokemonGraph.Post(query);
        string pattern = "(?<=\"currentUserId\":\").*\"";
        Regex regex = new Regex(pattern);
        var id = regex.Match(request.downloadHandler.text).Value.Replace('"', ' ').Replace(" ", "");
        Debug.Log(id);
        displayText.text = id;
        
        GraphApi.Query query2 = pokemonGraph.GetQueryByName("Student", GraphApi.Query.Type.Query);
        query2.SetArgs( new {id = new Guid(id)});
        UnityWebRequest request2 = await pokemonGraph.Post(query2);
        
        Debug.Log(request2.downloadHandler.text);
    }
    
}
