using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class InventoryPanelPositionig : MonoBehaviour
{
    public bool validate;

    private void OnValidate()
    {
        GetComponent<RectTransform>().position = new Vector2(transform.position.x, - Screen.height );
    }

    private void Awake()
    {
        GetComponent<RectTransform>().position = new Vector2(transform.position.x, -Screen.height);
    }
}
