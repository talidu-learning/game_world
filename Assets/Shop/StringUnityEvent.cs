using System;
using UnityEngine.Events;

namespace Shop
{
    public class StringUnityEvent : UnityEvent<string> { }
    
    public class StringGuidUnityEvent : UnityEvent<string, Guid> { }
}