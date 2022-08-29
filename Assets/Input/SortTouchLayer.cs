using TouchScript;
using TouchScript.Layers;
using UnityEngine;

public class SortTouchLayer : MonoBehaviour
{
    public FullscreenLayer FullscreenLayer;
    public StandardLayer Interactables;
    
    private ILayerManager layerManager;
    
    void Awake()
    {
        layerManager = LayerManager.Instance;
    }

    private void Start()
    {
        layerManager.Layers[0] = Interactables;
        layerManager.Layers[1] = FullscreenLayer;
    }
}
