using System;
using Interactables;
using TouchScript.Gestures;
using UnityEngine;

public class SpawnObjectButton : MonoBehaviour
{
    public GameObject Prefab;
    
    // private TapGesture _tapgesture;
    // void Awake()
    // {
    //     _tapgesture = GetComponent<TapGesture>();
    //     _tapgesture.Tapped += SpawnObject;
    // }
    
    private void SpawnObject(object sender, EventArgs e)
    {
        var go = Instantiate(Prefab);
        SelectionManager.SELECT_OBJECT_EVENT.Invoke(go.GetComponent<Interactable>());
    }

    public void SpawnObject()
    {
        var go = Instantiate(Prefab);
        SelectionManager.SELECT_OBJECT_EVENT.Invoke(go.GetComponent<Interactable>());
    }
    
}
