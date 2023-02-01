using Interactables;
using UnityEngine;
using UnityEngine.UI;

public class FlipItemButton : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        SelectionManager.FLIP_OBJECT_EVENT.Invoke();
    }
}
