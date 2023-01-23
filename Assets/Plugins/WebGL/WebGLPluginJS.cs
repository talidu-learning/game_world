using System.Runtime.InteropServices;
using UnityEngine;

namespace Plugins.WebGL
{
    /// <summary>
    /// Class with a JS Plugin functions for WebGL.
    /// </summary>
    public class WebGLPluginJS : MonoBehaviour
    {
        [DllImport("__Internal")]
        public static extern void PassTextParam(string text);

        [DllImport("__Internal")]
        public static extern void SetTestToken();

        [DllImport("__Internal")]
        public static extern string GetToken();

        public static void Browser_Log(string message)
        {
            Debug.Log(message);
            //PassTextParam(message);
        }

        public static void SetUpTestToken()
        {
            SetTestToken();
        }

        public static string GetTokenFromLocalStorage()
        {
            return GetToken();
        }
    }
}