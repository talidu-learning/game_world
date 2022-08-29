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

        public static void Browser_Log(string message)
        {
            Debug.Log(message);
            //PassTextParam(message);
        }

    }
}
