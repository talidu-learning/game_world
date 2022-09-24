using System;
using Interactables;
using TouchScript.Gestures;
using UnityEngine;

public class SpawnObjectButton : MonoBehaviour
{
    public GameObject Prefab;
    
    public void SpawnObject()
    {
        var go = Instantiate(Prefab);
        SelectionManager.SELECT_OBJECT_EVENT.Invoke(go.GetComponent<Interactable>());
    }
    
}
